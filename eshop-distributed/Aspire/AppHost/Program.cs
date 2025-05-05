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

var catalog =
    builder.AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

var basketDb =
    builder.AddProject<Projects.Basket>("basket")
      .WithReference(cache)
      .WithReference(catalog)
      .WaitFor(cache);



builder.Build().Run();
