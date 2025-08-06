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
    public class DailyMenuItemActivityConfigurations : IEntityTypeConfiguration<DailyMenuItemActivity>
    {
        public void Configure(EntityTypeBuilder<DailyMenuItemActivity> builder)
        {
        
            builder.HasOne(dma => dma.MenuItem)
                   .WithMany() 
                   .HasForeignKey(dma => dma.MenuItemId)
                   .OnDelete(DeleteBehavior.Cascade); 

            // جعل التركيبة (MenuItemId, ActivityDate) فريدة لمنع تكرار النشاط لنفس الصنف في نفس اليوم
            builder.HasIndex(dma => new { dma.MenuItemId, dma.ActivityDate }).IsUnique();
        }
    }
}