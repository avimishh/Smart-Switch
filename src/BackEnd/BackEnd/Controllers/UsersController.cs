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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BackEnd.Models.Auth;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SmartSwitchDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _currentUsername;

        public UsersController(UserManager<ApplicationUser> userManager, SmartSwitchDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _currentUsername = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        // PUT: api/Users/{username}/password
        [HttpPut("{username}/password")]
        public async Task<IActionResult> ChangePassword(string username, [FromBody] PasswordChangeRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _context.Users.FindAsync(username);
            if (user == null) return NotFound(Error.UserDoesNotExist);

            if (UserOwnershipValidator.IsNotValidated(_currentUsername, user)) return Unauthorized(Error.UnauthorizedUser);

            if (user.Password != request.OldPassword) return BadRequest(Error.IncorrectOldPassword);


            var appUser = await _userManager.FindByNameAsync(username);
            await _userManager.ChangePasswordAsync(appUser, request.OldPassword, request.NewPassword);
            user.Password = request.NewPassword;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(username))
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

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (UserExists(userDto.Username)) return BadRequest(Error.UserAlreadyExists);

            _context.Users.Add(_mapper.Map<User>(userDto));
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { username = userDto.Username }, userDto);
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Username == id);
        }
    }
}