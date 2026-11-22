using LootHub.Application;
using LootHub.Infrastructure;
using LootHub.Infrastructure.Authentication;
using LootHub.WebApi.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;
if (string.IsNullOrEmpty(jwtOptions.SecretKey))
    throw new InvalidOperationException("Jwt:SecretKey is missing. Add it to User Secrets.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ValidateLifetime = true,
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();


builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();



var app = builder.Build();

await IdentitySeeder.SeedAsync(app.Services, app.Configuration);

if (app.Environment.IsDevelopment())
{
    await DevDataSeeder.SeedAsync(app.Services);
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
