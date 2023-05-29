using CorpCom_svc.DTOs.CorpCom.Reponse;
using CorpCom_svc.DTOs.CorpCom.Request;
using CorpCom_svc.Models;

namespace CorpCom_svc.Services.CorpCom
{
    public interface ICorpComServices
    {
        Task<ServiceResponse<List<BannersResponseDto>>> Banners(BannersRequestDto filter);

        Task<ServiceResponse<List<PositionsResponseDto>>> Positions(PositionsRequestDto filter);

        Task<ServiceResponse<SaveFileResponseDto>> UpsertBanner(ImportBannerRequestDto input);

        Task<ServiceResponse<string>> UpsertPosition(UpsertPositionRequestDto input);
    }
}