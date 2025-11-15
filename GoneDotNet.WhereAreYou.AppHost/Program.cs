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

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var clustering = storage.AddTables("clustering");
var streaming = storage.AddQueues("streaming");
var grainStore = storage.AddBlobs("grain-state");
var reminders = storage.AddTables("reminders");
var pubSubStore = storage.AddBlobs("pubsub-storage");

var orleans = builder
    .AddOrleans("gdn-orleans")
    .WithClusterId("gdn-cluster")
    .WithServiceId("dgn-service")
    .WithClustering(clustering)
    .WithReminders(reminders)
    .WithStreaming("StreamProvider", streaming)
    .WithGrainStorage("PubSubStore", pubSubStore)
    .WithGrainStorage("Default", grainStore);

var silo = builder.AddProject<GoneDotNet_WhereAreYou_OrleansServer>("silo")
    .WithReference(orleans)
    .WithReference(clustering)
    .WaitFor(clustering)
    .WithReference(reminders)
    .WaitFor(reminders)
    .WithReference(streaming)
    .WaitFor(streaming)
    .WithReference(grainStore)
    .WaitFor(grainStore)
    .WithReference(pubSubStore)
    .WaitFor(pubSubStore);

var webapi = builder
    .AddProject<GoneDotNet_WhereAreYou_Api>("webapi")
    .WithExternalHttpEndpoints()
    .WithReference(database)
    .WithReference(orleans.AsClient())
    .WaitFor(silo);

if (!builder.ExecutionContext.IsPublishMode && !String.IsNullOrWhiteSpace(builder.Configuration["NGrok:AuthToken"]))
{
    builder
        .AddNgrok("ngrok")
        .WithAuthToken(builder.Configuration["NGrok:AuthToken"]!)
        .WithTunnelEndpoint(webapi, "http", builder.Configuration["NGrok:Url"]!)
        .WaitFor(webapi);
}
builder.Build().Run();