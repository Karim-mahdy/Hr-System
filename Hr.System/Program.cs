using HealthChecks.UI.Client;
using Hr.Application.Common.Filter;
using Hr.Application.Interfaces;
using Hr.Application.Services.implementation;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using Hr.Infrastructure.Data;
using Hr.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


namespace Hr.System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(option =>
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //     check health of database connection 
            builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
                option =>
                {
                    option.Password.RequireNonAlphanumeric = true;
                    option.Password.RequiredLength = 5;
                }

                ).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var serviceBuilder = builder.Services.BuildServiceProvider();


            #region DI
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IAttendanceServices, AttendanceServices>();
            builder.Services.AddScoped<IEmployeeServices, EmployeeServices>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddScoped<IPublicHolidaysService, PublicHolidaysService>();
            builder.Services.AddScoped<IWeekendService, WeekendService>();
            builder.Services.AddScoped<IGeneralSettingsService, GeneralSettingsService>();


            //Filter Permission // Authorization Services
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });
            #endregion

            #region JWT
            builder.Services.AddAuthentication(options =>
                       {
                           options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                           options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                           options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                       }).AddJwtBearer(options =>
                       {
                           options.SaveToken = true;
                           options.RequireHttpsMetadata = false;
                           options.TokenValidationParameters = new TokenValidationParameters()
                           {
                               ValidateIssuer = true,
                               ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                               ValidateAudience = true,
                               ValidAudience = builder.Configuration["JWT:ValidAudiance"],
                               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                           };
                       });
            #endregion

            #region CORS

            builder.Services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("MyPolicy", corsPolicyBuilder =>
                {
                    corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            #endregion

            #region Swagger Configurations

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo", Version = "v1" });
            });
            builder.Services.AddSwaggerGen(swagger =>
            {
                //This�is�to�generate�the�Default�UI�of�Swagger�Documentation����
                swagger.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET�7�Web�API",
                    Description = " Hr System"
                });

                //�To�Enable�authorization�using�Swagger�(JWT)����
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter�'Bearer'�[space]�and�then�your�valid�token�in�the�text�input�below.\r\n\r\nExample:�\"Bearer�eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] {}
                    }
                });
            });
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsEnvironment("Local"))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hr System");
                    c.RoutePrefix = "swagger"; // Set the Swagger UI route prefix
                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting(); // Add UseRouting here
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            // healthy check 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                }).AllowAnonymous(); // This line allows anonymous access to the health check endpoint
            });

            app.MapControllers();

            // Apply migrations and seeding if the database is healthy
            var healthCheckService = app.Services.GetRequiredService<HealthCheckService>();
            var healthCheckResult = await healthCheckService.CheckHealthAsync();

            if (healthCheckResult.Status == HealthStatus.Healthy)
            {
                DbInitializer.InitializeDatabase(app);
            }
            else
            {
                Console.WriteLine("Database connection is unhealthy. Migration and seeding skipped.");
            }

            app.Run();
        }
    }
}
