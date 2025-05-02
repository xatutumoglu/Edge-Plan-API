using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EdgePlan.API.Services;
using EdgePlan.API.Transformers;
using EdgePlan.Data.Postgre;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<ApplicationPostgreContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgre")));

//Services
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddAuthorizationBuilder();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("EdgePlan API");
        options.WithTheme(ScalarTheme.Default);
    });
}

app.UseAuthentication();
app.UseMiddleware<TokenMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();

app.Run();