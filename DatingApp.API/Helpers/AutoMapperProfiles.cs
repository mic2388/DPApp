using System.Linq;
using AutoMapper;
using DatingApp.API.DTO;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserForDetailedDTO>()
            .ForMember(dest => dest.PhotoUrl , opt => {
                opt.MapFrom(src=> src.Photos.FirstOrDefault(t=>t.IsMain).Url);
            })
            .ForMember(dest=> dest.Age,opt=>{

                opt.ResolveUsing(t=>t.DateOfBirth.CalculateAge());
            });
            CreateMap<User,UserForListDTO>()
            .ForMember(dest => dest.PhotoUrl , opt => {
                opt.MapFrom(src=> src.Photos.FirstOrDefault(t=>t.IsMain).Url);
            }) .ForMember(dest=> dest.Age,opt=>{

                opt.ResolveUsing(t=>t.DateOfBirth.CalculateAge());
            });
            CreateMap<Photo,PhotosForDetailDTO>();

        }
    }
}