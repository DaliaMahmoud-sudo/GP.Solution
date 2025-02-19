using GP.Repository.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;



using GP.Core.Entities.Identity;

namespace GP.APIs.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection Services , IConfiguration configuration)
        {
          
            Services.AddIdentity<AppUser, IdentityRole>()
                       .AddEntityFrameworkStores<StoreContext>();
            Services.AddAuthentication();
                

                
                
            return Services;

        }
    }
    }
