using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP.Core.Entites.OrderAggregate;
using GP.Core.Entities;

namespace GP.Repository.Data.Configuration
{
    public class AppointmentConfig : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasOne(U => U.User)
                .WithMany()
                .HasForeignKey(U => U.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(D => D.Doctor)
                .WithMany()
                .HasForeignKey(D => D.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
