using CorpCom_svc.DTOs.CorpCom.Reponse;
using CorpCom_svc.DTOs.CorpCom.Request;
using CorpCom_svc.Models;
using CorpCom_svc.Services.CorpCom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CorpCom_svc.Controllers
{
    [Route("api/corpcom")]
    [ApiController]
    public class CorpComController : ControllerBase
    {
        private readonly ICorpComServices _services;

        public CorpComController(ICorpComServices services)
        {
            _services = services;
        }

        [HttpGet("geturl")]
        public async Task<IActionResult> GetUrl()
        {
            string url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;
            return Ok(url);
        }

        [HttpPost("upsertbanner")]
        public async Task<IActionResult> UpsertBanner([FromForm] ImportBannerRequestDto input)
        {
            if (input.File != null)
            {
                return Ok(await _services.UpsertBanner(input));
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("upsertposition")]
        public async Task<ServiceResponse<string>> UpsertPosition(UpsertPositionRequestDto input) => await _services.UpsertPosition(input);

        [HttpGet("positions")]
        public async Task<ServiceResponse<List<PositionsResponseDto>>> Positions([FromQuery] PositionsRequestDto filter) => await _services.Positions(filter);

        [HttpGet("banners")]
        public async Task<ServiceResponse<List<BannersResponseDto>>> Banners([FromQuery] BannersRequestDto filter) => await _services.Banners(filter);
    }
}