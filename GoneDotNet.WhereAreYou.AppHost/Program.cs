using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

const string Volume = "gonedotnet-data-volume";

var dbPassword = builder.AddParameter("DatabasePassword", secret: true);
var postgres = builder
    .AddPostgres("gonedotnet")
    .WithPassword(dbPassword)
    .WithDataVolume(Volume)
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("gonedotnetdb");

var redis = builder
    .AddRedis("redis")
    .WithDataVolume(Volume);

var orleans = builder
    .AddOrleans("gdn-orleans")
    .WithClusterId("gdn-cluster")
    .WithServiceId("dgn-service")
    // .WithStreaming()
    // .WithBroadcastChannel()
    .WithClustering(redis)
    .WithGrainStorage("Default", redis);

var webapi = builder
    .AddProject<GoneDotNet_WhereAreYou_Api>("webapi")
    .WithExternalHttpEndpoints()
    .WithReference(database)
    .WithReference(orleans)
    .WaitFor(database);

if (builder.Environment.IsDevelopment())
{
    redis.WithRedisCommander();
    
    builder
        .AddNgrok("ngrok")
        .WithAuthToken(builder.Configuration["NGrok:AuthToken"]!)
        .WithTunnelEndpoint(webapi, "http", builder.Configuration["NGrok:Url"]!)
        .WaitFor(webapi);
}
builder.Build().Run();