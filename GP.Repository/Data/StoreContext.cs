using GP.Core.Entities;
using GP.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.OrderAggregate;

namespace GP.Repository.Data
{
    public class StoreContext : IdentityDbContext<AppUser>
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Product> products { get; set; }
        public DbSet <XRayReport> xRayReports{ get; set; }
        public DbSet<Notification> notifications { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews{ get; set; }
        public DbSet<Appointment> Appointments{ get; set; }
        

        public DbSet<DeliveryMethod> DeliveryMethod { get; set; }
    }
}
