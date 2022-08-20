using API.Extension;
using API.Filter;
using API.Models.Entity;
using API.Services;
using AutoMapper;
using CodeStudy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Models.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> GetTopUser(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                IEnumerable<SubmissionStatistic> data = await _cache.GetRecordAsync<IEnumerable<SubmissionStatistic>>("ardent-users");
                if (data == null)
                {
                    data = _statisticService.GetUserSubmit();
                    await _cache.SetRecordAsync("ardent-user", data, TimeSpan.FromMinutes(5));
                }
                return Ok(data);
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
            IEnumerable<Statistic> data = _statisticService.GetNewUser(start, end);
            return Ok(data);
        }

        [HttpGet("hot-problems")]
        public async Task<IActionResult> GetHotProblem(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                IEnumerable<ProblemStatistic> data = await _cache.GetRecordAsync<IEnumerable<ProblemStatistic>>("hot-problems");
                if (data == null)
                {
                    data = _statisticService.GetProblemSubmit();
                    await _cache.SetRecordAsync("hot-problems", data, TimeSpan.FromMinutes(15));
                }
                return Ok(data);
            }
            else
            {
                return Ok(_statisticService.GetProblemSubmitInRange((DateTime)startDate, (DateTime)endDate));
            }
        }
    }
}
