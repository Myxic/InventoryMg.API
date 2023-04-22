using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.DAL.Entities
{
    public class AppRole : IdentityRole
    {
        public AppRole(): base()
        {

        }
        public AppRole(string rolename): base(rolename)
        {

        }

    }
}
