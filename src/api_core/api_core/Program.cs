using Application;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. DATABASE
builder.Services.AddDbContext<DataDBContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(DataDBContext).Assembly.FullName)
    )
);

// 2. IDENTITY
builder.Services.AddIdentityCore<User>(options =>
{
    // Configurar opciones de Identity si es necesario
})
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<DataDBContext>()
.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<User, IdentityRole>>();

builder.Services.TryAddSingleton<TimeProvider>(TimeProvider.System);

// 3. AUTHENTICATION - JWT
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });

// 4. APPLICATION & INFRASTRUCTURE LAYERS (ESTE ORDEN)
builder.Services.AddApplicationServices(builder.Configuration);    // Primero Application
builder.Services.AddInfrastructureServices(builder.Configuration); // Luego Infrastructure

// 5. CONTROLLERS
builder.Services.AddControllers(opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
})
.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// 6. CORS
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 7. SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================================
// BUILD APP
// ============================================
var app = builder.Build();

// 8. MIDDLEWARE PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();  // IMPORTANTE: Antes de Authorization
app.UseAuthorization();

app.MapControllers();

// 9. DATABASE MIGRATIONS (OPCIONAL - para desarrollo)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataDBContext>();
    dbContext.Database.Migrate(); // Ejecuta migraciones pendientes
}

app.Run();