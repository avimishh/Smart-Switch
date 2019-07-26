using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models;
using AutoMapper;
using BackEnd.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Models.Auth;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlugsController : ControllerBase
    {
        private readonly SmartSwitchDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _currentUsername;

        public PlugsController(SmartSwitchDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _currentUsername = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        // GET: api/Plugs/5
        [HttpGet("{mac}")]
        public async Task<ActionResult<PlugDto>> GetPlug([FromRoute] string mac)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var plug = await _context.Plugs.FindAsync(mac);
            if (plug == null) return NotFound(Error.PlugDoesNotExist);

            if (UserOwnershipValidator.IsNotValidated(_currentUsername, plug, _context)) return Unauthorized(Error.UnauthorizedOwner);

            return Ok(_mapper.Map<PlugDto>(plug));
        }

        // GET: api/Plugs/User/{user.id}
        [HttpGet("user/{username}")]
        public async Task<ActionResult<IEnumerable<PlugDto>>> GetUserPlugs([FromRoute] string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _context.Users.Include(u => u.Plugs).SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound(Error.UserDoesNotExist);

            if (UserOwnershipValidator.IsNotValidated(_currentUsername, user)) return Unauthorized(Error.UnauthorizedOwner);

            return Ok(_mapper.Map<List<PlugDto>>(user.Plugs.Where(p => p.IsDeleted == false)));
        }

        // PUT: api/Plugs
        [HttpPut]
        public async Task<IActionResult> PutPlug([FromBody] PlugDtoIn plugDtoIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Plug plug = await _context.Plugs.FindAsync(plugDtoIn.Mac);
            if (plug == null) return NotFound(Error.PlugDoesNotExist);

            if (UserOwnershipValidator.IsNotValidated(_currentUsername, plug, _context)) return Unauthorized(Error.UnauthorizedOwner);

            _mapper.Map(plugDtoIn, plug);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlugExists(plugDtoIn.Mac))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("{mac}")]
        public async Task<IActionResult> TurnApproveOrDenyPlug(string mac, [FromQuery]Models.Task.Operations? op, [FromQuery]bool? approved)
        {
            Plug plug = await _context.Plugs.FindAsync(mac);
            if (plug == null) return NotFound(Error.PlugDoesNotExist);

            if (UserOwnershipValidator.IsNotValidated(_currentUsername, plug, _context)) return Unauthorized(Error.UnauthorizedOwner);

            if (approved != null)
            {
                plug.Approved = (bool)approved;
                if (!plug.Approved) plug.IsDeleted = true;
            }

            await _context.SaveChangesAsync();

            if (op != null)
            {
                try
                {
                    Models.Task.Execute((Models.Task.Operations)op, plug, _context);
                }
                catch (PlugNotConnectedException)
                {
                    return BadRequest(Error.PlugNotConnected);
                }
            }
            
            return NoContent();
        }

        private bool PlugExists(string id)
        {
            return _context.Plugs.Any(e => e.Mac == id);
        }
    }
}