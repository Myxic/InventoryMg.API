using AutoMapper;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.MappingConfiguration
{
    public class UserProfileProfile : Profile
    {
        public UserProfileProfile() {

            CreateMap<UserRegistration, UserProfile>()
                .ForMember(dest => dest.FullName,
                opts => opts.MapFrom(src => src.FirstName + " " + src.LastName));
        
        }
    }
}
