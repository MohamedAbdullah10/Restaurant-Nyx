using DAL;
using DAL.Repositories.Interfaces;
using DAL.Repositories;
using BLL.Interfaces; // ���� �� ���� ��� using ��
using BLL.Services;   // ���� �� ���� ��� using ��
using Microsoft.EntityFrameworkCore;
using PL.Middleware;
using PL.Controllers;

namespace PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Add services to the container.

            // 1. Configure Database Context
            builder.Services.AddDbContext<AppDbContext>(option => {
                option.UseSqlServer(ConnectionString);
            });

            // 2. Register Unit of Work (which manages all specific repositories)
            // ��� �� ��� UnitOfWork �� ���� ����� ��� Repositories� ������ �� ������ ���� �� Repository �����.
            // ��� UnitOfWork ����� �� �� Repositories ������� ��� ��� DbContext instance.
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 3. Register BLL Services
            // ���� ���� ��� Interfaces �� ��� Implementations �������
            builder.Services.AddScoped<IMenuItemService, MenuItemService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>(); // <<=== ��� �� ����� ���� ��� ������

            // 4. Register generic repository (Optional if only using specific repos via UoW)
            // ����� �� ���� ���� ������� �� �� �������� ��� Repositories ���� �� ���� ��� UnitOfWork
            // ��� �� ����� ����� �� �����.
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ILogger<HomeController>, Logger<HomeController>>();

            // Other service registrations
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            app.UseMiddleware<ResetAvailabilityMiddleware>();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
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