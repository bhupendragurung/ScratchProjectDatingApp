﻿using AutoMapper;
using ScratchProjectDatingApp.DTOs;
using ScratchProjectDatingApp.Entity;
using ScratchProjectDatingApp.Extensions;

namespace ScratchProjectDatingApp.Helper
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>().
                ForMember(dest => dest.PhotoUrl,opt => opt.MapFrom(src =>src.Photos.FirstOrDefault(x =>x.IsMain).Url))
                .ForMember(dest =>dest.Age,opt =>opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Message, MessageDto>()
                .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<DateTime,DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d,DateTimeKind.Utc));
            CreateMap<DateTime?,DateTime?>().ConvertUsing(d => d.HasValue? DateTime.SpecifyKind(d.Value,DateTimeKind.Utc):null);
        }
        
    }
}
