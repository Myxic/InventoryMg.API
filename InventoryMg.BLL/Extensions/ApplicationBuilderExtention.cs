using InventoryMg.DAL.Configurations;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.Extensions
{
    public static class ApplicationBuilderExtention
    {
        public static IApplicationBuilder AddGlobalErrorHandler(this IApplicationBuilder applicationBuilder) 
            => applicationBuilder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
