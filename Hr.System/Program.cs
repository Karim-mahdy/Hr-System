using Hr.Application.Interfaces;
using Hr.Application.Services.implementation;
using Hr.Application.Services.Interfaces;
using Hr.Domain.Entities;
using Hr.Infrastructure.Data;
using Hr.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hr.System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(option =>
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<Employee, IdentityRole>(
                option =>
                {
                    option.Password.RequireNonAlphanumeric = false;
                    option.Password.RequiredLength = 5;
                }

                ).AddEntityFrameworkStores<ApplicationDbContext>();
            

             //builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IPublicHolidaysService, PublicHolidaysService>();
            builder.Services.AddScoped<IWeekendService, WeekendService>();
            builder.Services.AddScoped<IGeneralSettingsService, GeneralSettingsService>();



            //CORS configration
            builder.Services.AddCors(corsOptions => {
                corsOptions.AddPolicy("MyPolicy", corsPolicyBuilder =>
                {
                    corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthorization();


            app.MapControllers();
            DbInitializer.Configure(app);
            app.Run();
        }
    }
}