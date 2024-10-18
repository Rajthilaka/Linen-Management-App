using System.ComponentModel.Design;
using TDSSWebApplication.IServices;
using TDSSWebApplication.Services;

namespace TDSSWebApplication.IOC
{
    public static class ServiceModuleExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordServices, PasswordServices>();
           
            services.AddScoped<ITokenServices, TokenServices>();
            services.AddScoped<IEmployeeServices, EmployeeServices>();
            services.AddScoped<ICartlogServices, CartlogServices>();

        }

    }
}
