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
   // .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitMq =
    builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    //.WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

if(builder.ExecutionContext.IsRunMode)
{
    cache.WithDataVolume();
    rabbitMq.WithDataVolume();
}


var ollama = builder
    .AddOllama("ollama", 11434)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOpenWebUI();

var llama = ollama.AddModel("llama3.2");
var embedding = ollama.AddModel("all-minilm");

// Projects
var catalog = builder
    .AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(rabbitMq)
    .WithReference(llama)
    .WithReference(embedding)
    .WaitFor(catalogDb)
    .WaitFor(rabbitMq)
    .WaitFor(llama)
    .WaitFor(embedding);

var basket = builder
    .AddProject<Projects.Basket>("basket")
    .WithReference(cache)
    .WithReference(catalog)
    .WithReference(rabbitMq)
    .WaitFor(cache)
    .WaitFor(rabbitMq);

var webapp = builder
    .AddProject<Projects.WebApp>("webapp")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(catalog)
    .WithReference(basket)
    .WaitFor(catalog)
    .WaitFor(basket);



builder.Build().Run();
