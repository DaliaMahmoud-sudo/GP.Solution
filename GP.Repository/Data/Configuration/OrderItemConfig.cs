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
    public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>

    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(OI => OI.Price).HasColumnType("decimal(18,2)");
            builder.OwnsOne(OI => OI.Product,  P=>P.WithOwner());

        }
    }
}
