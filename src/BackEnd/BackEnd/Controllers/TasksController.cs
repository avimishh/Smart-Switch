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
using BackEnd.Models.Auth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly SmartSwitchDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _currentUsername;

        public TasksController(SmartSwitchDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _currentUsername = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        // GET: api/Tasks/plug/DC:DD:C2:23:D6:60
        [HttpGet("plug/{mac}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(string mac)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Plug plug = await _context.Plugs.Include(p => p.Tasks).SingleOrDefaultAsync(p => p.Mac == mac);
            if (plug == null) return NotFound(Error.PlugDoesNotExist);

            if (UserOwnershipValidator.IsNotValidated(_currentUsername, plug, _context)) return Unauthorized(Error.UnauthorizedOwner);

            return Ok(_mapper.Map<List<TaskDto>>(plug.Tasks));
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<IActionResult> PostTask([FromBody]TaskDto taskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Plug plug = await _context.Plugs.FindAsync(taskDto.DeviceMac);
            if (plug == null) return NotFound(Error.PlugDoesNotExist);

            if (UserOwnershipValidator.IsNotValidated(_currentUsername, plug, _context)) return Unauthorized(Error.UnauthorizedOwner);

            Models.Task task = _mapper.Map<Models.Task>(taskDto);
            plug.AddTask(task, _context);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTasks", new { mac = task.DeviceMac }, task);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            if (UserOwnershipValidator.IsNotValidated(_currentUsername, task, _context)) return Unauthorized(Error.UnauthorizedOwner);

            _context.Tasks.Remove(task);
            task.Delete(); // cancel actual Hangfire job
            await _context.SaveChangesAsync();

            return Ok(task);
        }
    }
}