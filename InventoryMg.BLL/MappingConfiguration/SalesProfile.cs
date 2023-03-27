using AutoMapper;
using InventoryMg.BLL.DTOs.Request;
using InventoryMg.BLL.DTOs.Response;
using InventoryMg.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMg.BLL.MappingConfiguration
{
    public class SalesProfile : Profile
    {
        public SalesProfile()
        {
            CreateMap<SalesRequestDto, Sale>();
            CreateMap<Sale, SalesResponseDto>();
        }
    }
}
