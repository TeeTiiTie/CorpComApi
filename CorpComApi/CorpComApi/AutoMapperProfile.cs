using AutoMapper;
using CorpCom.Models.CorpCom;
using CorpComApi.DTOs.CorpCom.Reponse;
using CorpComApi.DTOs.CorpCom.Request;

namespace CorpComApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            /*
             * CreateMap<SampleMessage, ExampleModels>()
             *     .ForMember(_ => _.ExampleName, _ => _.MapFrom(_ => _.Name))
             *     .ReverseMap();
             *
             * CreateMap<ExampleModels, GetExampleReponseDto>();
             */

            // CorpCom
            CreateMap<ImportBannerRequestDto, Banner>()
                .AfterMap((src, des) => des.IsActive = true);
            CreateMap<UpsertPositionRequestDto, Position>()
                .AfterMap((src, des) => des.IsActive = true);

            CreateMap<Position, PositionsResponseDto>();
            CreateMap<Banner, BannersResponseDto>();
        }
    }
}