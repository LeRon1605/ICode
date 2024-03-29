﻿using API.Extension;
using API.Filter;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using Models.Statistic;
using Services.Interfaces;
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
        private readonly IDistributedCache _cache;
        public CollectionController(IStatisticService statisticService, IDistributedCache cache)
        {
            _statisticService = statisticService;
            _cache = cache;
        }

        [HttpGet("top-activity")]
        public async Task<IActionResult> GetTopActivityUser(DateTime? startDate, DateTime? endDate, bool? gender, string name = "")
        {
            if (startDate == null || endDate == null)
            {
                // Get top activity user all the time
                CacheData cache_data = await _cache.GetRecordAsync<CacheData>($"top-activity-{gender}-{name}");
                if (cache_data == null)
                {
                    cache_data = new CacheData
                    {
                        RecordID = $"top-activity-{gender}-{name}",
                        Data = _statisticService.GetUserSubmit(gender, name),
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddMinutes(15)
                    };
                    await _cache.SetRecordAsync(cache_data.RecordID, cache_data, TimeSpan.FromMinutes(15));
                }
                return Ok(cache_data);
            }
            else
            {
                // Get top activity user in range
                CacheData cache_data = await _cache.GetRecordAsync<CacheData>($"top-activity-{startDate.Value.Date}-{endDate.Value.Date}-{gender}-{name}");
                if (cache_data == null)
                {
                    cache_data = new CacheData
                    {
                        RecordID = $"top-activity-{startDate.Value.Date}-{endDate.Value.Date}-{gender}-{name}",
                        Data = _statisticService.GetUserSubmitInRage((DateTime)startDate, (DateTime)endDate, name, gender),
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddMinutes(5)
                    };
                    await _cache.SetRecordAsync(cache_data.RecordID, cache_data, TimeSpan.FromMinutes(5));
                }
                return Ok(cache_data);
            }
        }

        [HttpGet("new-users")]
        public async Task<IActionResult> GetNewUser(DateTime? startDate, DateTime? endDate, bool? gender, string name = "")
        {
            DateTime start = startDate == null ? DateTime.Now : (DateTime)startDate;
            DateTime end = endDate == null ? DateTime.Now : (DateTime)endDate;
            CacheData cacheData = await _cache.GetRecordAsync<CacheData>($"new-users-{start.Date}-{end.Date}-{name}-{gender}");
            if (cacheData == null)
            {
                cacheData = new CacheData
                {
                    RecordID = $"new-users-{start.Date}-{end.Date}-{name}-{gender}",
                    Data = _statisticService.GetNewUserInRange(start, end, name, gender),
                    CacheAt = DateTime.Now,
                    ExpireAt = DateTime.Now.AddMinutes(5)
                };
                await _cache.SetRecordAsync(cacheData.RecordID, cacheData, TimeSpan.FromMinutes(5));
            }
            return Ok(cacheData);
        }

        [HttpGet("hot-problems")]
        public async Task<IActionResult> GetHotProblem(DateTime? startDate, DateTime? endDate, string name = "", string author = "", string tag = "")
        {
            if (startDate == null || endDate == null)
            {
                // Get hot problems for all the time
                CacheData cache_data = await _cache.GetRecordAsync<CacheData>($"hot-problems-{name}-{author}-{tag}");
                if (cache_data == null)
                {
                    cache_data = new CacheData
                    {
                        RecordID = $"hot-problems-{name}-{author}-{tag}",
                        Data = _statisticService.GetSubmitOfProblem(name, author, tag),
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddMinutes(5)
                    };
                    await _cache.SetRecordAsync(cache_data.RecordID, cache_data, TimeSpan.FromMinutes(5));
                }
                return Ok(cache_data);
            }
            else
            {
                // Get hot problems in range
                CacheData cache_data = await _cache.GetRecordAsync<CacheData>($"hot-problems-{startDate.Value}-{endDate.Value}-{name}-{author}-{tag}");
                if (cache_data == null)
                {
                    cache_data = new CacheData
                    {
                        RecordID = $"hot-problems-{startDate.Value.Date}-{endDate.Value.Date}-{name}-{author}-{tag}",
                        Data = _statisticService.GetSubmitOfProblemInRange((DateTime)startDate, (DateTime)endDate, name, author, tag),
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddMinutes(15)
                    };
                    await _cache.SetRecordAsync(cache_data.RecordID, cache_data, TimeSpan.FromMinutes(1));
                }
                return Ok(cache_data);
            }
        }

        [HttpGet("rank")]
        public async Task<IActionResult> GetUserRank(DateTime? startDate, DateTime? endDate, bool? gender, string name = "")
        {
            if (startDate == null || endDate == null)
            {
                // Get user rank for all the time
                CacheData cache_data = await _cache.GetRecordAsync<CacheData>($"rank-{name}-{gender}");
                if (cache_data == null)
                {
                    cache_data = new CacheData
                    {
                        RecordID = $"rank-{name}-{gender}",
                        Data = _statisticService.GetUserRank(name, gender),
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddMinutes(5)
                    };
                    await _cache.SetRecordAsync(cache_data.RecordID, cache_data, TimeSpan.FromMinutes(5));
                }
                return Ok(cache_data);
            }
            else
            {
                // Get user rank in range
                CacheData cache_data = await _cache.GetRecordAsync<CacheData>($"rank-{startDate.Value}-{endDate.Value}-{name}-{gender}");
                if (cache_data == null)
                {
                    cache_data = new CacheData
                    {
                        RecordID = $"rank-{startDate.Value}-{endDate.Value}-{name}-{gender}",
                        Data = _statisticService.GetUserRankInRange((DateTime)startDate, (DateTime)endDate, name, gender),
                        CacheAt = DateTime.Now,
                        ExpireAt = DateTime.Now.AddMinutes(15)
                    };
                    await _cache.SetRecordAsync(cache_data.RecordID, cache_data, TimeSpan.FromMinutes(1));
                }
                return Ok(cache_data);
            }
        }

        [HttpGet("new-problems")]
        public async Task<IActionResult> GetNewProblem(DateTime? startDate, DateTime? endDate, string name = "", string author = "", string tag = "")
        {
            DateTime start = startDate == null ? DateTime.Now.Date : (DateTime)startDate;
            DateTime end = endDate == null ? DateTime.Now.Date : (DateTime)endDate;
            CacheData data = await _cache.GetRecordAsync<CacheData>($"new-problems-{start.Date}-{end.Date}-{name}-{author}-{tag}");
            if (data == null)
            {
                data = new CacheData
                {
                    RecordID = $"new-problems-{start.Date}-{end.Date}-{name}-{author}-{tag}",
                    Data = _statisticService.GetNewProblemInRange(start, end, name, author, tag),
                    CacheAt = DateTime.Now,
                    ExpireAt = DateTime.Now.AddMinutes(1)
                };
                await _cache.SetRecordAsync(data.RecordID, data, TimeSpan.FromMinutes(1));
            }
            return Ok(data);
        }
    }
}
