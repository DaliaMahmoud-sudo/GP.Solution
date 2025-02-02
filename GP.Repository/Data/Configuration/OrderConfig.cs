using GP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.OrderAggregate;

namespace Talabat.Repository.Data.Configuration
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(O=>O.Status)
                .HasConversion(OStatus=> OStatus.ToString(), OStatus=>(Status)Enum.Parse(typeof(Status), OStatus));
            builder.Property(O => O.SubTotal)
                    .HasColumnType("decimal(18,2)");
            builder.OwnsOne(O => O.ShippingAddress, X => X.WithOwner());
            builder.HasOne(O => O.DeliveryMethod)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
