using Fiorella.Data;
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

        }
    }
}
