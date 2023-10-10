using AutoMapper;
using Docs.Api.Models;
using Docs.Api.Models.Dtos;

namespace Docs.Api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Document, DocumentDto>().ReverseMap();
        }
    }
}
