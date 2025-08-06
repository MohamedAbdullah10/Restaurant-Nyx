using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data.Configuratinos
{
    public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            
            builder.HasOne(oi => oi.MenuItem)
                   .WithMany() 
                   .HasForeignKey(oi => oi.MenuItemId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure composite primary key if needed (not typically for simple PKs derived from ModelBase)
            // Example for Subtotal calculation constraint if desired:
            // builder.HasComputedColumnSql("Quantity * UnitPrice", stored: true); // SQL Server specific, calculates in DB
        }
    }
}