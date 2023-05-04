using System.Text.Json;
using Atletika_Denik_API.Data.ViewModels;
using Atletika_Denik_API.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Atletika_Denik_API.Data.Models
{
    public class ActivitiesContext : DbContext
    {
        Errors error = new Errors();

        public DbSet<ViewModels.Tag> Tag { get; set; }
        public DbSet<ViewModels.Tag_Association> Tag_Association { get; set; }

        public DbSet<ViewModels.Tag_Details> Tag_Details { get; set; }

        public DbSet<ViewModels.Tag_User_Settings> Tag_User_Settings { get; set; }

        public DbSet<ViewModels.Tag_User_Response> Tag_User_Response { get; set; }

        public DbSet<Tag_Activities_Definitions> Tag_Activities_Definitions { get; set; }
        public DbSet<Tag_Activities_Association> Tag_Activities_Association { get; set; }
        public DbSet<Tag_Activities_User_Responses> Tag_Activities_User_Responses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString3"]);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViewModels.Tag>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Tag>().Property(p => p.id).ValueGeneratedNever();

            modelBuilder.Entity<ViewModels.Tag_Association>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Tag_Association>().Property(p => p.id).ValueGeneratedNever();

            modelBuilder.Entity<ViewModels.Tag_Details>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Tag_Details>().Property(p => p.id).ValueGeneratedNever();

            modelBuilder.Entity<ViewModels.Tag_User_Settings>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Tag_User_Settings>().Property(p => p.id).ValueGeneratedNever();

            modelBuilder.Entity<ViewModels.Tag_User_Response>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Tag_User_Response>().Property(p => p.id).ValueGeneratedNever();
        }
    }
}