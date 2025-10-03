using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.CustomRepositories.Implements;
using Repositories.CustomRepositories.Interfaces;
using Repositories.WorkSeeds.Implements;
using Repositories.WorkSeeds.Interfaces;
using Services.Helper.AutoMapper;
using Services.Implements;
using Services.Intefaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Services.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Runtime;

namespace ElectricVehicleDealerManagermentSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure garbage collection for better performance
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

            // Configure file upload limits with more conservative settings
            builder.Services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = 2 * 1024 * 1024; // 2MB limit
                options.MultipartHeadersLengthLimit = int.MaxValue;
                options.MemoryBufferThreshold = 256 * 1024; // 256KB buffer (smaller)
                options.BufferBodyLengthLimit = 2 * 1024 * 1024; // 2MB limit for buffering
            });

            // Configure Kestrel server limits with more conservative settings
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 2 * 1024 * 1024; // 2MB limit
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(1);
                options.Limits.MaxConcurrentConnections = 50; // Reduced from 100
                options.Limits.MaxConcurrentUpgradedConnections = 50; // Reduced from 100
            });

            // Configure IIS server limits (if deploying to IIS)
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 2 * 1024 * 1024; // 2MB limit
            });

            // Configure JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                            {
                                context.Token = context.Request.Cookies["X-Access-Token"];
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            //SQL connection
            builder.Services.AddDbContext<Prn222asm1Context>(options => 
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                       .EnableSensitiveDataLogging(false) // Disable in production
                       .EnableServiceProviderCaching(true)
                       .EnableDetailedErrors(false); // Disable in production
            }, ServiceLifetime.Scoped);

            // Register repositories
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            
            // Register custom repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IDealerTypeRepository, DealerTypeRepository>();
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IInventoryRequestRepository, InventoryRequestRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<IVehicleCategoryRepository, VehicleCategoryRepository>();

            // Register services
            builder.Services.AddScoped<IVehicleServices, VehicleServices>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<IOrderServices, OrderService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            //auto mapper
            builder.Services.AddAutoMapper(cfg => { }, typeof(MapperProfile));

            // Configure session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(3);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add logging
            builder.Services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Add health check endpoint for connection testing
            app.MapGet("/health", () => "OK");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
