﻿using API.Extension;
using API.Filter;
using API.Models.Data;
using API.Models.Entity;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using Models.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("collection")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private readonly IStatisticService _statisticService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        public CollectionController(IStatisticService statisticService, IMapper mapper, IDistributedCache cache)
        {
            _statisticService = statisticService;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet("ardent-users")]
        public async Task<IActionResult> GetTopArdentUser(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                CacheData cache_data = await _cache.GetRecordAsync<CacheData>("ardent-users");
                if (cache_data == null)
                {
                    cache_data = new CacheData
                    {
                        RecordID = "ardent-users",
                        Data = _statisticService.GetUserSubmit(),
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddMinutes(5)
                    };
                    await _cache.SetRecordAsync("ardent-users", cache_data, TimeSpan.FromMinutes(5));
                }
                return Ok(cache_data);
            }
            else
            {
                return Ok(_statisticService.GetUserSubmitInRage((DateTime)startDate, (DateTime)endDate));
            }
        }

        [HttpGet("new-users")]
        public IActionResult GetNewUser(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate == null ? DateTime.Now : (DateTime)startDate;
            DateTime end = endDate == null ? DateTime.Now : (DateTime)endDate;
            IEnumerable<Statistic> data = _statisticService.GetNewUserInRange(start, end);
            return Ok(data);
        }

        [HttpGet("hot-problems")]
        public async Task<IActionResult> GetHotProblem(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                CacheData cache_data = await _cache.GetRecordAsync<CacheData>("hot-problems");
                if (cache_data == null)
                {
                    cache_data = new CacheData
                    {
                        RecordID = "hot-problems",
                        Data = _statisticService.GetSubmitOfProblem(),
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddMinutes(15)
                    };
                    await _cache.SetRecordAsync("hot-problems", cache_data, TimeSpan.FromMinutes(15));
                }
                return Ok(cache_data);
            }
            else
            {
                return Ok(_statisticService.GetSubmitOfProblemInRange((DateTime)startDate, (DateTime)endDate));
            }
        }

        [HttpGet("rank")]
        public async Task<IActionResult> GetUserRank(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                CacheData cache_data = await _cache.GetRecordAsync<CacheData>("rank");
                if (cache_data == null)
                {
                    cache_data = new CacheData
                    {
                        RecordID = "rank",
                        Data = _statisticService.GetUserRank(),
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddMinutes(15)
                    };
                    await _cache.SetRecordAsync("rank", cache_data);
                }
                return Ok(cache_data);
            }
            else
            {
                return Ok(_statisticService.GetUserRankInRange((DateTime)startDate, (DateTime)endDate));
            }
        }
    }
}
