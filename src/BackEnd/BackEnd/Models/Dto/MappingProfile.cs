using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Plug, PlugDto>();
            CreateMap<PlugDto, Plug>();
            CreateMap<PlugDtoIn, Plug>();
            CreateMap<PowerUsageSample, PowerUsageSampleDto>();
            CreateMap<Task, TaskDto>();
            CreateMap<TaskDto, Task>();
        }
    }
}
