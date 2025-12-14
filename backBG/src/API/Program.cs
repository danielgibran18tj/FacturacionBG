using API.Middleware;
using Application.Common.Interfaces;
using Application.Common.Settings;
using Application.DTOs;
using Application.Services;
using Application.Validators;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // CONFIGURACION DE SERILOG
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context, services, configuration) =>
                                    configuration
                                        .ReadFrom.Configuration(context.Configuration)
                                        .ReadFrom.Services(services)
                                        .Enrich.FromLogContext()
                                        .Enrich.WithProperty("Application", "FacturacionAPI"));

                // CONFIGURACION DE SERVICIOS
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();

                // Swagger con JWT
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Banco Guayaquil - API",
                        Version = "v1",
                        Description = "API de facturacion con JWT para prueba tecnica"
                    });

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });

                // CONFIGURACION DE JWT
                var jwtSettings = builder.Configuration.GetSection("JwtSettings");
                builder.Services.Configure<JwtSettings>(jwtSettings);

                var jwtConfig = jwtSettings.Get<JwtSettings>()
                    ?? throw new InvalidOperationException("JwtSettings no esta configurado en appsettings.json");

                var key = Encoding.UTF8.GetBytes(jwtConfig.Secret);

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                });

                builder.Services.AddAuthorization();

                // CONFIGURACION DE BASE DE DATOS
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("DefaultConnection"),
                        sql => sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    ));

                // INYECCION DE DEPENDENCIAS
                builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
                builder.Services.AddAutoMapper(typeof(MapperProfile));

                builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
                builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
                builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
                builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
                builder.Services.AddScoped<IRoleRepository, RoleRepository>();
                builder.Services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
                builder.Services.AddScoped<IUserRepository, UserRepository>();

                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
                builder.Services.AddScoped<ICustomerService, CustomerService>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IInvoiceService, InvoiceService>();
                builder.Services.AddScoped<IJwtService, JwtService>();
                builder.Services.AddScoped<IProductService, ProductService>();
                builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

                builder.Services.Configure<CompanySettings>(builder.Configuration.GetSection("CompanySettings"));


                // CONFIGURACION DE CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAngular", policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                });

                var app = builder.Build();


                // CONFIGURACION DEL PIPELINE HTTP
                // if (app.Environment.IsDevelopment())
                // {
                // AUTO-MIGRACION AL LEVANTAR LA APP
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                // Para usar migraciones
                //await context.Database.MigrateAsync();

                await SeedInitialDataAsync(context, app);

                app.UseSwagger();
                app.UseSwaggerUI();
                // }

                app.UseMiddleware<ExceptionMiddleware>();

                app.UseHttpsRedirection();
                app.UseCors("AllowAngular");
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "La aplicacion cerro inesperadamente");
                throw;
            }
            finally
            {
                Log.Information("=== Cerrando aplicacion ===");
                Log.CloseAndFlush();
            }
        }


        // Crea datos iniciales si no existen
        private static async Task SeedInitialDataAsync(ApplicationDbContext context, WebApplication app)
        {
            try
            {
                // Verificar si ya existen usuarios
                var hasUsers = await context.Users.AnyAsync();

                if (!hasUsers)
                {
                    var path = "";
                    if (app.Environment.IsDevelopment())
                    {
                        path = Path.GetFullPath("../../scripts/database init Data.sql");
                    }
                    else
                    {
                        path = Path.Combine(AppContext.BaseDirectory, "scripts", "database BillingDB.sql");
                    }

                    if (!File.Exists(path))
                    {
                        Log.Warning($"No se encontro el script SQL: {path}");
                        return;
                    }

                    var script = await File.ReadAllTextAsync(path);

                    if (string.IsNullOrWhiteSpace(script))
                    {
                        Log.Warning("El script SQL esta vacio");
                        return;
                    }

                    await context.Database.ExecuteSqlRawAsync(script);

                    Log.Information("Datos iniciales de tienda creados");

                }
                else
                {
                    Log.Information("Datos iniciales ya existen");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "No se pudieron crear datos iniciales: {Message}", ex.Message);
            }
        }

    }
}