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

                opt.MapFrom(t=>t.DateOfBirth.CalculateAge());
            });
            CreateMap<User,UserForListDTO>()
            .ForMember(dest => dest.PhotoUrl , opt => {
                opt.MapFrom(src=> src.Photos.FirstOrDefault(t=>t.IsMain).Url);
            }) .ForMember(dest=> dest.Age,opt=>{

                opt.MapFrom(t=>t.DateOfBirth.CalculateAge());
            });
            CreateMap<Photo,PhotosForDetailDTO>();
            CreateMap<UserForUpdatesDTO,User>();
            CreateMap<Photo,PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto,Photo>();
            CreateMap<UserForRegisterDTO,User>();
            CreateMap<MessageForCreationDTO,Message>().ReverseMap();
            CreateMap<Message,MessageToReturnDto>()
                    .ForMember(m=>m.SenderPhotoUrl , 
                    opt=> opt.MapFrom(m=>m.Sender.Photos.FirstOrDefault(t=>t.IsMain).Url))
                    .ForMember(m=>m.RecipientPhotoUrl,
                     opt => opt.MapFrom(m=>m.Recipient.Photos.FirstOrDefault(t=>t.IsMain).Url));

        }
    }
}