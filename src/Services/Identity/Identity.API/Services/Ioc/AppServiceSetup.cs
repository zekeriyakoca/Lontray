using IdentityServer4.Services;
using Lontray.Services.Identity.API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lontray.Services.Identity.API.Services.Ioc
{
    public static class AppServiceSetup
    {
        public static void Set(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
            services.AddTransient<IRedirectService, RedirectService>();
            services.AddTransient<IProfileService, ProfileService>();
        }
    }
}
