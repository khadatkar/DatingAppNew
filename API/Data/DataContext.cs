using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class DataContext : IdentityDbContext<AppUser,AppRole,int,IdentityUserClaim<int>,AppUserRole,
		IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>
	{
		public DataContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Photo> photos { get; set; }
		public DbSet<UserLike> Likes { get; set; }
		public DbSet<Message> Messages { get; set; }

		public DbSet<Group> Groups { get; set; }
		public DbSet<Connection> connections { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<AppUser>()
				.HasMany(ur => ur.UserRoles)
				.WithOne(u => u.User)
				.HasForeignKey(ur => ur.UserId)
				.IsRequired();

			builder.Entity<AppRole>()
				.HasMany(ur => ur.UserRoles)
				.WithOne(u => u.Role)
				.HasForeignKey(ur => ur.RoleId)
				.IsRequired();


			builder.Entity<UserLike>()
				.HasKey(k => new { k.SourceUserId, k.LikedUserId });

			builder.Entity<UserLike>()
				.HasOne(s => s.SourceUser)
				.WithMany(l => l.LikedUsers)
				.HasForeignKey(l => l.SourceUserId)
				.OnDelete(DeleteBehavior.NoAction);


			builder.Entity<UserLike>()
				.HasOne(s => s.LikedUser)
				.WithMany(l => l.LikedByUsers)
				.HasForeignKey(l => l.LikedUserId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Entity<Message>()
				.HasOne(u => u.Sender)
				.WithMany(m => m.MessagesSent)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
