using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models;
using BackEnd.Models.Dto;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Models.Auth;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PowerUsageSamplesController : ControllerBase
    {
        private readonly SmartSwitchDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _currentUsername;

        public PowerUsageSamplesController(SmartSwitchDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _currentUsername = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        // POST: api/PowerUsageSamples/plug
        [HttpPost("plug")]
        public async Task<ActionResult<IEnumerable<PowerUsageSampleDto>>> GetPowerUsageSamples([FromBody] DateRangeDto dateRange)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string mac = dateRange.Mac;

            Plug plug = await _context.Plugs.Include(p => p.Samples).SingleOrDefaultAsync(p => p.Mac == mac);
            if (plug == null) return NotFound(Error.PlugDoesNotExist);

            if (UserOwnershipValidator.IsNotValidated(_currentUsername, plug, _context)) return Unauthorized(Error.UnauthorizedOwner);

            return Ok(_mapper.Map<List<PowerUsageSampleDto>>(plug.Samples
                .Where(s => s.SampleDate >= dateRange.EarlierDate && s.SampleDate <= dateRange.LaterDate)
                .OrderBy(s => s.SampleDate)));
        }
    }
}