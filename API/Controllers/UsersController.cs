using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly DataContext _context;

		public UsersController(DataContext context)
		{
			_context = context;
		}
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
		{
			return await _context.Users.ToListAsync();
		}

		[Authorize]
		[HttpGet("{id}")]
		public ActionResult<AppUser> GetUsers(int id)
		{
			var user = _context.Users.Find(id);
			return user;
		}
	}
}
