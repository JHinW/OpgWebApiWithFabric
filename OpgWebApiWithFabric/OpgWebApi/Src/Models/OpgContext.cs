using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using OpgWebApi.Src.Entitys;
using OpgWebApi.Src.Entitys.Camera;
using OpgWebApi.Src.Entitys.Account;

namespace OpgWebApi.Src.Models
{
    public class OpgContext : DbContext
    {
        public OpgContext(DbContextOptions<OpgContext> options)
            : base(options)
        {
        }

        public DbSet<CameraEntity> CameraEntity { get; set; }

        public DbSet<PersonDetailsEntity> PersonDetails { get; set; }

        public DbSet<ImageEntity> ImageEntity { get; set; }

        public DbSet<ImageForNameEntity> ImageForNameEntity { get; set; }

        public DbSet<PersonGroupEntity> PersonGroupEntity { get; set; }

        public DbSet<CameraRegisterEntity> CameraRegisterEntity { get; set; }

        public DbSet<AccountRegisterEntity> AccountRegisterEntity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CameraEntity>().ToTable("Cameras");
            modelBuilder.Entity<PersonDetailsEntity>().ToTable("PersonDetails");
            modelBuilder.Entity<ImageEntity>().ToTable("Images");
            modelBuilder.Entity<ImageForNameEntity>().ToTable("ImageForNames");
            modelBuilder.Entity<PersonGroupEntity>().ToTable("PersonGroup");
            modelBuilder.Entity<CameraRegisterEntity>().ToTable("CameraRegister");
            modelBuilder.Entity<AccountRegisterEntity>().ToTable("AccountRegister");

        }
    }
}
