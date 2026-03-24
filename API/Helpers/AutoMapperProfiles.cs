using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    // maps one object to another
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, ops =>
                    ops.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.IsMain).Url))
                .ForMember(dest => dest.Age, ops =>
                    ops.MapFrom(src => src.DateOfBirth.CalculateAge())); // to not let the mapper call the AppUser
            CreateMap<Photo, PhotoDto>();
        }
    }
}