using GoneDotNet.WhereAreYou.DbMigrations;

var builder = Host.CreateApplicationBuilder(args);
builder.AddAppDbContext();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
