﻿using AutoMapper;
using CorpCom.Data;
using CorpCom.Models.CorpCom;
using CorpComApi.Configurations;
using CorpComApi.DTOs.CorpCom.Reponse;
using CorpComApi.DTOs.CorpCom.Request;
using CorpComApi.Helpers;
using CorpComApi.Models;
using CorpComApi.Services.Auth;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace CorpComApi.Services.CorpCom
{
    public class CorpComServices : ICorpComServices
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;

        private readonly ILoginDetailServices _login;
        private readonly IOptions<PathBannerSetting> _optionsPathBanner;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorpComServices(AppDBContext context, IMapper mapper, IOptions<PathBannerSetting> optionsPathBanner, IHttpContextAccessor httpContextAccessor, ILoginDetailServices login)
        {
            _context = context;
            _mapper = mapper;
            _login = login;
            _optionsPathBanner = optionsPathBanner;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<SaveFileResponseDto>> UpsertBanner(ImportBannerRequestDto input)
        {
            try
            {
                // check file name extension
                var extension = Path.GetExtension(input.File.FileName);
                if (extension != ".jpg" && extension != ".png")
                    return ResponseResult.Failure<SaveFileResponseDto>("Invalid file name extension.");

                DateTime now = DateTime.Now;
                var claim = _login.GetClaim();

                // value path from app setting
                string folder = _optionsPathBanner.Value.StaticPath;
                var fileName = $"{Guid.NewGuid()}{extension}";

                // url of image
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
                    update.LinkUrl = input.LinkUrl; // link url ที่ต้องการกดแล้วให้ไปที่ไหน
                    update.Priority = input.Priority;
                    update.PositionId = input.PositionId;
                    update.IsPublish = input.IsPublish;
                    update.Remark = input.Remark;
                    update.UpdatedDate = now;
                    update.UpdatedByUserId = claim.UserId;
                    _context.Update(update);

                    resp.Method = "Update";
                }
                else
                {
                    var insert = _mapper.Map<Banner>(input);
                    insert.PhysicalPath = folder;
                    insert.ImageName = fileName;
                    insert.ImageUrl = currentUrl;
                    insert.CreatedByUserId = claim.UserId;
                    insert.CreatedDate = now;
                    insert.UpdatedByUserId = claim.UserId;
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
                var claim = _login.GetClaim();

                var qry = _context.Positions.Where(x => x.IsActive == true);

                if (qry.Any(x => x.PositionName == input.PositionName)) // check position name duplicate
                {
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
                    select.UpdatedByUserId = claim.UserId;
                    _context.Update(select);
                }
                else
                {
                    var insert = _mapper.Map<Position>(input);
                    insert.CreatedDate = now;
                    insert.CreatedByUserId = claim.UserId;
                    insert.UpdatedDate = now;
                    insert.UpdatedByUserId = claim.UserId;
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