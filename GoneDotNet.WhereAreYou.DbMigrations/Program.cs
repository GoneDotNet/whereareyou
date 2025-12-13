using GoneDotNet.WhereAreYou.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddAppDbContext();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var strategy = dbContext.Database.CreateExecutionStrategy();
await strategy.ExecuteAsync(async () =>
{
    Console.WriteLine("Dropping and recreating the database...");
    
    var dbExistedBeforeMigration = await dbContext.Database.CanConnectAsync();
    await dbContext.Database.MigrateAsync();
    if (!dbExistedBeforeMigration)
    {
        Console.WriteLine("Seeding initial data...");
        // await DataSeeder.SeedDataAsync(dbContext);
    }
});
Console.WriteLine("Db migration completed");