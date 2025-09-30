using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.CustomRepositories.Implements;
using Repositories.CustomRepositories.Interfaces;
using Repositories.WorkSeeds.Implements;
using Repositories.WorkSeeds.Interfaces;
using Services.Helper.AutoMapper;
using Services.Implements;
using Services.Intefaces;

namespace ElectricVehicleDealerManagermentSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //SQL connection
            builder.Services.AddDbContext<Prn222asm1Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

            // Generic repository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Custom repositories
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IDealerTypeRepository, DealerTypeRepository>();
            builder.Services.AddScoped<IDealerTypeRepository, DealerTypeRepository>();
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<IVehicleCategoryRepository, VehicleCategoryRepository>();

            // Services
            builder.Services.AddScoped<IVehicleServices, VehicleServices>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();

            //auto mapper
            builder.Services.AddAutoMapper(cfg => { }, typeof(MapperProfile));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
