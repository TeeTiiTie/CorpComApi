using CorpComApi.DTOs.CorpCom.Reponse;
using CorpComApi.DTOs.CorpCom.Request;
using CorpComApi.Models;

namespace CorpComApi.Services.CorpCom
{
    public interface ICorpComServices
    {
        Task<ServiceResponse<List<BannersResponseDto>>> Banners(BannersRequestDto filter);

        Task<ServiceResponse<List<PositionsResponseDto>>> Positions(PositionsRequestDto filter);

        Task<ServiceResponse<SaveFileResponseDto>> UpsertBanner(ImportBannerRequestDto input);

        Task<ServiceResponse<string>> UpsertPosition(UpsertPositionRequestDto input);
    }
}