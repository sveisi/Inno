using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Inno.Models;

namespace Inno.Data
{
    public class InnoContext : IdentityDbContext<User>
    {
        //خودکارسازی
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SKU> SKUs { get; set; }

        public InnoContext(DbContextOptions<InnoContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //register entities config
            modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Unit>().HasData(new Unit() { Id = 1, Name = "متر", EnName = "Meter", Symbol = "M" });
            modelBuilder.Entity<Unit>().HasData(new Unit() { Id = 2, Name = "عدد", EnName = "Quantity", Symbol = "Qty" });

            modelBuilder.Entity<Category>().HasData(new Category() { Id = 1, Name = "پارچه", EnName = "Fabric" });
            modelBuilder.Entity<Category>().HasData(new Category() { Id = 10, Name = "اکسسوری", EnName = "Accessory" });

            modelBuilder.Entity<Color>().HasData(new Color() { Id = 1, Name = "سفید", EnName = "White" });
            modelBuilder.Entity<Color>().HasData(new Color() { Id = 2, Name = "کرم", EnName = "Cream" });
            modelBuilder.Entity<Color>().HasData(new Color() { Id = 3, Name = "سبز", EnName = "Green" });
            modelBuilder.Entity<Color>().HasData(new Color() { Id = 4, Name = "آبی", EnName = "Blue" });
            modelBuilder.Entity<Color>().HasData(new Color() { Id = 5, Name = "قرمز", EnName = "Red" });

            //modelBuilder.Entity<Region>().HasData(new Region() { Id = 1, Type = RegionType.Country, Name = "ایران", EnName = "Iran", Code = "IR" });
            //modelBuilder.Entity<Region>().HasData(new Region() { Id = 10, Type = RegionType.Province, ParentId = 1, Name = "تهران", EnName = "Tehran", Code = "10" });
            modelBuilder.Entity<Region>().HasData(new Region() { Id = 11, Type = RegionType.City, ParentId = 10, Name = "تهران", EnName = "Tehran", Code = "11" });
            modelBuilder.Entity<Region>().HasData(new Region() { Id = 12, Type = RegionType.City, ParentId = 10, Name = "کرج", EnName = "Karaj", Code = "12" });

            //modelBuilder.Entity<Region>().HasData(new Region() { Id = 20, Type = RegionType.Province, ParentId = 1, Name = "اصفهان", EnName = "Esfahan", Code = "20" });
            modelBuilder.Entity<Region>().HasData(new Region() { Id = 13, Type = RegionType.City, ParentId = 12, Name = "اصفهان", EnName = "Esfahan", Code = "21" });

            modelBuilder.Entity<Storage>().HasData(new Storage() { Id = 1, Name = "اصلی", EnName = "Main" });
            modelBuilder.Entity<Location>().HasData(new Location() { Id = "T", Name = "موقت", EnName = "Temp", StorageId = 1 });

            //Database.ExecuteSqlRaw("CHECKIDENT ('Cargo', RESEED, 1000)");
        }

        public async System.Threading.Tasks.Task<int> ExecuteRawSqlAsync(string sql, params object[] parameters)
        {
            return await Database.ExecuteSqlRawAsync(sql, parameters);
        }
    }
}