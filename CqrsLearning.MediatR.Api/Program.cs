using CqrsLearning.MediatR.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ProductsDb")
        ?? "Data Source=App_Data/mediatr-products.db";

    var connectionStringBuilder = new SqliteConnectionStringBuilder(connectionString);

    if (!Path.IsPathRooted(connectionStringBuilder.DataSource))
    {
        var dbPath = Path.GetFullPath(
            Path.Combine(builder.Environment.ContentRootPath, connectionStringBuilder.DataSource));

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        connectionStringBuilder.DataSource = dbPath;
    }

    options.UseSqlite(connectionStringBuilder.ToString());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
