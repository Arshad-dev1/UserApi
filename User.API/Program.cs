using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using UserApi.DB;
using UserApi.Services;
using UserApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Added dbPath within data folder
var dbPath = Path.GetFullPath(builder.Configuration["Database:Path"] ?? "data/app.db");
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
builder.Services.AddDbContext<UserDbContext>(o => o.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

builder.Services.AddControllers();

var auth = builder.Configuration.GetSection("Authentication");
var signingKey = auth["SigningKey"];
var issuer = auth["Issuer"];
var audience = auth["Audience"];
var username = auth["Username"];
var password = auth["Password"];
var role = auth["Role"];
var tokenLifetimeMinutes = auth.GetValue<int?>("TokenLifetimeMinutes");
if (string.IsNullOrWhiteSpace(signingKey) || signingKey.Length < 32 ||
    string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience) ||
    string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
    string.IsNullOrWhiteSpace(role) || tokenLifetimeMinutes is null or <= 0)
{
    throw new InvalidOperationException(
        "Authentication is not configured. Set issuer, audience, username, password, role, a positive token lifetime, and a signing key of at least 32 characters under Authentication.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, ValidIssuer = issuer,
            ValidateAudience = true, ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization(options =>
    options.AddPolicy("DirectoryAdministrator", policy => policy.RequireRole(role)));

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the access token returned by POST /api/auth/login."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

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

app.UseExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
