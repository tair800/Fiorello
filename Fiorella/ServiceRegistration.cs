using Fiorella.Data;
using Fiorella.Services;
using Fiorella.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fiorella
{
    public static class ServiceRegistration
    {
        public static void Register(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllersWithViews();
            services.AddDbContext<FiorelloDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });
            //services.AddScoped<ISumService, SumService>();
            //services.AddTransient<ISumService, SumService>();
            //services.AddSingleton<ISumService, SumService>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(15);
            });
            services.AddHttpContextAccessor();
            services.AddScoped<IBasketService, BasketService>();
        }
    }
}
