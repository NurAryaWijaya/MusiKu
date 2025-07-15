using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MusiKu_API.Data;
using MusiKu_API.Models;
using MusiKu_API.Services;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Database
var connectionString = "server=localhost;user=root;password=NurAryaWijaya140106;database=musiku_app";
var serverVersion = new MySqlServerVersion(new Version(8, 0, 41));

builder.Services.AddDbContext<MusiKuContext>(options =>
    options.UseMySql(connectionString, serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Debug)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<MusiKuContext>()
    .AddDefaultTokenProviders();

// ===== Tambahkan Authentication JWT =====
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "secret1234567890"))
    };
});
// =========================================

// Mendaftarkan EmailService
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddMemoryCache();


// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MusiKu_API", Version = "v1" });

    // Setup Bearer Authorization di Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Masukkan 'Bearer' diikuti spasi dan token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }});
    c.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<CreatePlaylistDtoExample>();

var app = builder.Build();


// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Harus urut, Authentication dulu baru Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
