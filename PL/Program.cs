using DAL;
using DAL.Repositories.Interfaces;
using DAL.Repositories;
using BLL.Interfaces; //  √ﬂœ „‰ ÊÃÊœ «·‹ using œÂ
using BLL.Services;   //  √ﬂœ „‰ ÊÃÊœ «·‹ using œÂ
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
            // »„« ≈‰ «·‹ UnitOfWork ÂÊ «··Ì »ÌÊ›— «·‹ Repositories° €«·»« „‘ Â Õ «Ã  ”Ã· ﬂ· Repository ·ÊÕœÂ.
            // «·‹ UnitOfWork »Ì÷„‰ ≈‰ ﬂ· Repositories » ” Œœ„ ‰›” «·‹ DbContext instance.
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 3. Register BLL Services
            // ·«“„  ”Ã· «·‹ Interfaces „⁄ «·‹ Implementations » «⁄ Â«
            builder.Services.AddScoped<IMenuItemService, MenuItemService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>(); // <<=== Â–« ÂÊ «·”ÿ— «·–Ì ﬂ«‰ ‰«ﬁ’«

            // 4. Register generic repository (Optional if only using specific repos via UoW)
            // «·”ÿ— œÂ „„ﬂ‰ ÌﬂÊ‰ «Œ Ì«—Ì ·Ê ﬂ· «” Œœ«„ﬂ ··‹ Repositories »Ì „ ⁄‰ ÿ—Ìﬁ «·‹ UnitOfWork
            // ·ﬂ‰ „‘ ÂÌ”»» „‘ﬂ·… ·Ê „ÊÃÊœ.
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