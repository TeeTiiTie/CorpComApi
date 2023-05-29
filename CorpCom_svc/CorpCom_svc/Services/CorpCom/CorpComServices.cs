using AutoMapper;
using CorpCom.Data;
using CorpCom.Models.CorpCom;
using CorpCom_svc.Configurations;
using CorpCom_svc.DTOs.CorpCom.Reponse;
using CorpCom_svc.DTOs.CorpCom.Request;
using CorpCom_svc.Helpers;
using CorpCom_svc.Models;
using CorpCom_svc.Services.Auth;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace CorpCom_svc.Services.CorpCom
{
    public class CorpComServices : ICorpComServices
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;

        //private readonly ILoginDetailServices _login;
        private readonly IOptions<PathBannerSetting> _optionsPathBanner;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorpComServices(AppDBContext context, IMapper mapper, IOptions<PathBannerSetting> optionsPathBanner, IHttpContextAccessor httpContextAccessor) //ILoginDetailServices login
        {
            _context = context;
            _mapper = mapper;
            //_login = login;
            _optionsPathBanner = optionsPathBanner;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<SaveFileResponseDto>> UpsertBanner(ImportBannerRequestDto input)
        {
            try
            {
                DateTime now = DateTime.Now;
                //var claim = _login.GetClaim();

                string folder = _optionsPathBanner.Value.StaticPath;
                var extension = Path.GetExtension(input.File.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var currentUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_optionsPathBanner.Value.RequestPath}/{fileName}";

                var resp = new SaveFileResponseDto
                {
                    PhysicalPath = folder,
                    ImageName = fileName,
                    ImageUrl = currentUrl,
                };

                var update = await _context.Banners.FirstOrDefaultAsync(x => x.BannerId == input.BannerId && x.IsActive == true);

                if (update != null)
                {
                    update.PhysicalPath = folder;
                    update.ImageName = fileName;
                    update.ImageUrl = currentUrl;
                    update.LinkUrl = input.LinkUrl;
                    update.Priority = input.Priority;
                    update.PositionId = input.PositionId;
                    update.IsPublish = input.IsPublish;
                    update.Remark = input.Remark;
                    update.UpdatedDate = now;
                    update.UpdatedByUserId = 1; //claim.UserId;
                    _context.Update(update);

                    resp.Method = "Update";
                }
                else
                {
                    var insert = _mapper.Map<Banner>(input);
                    insert.PhysicalPath = folder;
                    insert.ImageName = fileName;
                    insert.ImageUrl = currentUrl;
                    insert.CreatedByUserId = 1; //claim.UserId;
                    insert.CreatedDate = now;
                    insert.UpdatedByUserId = 1; //claim.UserId;
                    insert.UpdatedDate = now;
                    _context.Add(insert);

                    resp.Method = "Insert";
                }

                await SaveFile(input.File, fileName);

                await _context.SaveChangesAsync();

                return ResponseResult.Success(resp);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[UpsertBanner] - An error occurred.");
                return ResponseResult.Failure<SaveFileResponseDto>(ex.Message);
            }
        }

        public async Task SaveFile(IFormFile file, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var fileSize = file.Length;
                string folder = _optionsPathBanner.Value.StaticPath;

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string savingPath = Path.Combine(folder, fileName);
                await File.WriteAllBytesAsync(savingPath, content);
            }
        }

        public async Task<ServiceResponse<string>> UpsertPosition(UpsertPositionRequestDto input)
        {
            try
            {
                DateTime now = DateTime.Now;
                //var claim = _login.GetClaim();

                var qry = _context.Positions.Where(x => x.IsActive == true);

                if (!string.IsNullOrWhiteSpace(input.PositionName))
                {
                    var checkName = qry.FirstOrDefault(x => x.PositionName == input.PositionName);

                    if (checkName != null)
                        return ResponseResult.Failure<string>("Position name is duplicate.");
                }

                var select = await qry.FirstOrDefaultAsync(x => x.PositionId == input.PositionId);

                if (select != null)
                {
                    select.PositionName = input.PositionName;
                    select.Description = input.Description;
                    select.Width = input.Width;
                    select.Height = input.Height;
                    select.UpdatedDate = now;
                    select.UpdatedByUserId = 1; // claim.UserId;
                    _context.Update(select);
                }
                else
                {
                    var insert = _mapper.Map<Position>(input);
                    insert.CreatedDate = now;
                    insert.CreatedByUserId = 1; // claim.UserId;
                    insert.UpdatedDate = now;
                    insert.UpdatedByUserId = 1; //claim.UserId;
                    _context.Add(insert);
                }

                await _context.SaveChangesAsync();
                return ResponseResult.Success(input.PositionName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[UpsertPosition] - An error occurred.");
                return ResponseResult.Failure<string>(ex.Message);
            }
        }

        public async Task<ServiceResponse<List<PositionsResponseDto>>> Positions(PositionsRequestDto filter)
        {
            try
            {
                var qry = _context.Positions.Where(x => x.IsActive == true);
                if (filter.PositionId != null)
                    qry.Where(x => x.PositionId == filter.PositionId);

                if (!string.IsNullOrWhiteSpace(filter.PositionName))
                    qry.Where(x => x.PositionName.Contains(filter.PositionName));

                // Ordering
                if (!string.IsNullOrWhiteSpace(filter.OrderingField))
                {
                    try
                    {
                        qry = qry.OrderBy($"{filter.OrderingField} {(filter.AscendingOrder ? "ascending" : "descending")}");
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "[GetDebtDetails] - An error occurred.");
                        return ResponseResult.Failure<List<PositionsResponseDto>>($"Could not order by field: {filter.OrderingField}");
                    }
                }
                //Pagination
                var paginationResult = await _httpContextAccessor.HttpContext.InsertPaginationParametersInResponse(qry, filter.RecordsPerPage, filter.Page);
                var output = await qry.Paginate(filter).ToListAsync();

                var dto = _mapper.Map<List<PositionsResponseDto>>(output);
                return ResponseResult.Success(dto, paginationResult);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[Positions] - An error occurred.");
                return ResponseResult.Failure<List<PositionsResponseDto>>(ex.Message);
            }
        }

        public async Task<ServiceResponse<List<BannersResponseDto>>> Banners(BannersRequestDto filter)
        {
            try
            {
                var qry = _context.Banners.Where(x => x.IsActive == true && x.PositionId == filter.PositionId);

                // Ordering
                if (!string.IsNullOrWhiteSpace(filter.OrderingField))
                {
                    try
                    {
                        qry = qry.OrderBy($"{filter.OrderingField} {(filter.AscendingOrder ? "ascending" : "descending")}");
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "[GetDebtDetails] - An error occurred.");
                        return ResponseResult.Failure<List<BannersResponseDto>>($"Could not order by field: {filter.OrderingField}");
                    }
                }
                //Pagination
                var paginationResult = await _httpContextAccessor.HttpContext.InsertPaginationParametersInResponse(qry, filter.RecordsPerPage, filter.Page);
                var output = await qry.Paginate(filter).ToListAsync();

                var dto = _mapper.Map<List<BannersResponseDto>>(output);
                return ResponseResult.Success(dto, paginationResult);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[Banners] - An error occurred.");
                return ResponseResult.Failure<List<BannersResponseDto>>(ex.Message);
            }
        }
    }
}