﻿using Fiorella.Models;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Data
{
    public class FiorelloDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SliderEntity> SliderEntities { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        public FiorelloDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().Property(b => b.CreatedDate).HasDefaultValue(DateTime.Now);
            base.OnModelCreating(modelBuilder);
        }
    }
}
