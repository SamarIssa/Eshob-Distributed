var builder = DistributedApplication.CreateBuilder(args);

//add projects and cloud native backing services
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);


var catalogDb = postgres.AddDatabase("catalogDb");

var cache =
    builder.AddRedis("redis")
    .WithRedisInsight()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitMq =
    builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var catalog =
    builder.AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(rabbitMq)
    .WaitFor(catalogDb)
    .WaitFor(rabbitMq);

var basketDb =
    builder.AddProject<Projects.Basket>("basket")
      .WithReference(cache)
      .WithReference(catalog)
        .WithReference(rabbitMq)
      .WaitFor(cache)
      .WaitFor(rabbitMq);



builder.AddProject<Projects.WebApp>("webapp")
    .WithExternalHttpEndpoints()
    .WithReference(catalog)
    .WithReference(basketDb)
    .WaitFor(basketDb)
    .WaitFor(catalog);
    ;



builder.Build().Run();
