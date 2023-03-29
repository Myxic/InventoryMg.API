using InventoryMg.DAL.Entities;
using InventoryMg.BLL.Implementation;
using InventoryMg.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryMg.DAL;
using InventoryMg.DAL.Repository;

namespace InventoryMg.BLL.Extensions
{
    public static class MiddlewareExtention
    {
        public static void RegisterServices(this IServiceCollection services)
        {
          
           services.AddTransient<IAuthenticationService, AuthenticationService>();
          // services.AddTransient<IJwtService, JwtService>();
           services.AddTransient<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ISalesServices, SalesServices>();
            services.AddTransient<IRoleService, RoleService>(); 
           
        }
    }
}
