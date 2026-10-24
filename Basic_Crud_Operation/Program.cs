using Basic_Crud_Operation.Data;
using Basic_Crud_Operation.Repository;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/startup-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();


try
{
    Log.Information("Starting Product API");

    var builder = WebApplication.CreateBuilder(args);


    // Replace the default logging with Serilog, configured from appsettings.json
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    // Add services to the container.

    builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();


// Registers AppDbContext and tells EF to use SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// Dependency Injection: whenever IProductRepository is requested, give ProductRepository
builder.Services.AddScoped<IProductRepository, ProductRepository>();


var app = builder.Build();

//Auto-Create Database and Seed Data at Startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseSerilogRequestLogging();   // one log line per HTTP request

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();   // makes sure the last logs are written to the file
}