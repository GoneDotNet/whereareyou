using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder
    .AddPostgres("gonedotnet");

var database = postgres.AddDatabase("gonedotnetdb");

var webapi = builder
    .AddProject<GoneDotNet_WhereAreYou_Api>("webapi")
    .WithExternalHttpEndpoints()
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();