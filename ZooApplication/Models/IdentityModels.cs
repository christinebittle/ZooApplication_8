using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZooApplication.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FavoriteColor { get; set; }
        public ICollection<Booking> Bookings { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        //Add an animal entity to our system
        public DbSet<Animal> Animals { get; set; }

        public DbSet<Species> Species { get; set; }

        public DbSet<Keeper> Keepers { get; set; }

        public DbSet<Trivia> Trivias { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<BookingxTicket> BookingxTickets {get;set;}


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}