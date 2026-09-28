using Microsoft.EntityFrameworkCore;
using UserApi.DB;

var builder = WebApplication.CreateBuilder(args);

// Added dbPath within data folder
var dbPath = Path.GetFullPath(builder.Configuration["Database:Path"] ?? "data/app.db");
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
builder.Services.AddDbContext<UserDbContext>(o => o.UseSqlite($"Data Source={dbPath}"));



builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
              ?? ["http://localhost:5173"];
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.EnsureCreated();
    if (app.Configuration.GetValue("Database:Seed", true)) DBSeeder.Seed(db);
}

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.UseExceptionHandler();
app.UseCors();

app.MapControllers();

app.Run();
