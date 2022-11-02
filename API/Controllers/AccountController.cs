using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
	public class AccountController : BaseApiController
	{
		private readonly DataContext _context;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;

		public AccountController(DataContext context,ITokenService tokenService,IMapper mapper)
		{
			_context = context;
			_tokenService = tokenService;
			_mapper = mapper;
		}

		[HttpPost("register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDTOs registerDTOs)
		{
			if (await userExists(registerDTOs.Username)) return BadRequest("Username is taken.");

			var user=_mapper.Map<AppUser>(registerDTOs);

			using var hmac = new HMACSHA512();


			user.UserName = registerDTOs.Username.ToLower();
			user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTOs.Password));
			user.PasswordSalt = hmac.Key;
			

			_context.Users.Add(user);

			await _context.SaveChangesAsync();

			return new UserDto { 
				Username=user.UserName,
				Token=_tokenService.CreateToken(user),
				KnownAs=user.KnownAs
			};
		}

		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDTO loginDTO)
		{
			var user = await _context.Users
				.Include(p => p.Photos)
				.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);
			if (user == null) return Unauthorized("Invalid username.");

			using var hmac = new HMACSHA512(user.PasswordSalt);

			var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

			for (int i = 0; i < computedhash.Length; i++)
			{
				if (computedhash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password.");
			}

			return new UserDto
			{
				Username = user.UserName,
				Token = _tokenService.CreateToken(user),
				PhotoUrl=user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
				KnownAs=user.KnownAs
			};
		}

		public async Task<bool> userExists(string username)
		{
			return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
		}
	}
}
