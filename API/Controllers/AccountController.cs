using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
	public class AccountController : BaseApiController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;

		public AccountController(UserManager<AppUser> userManager,ITokenService tokenService,IMapper mapper)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_mapper = mapper;
		}

		[HttpPost("register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDTOs registerDTOs)
		{
			if (await userExists(registerDTOs.Username)) return BadRequest("Username is taken.");

			var user=_mapper.Map<AppUser>(registerDTOs);
			user.UserName = registerDTOs.Username.ToLower();
			
			var result = await _userManager.CreateAsync(user,registerDTOs.Password);

			if (!result.Succeeded) return BadRequest(result.Errors);

			var roleResult = await _userManager.AddToRoleAsync(user, "Member");

			if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);

			return new UserDto { 
				Username=user.UserName,
				Token= await _tokenService.CreateToken(user),
				KnownAs=user.KnownAs,
				Gender=user.Gender
			};
		}

		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDTO loginDTO)
		{
			var user = await _userManager.Users
				.Include(p => p.Photos)
				.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);
			if (user == null) return Unauthorized("Invalid username.");
			var result = await _userManager.CheckPasswordAsync(user,loginDTO.Password);

			if(!result) return Unauthorized("Invalid password");

			return new UserDto
			{
				Username = user.UserName,
				Token = await _tokenService.CreateToken(user),
				PhotoUrl=user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
				KnownAs=user.KnownAs,
				Gender=user.Gender
			};
		}

		public async Task<bool> userExists(string username)
		{
			return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
		}
	}
}
