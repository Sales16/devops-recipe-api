using Microsoft.EntityFrameworkCore;
using ReceitasAPI.Data;

var builder = WebApplication.CreateBuilder(args);

var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
var connectionString = $"Host=db;Port=5432;Database={dbName};Username={dbUser};Password={dbPassword}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls("http://0.0.0.0:80");
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
