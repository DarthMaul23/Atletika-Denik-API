using System.Text.Json;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Atletika_Denik_API.Data.Models
{
    public class UserContext : DbContext
    {
        public DbSet<ViewModels.Users> Users { get; set; }
        public DbSet<ViewModels.Asociace_Trener_Uzivatel> Asociace_Trener_Uzivatel { get; set; }
        public DbSet<ViewModels.User_Info> User_Info { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
                .GetSection("ConnectionStrings")["ConnectionString3"]);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViewModels.User>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Users>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.Asociace_Treninku>().HasKey(k => k.id);
            modelBuilder.Entity<ViewModels.User_Info>().HasKey(k => k.UserId);
        }
    }
}