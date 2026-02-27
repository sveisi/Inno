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

            //Database.ExecuteSqlRaw("CHECKIDENT ('Cargo', RESEED, 1000)");
            /*
            modelBuilder.Entity<Brand>().HasData(new Brand() { BrandId = 1, BrandName = "تویوتا", BrandEnName = "TOYOTA" });
            modelBuilder.Entity<Brand>().HasData(new Brand() { BrandId = 2, BrandName = "هیوندا", BrandEnName = "HYUNDA" });
            modelBuilder.Entity<Brand>().HasData(new Brand() { BrandId = 3, BrandName = "کیا", BrandEnName = "KIA" });
            modelBuilder.Entity<Brand>().HasData(new Brand() { BrandId = 4, BrandName = "پژو", BrandEnName = "PEJOUT" });
            modelBuilder.Entity<Model>().HasData(new Model() { ModelId = 1, ModelName = "سانتافه", ModelEnName = "Santafe", ValueAmount = 20000, Weight = 1200, BrandId = 2 });

            modelBuilder.Entity<Color>().HasData(new Color() { ColorId = 1, ColorName = "سپی", ColorEnName = "White" });
            modelBuilder.Entity<Color>().HasData(new Color() { ColorId = 2, ColorName = "ره‌ش", ColorEnName = "Black" });
            modelBuilder.Entity<Color>().HasData(new Color() { ColorId = 3, ColorName = "شین", ColorEnName = "Blue" });
            modelBuilder.Entity<Color>().HasData(new Color() { ColorId = 4, ColorName = "سور", ColorEnName = "Red" });
            modelBuilder.Entity<Color>().HasData(new Color() { ColorId = 5, ColorName = "زرد", ColorEnName = "Yellow" });

            modelBuilder.Entity<HealthItem>().HasData(new HealthItem() { HealthItemId = 1, ItemName = "کنترل سویچ", ItemEnName = "Remote" });
            modelBuilder.Entity<HealthItem>().HasData(new HealthItem() { HealthItemId = 2, ItemName = "مانیتور", ItemEnName = "Monitor" });
            modelBuilder.Entity<HealthItem>().HasData(new HealthItem() { HealthItemId = 3, ItemName = "فندک" });
            modelBuilder.Entity<HealthItem>().HasData(new HealthItem() { HealthItemId = 4, ItemName = "بلندگو", ItemEnName = "Speaker" });
            modelBuilder.Entity<HealthItem>().HasData(new HealthItem() { HealthItemId = 5, ItemName = "پشتی سر" });
            modelBuilder.Entity<HealthItem>().HasData(new HealthItem() { HealthItemId = 6, ItemName = "کف پوش" });
            modelBuilder.Entity<HealthItem>().HasData(new HealthItem() { HealthItemId = 7, ItemName = "برف پاکن جلو" });
            modelBuilder.Entity<HealthItem>().HasData(new HealthItem() { HealthItemId = 8, ItemName = "برف پاکن عقب" });

            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.Receipt.GetHashCode(), CostGroupName = "رصید", AutoFill = true, CostAmountIsUSD = false, SortOrder = 1, HasUserEntry = true });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.Cargo.GetHashCode(), CostGroupName = "کرێ کارگۆ", AutoFill = true, HasContact = false, CostAmountIsUSD = true, SortOrder = 5, HasUserEntry = true });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.DubaiCost.GetHashCode(), CostGroupName = "کرێ بار دوبه‌ی", AutoFill = true, HasContact = false, CostAmountIsUSD = true, SortOrder = 5, HasUserEntry = true });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.Shipping.GetHashCode(), CostGroupName = "کرێ که‌شتی", AutoFill = true, HasContact = true, CostAmountIsUSD = true, SortOrder = 10, HasUserEntry = true });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.Port.GetHashCode(), CostGroupName = "به‌ند‌ه‌ر", AutoFill = true, HasContact = true, CostAmountIsUSD = true, SortOrder = 20, HasUserEntry = true });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.ShippingByTrailer.GetHashCode(), CostGroupName = "کرێ ترێلە", AutoFill = true, HasContact = true, CostAmountIsUSD = true, SortOrder = 30, HasUserEntry = true });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.IranBorder.GetHashCode(), CostGroupName = "کرێ مەرزی ئێران", AutoFill = true, HasContact = false, CostAmountIsUSD = true, SortOrder = 15, HasUserEntry = true });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.Customs.GetHashCode(), CostGroupName = "گومرک", AutoFill = false, HasContact = true, CostAmountIsUSD = false, SortOrder = 45 });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.COC.GetHashCode(), CostGroupName = "سی‌او‌سی", AutoFill = false, HasContact = true, CostAmountIsUSD = false, SortOrder = 35 });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.Deport.GetHashCode(), CostGroupName = "اخراج", AutoFill = false, HasContact = true, CostAmountIsUSD = false, SortOrder = 40 });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.CustomsFee.GetHashCode(), CostGroupName = "مصروفات", AutoFill = false, HasContact = true, CostAmountIsUSD = false, SortOrder = 50 });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.ExchangeFee.GetHashCode(), CostGroupName = "عموله", AutoFill = false, HasContact = false, CostAmountIsUSD = false, SortOrder = 55 });
            modelBuilder.Entity<CostGroup>().HasData(new CostGroup() { CostGroupId = TransactionGroupType.MoreFee.GetHashCode(), CostGroupName = "پارەی زیاده", AutoFill = false, HasContact = false, CostAmountIsUSD = false, SortOrder = 60 });

            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.Receipt.GetHashCode(), TransactionGroupName = "خەرجی رصید", IsCost = true, SubGroup = TransactionSubGroup.Kurdestan });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.Cargo.GetHashCode(), TransactionGroupName = "کرێ کارگۆ", IsCost = true, SubGroup = TransactionSubGroup.Dubai });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.DubaiCost.GetHashCode(), TransactionGroupName = "خەرجی دوبه‌ی", IsCost = true, SubGroup = TransactionSubGroup.Dubai });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.Shipping.GetHashCode(), TransactionGroupName = "خەرجی که‌شتی", IsCost = true, SubGroup = TransactionSubGroup.Dubai });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.Port.GetHashCode(), TransactionGroupName = "خەرجی به‌ند‌ه‌ر", IsCost = true, SubGroup = TransactionSubGroup.Iran });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.ShippingByTrailer.GetHashCode(), TransactionGroupName = "خەرجی ترێلە", IsCost = true, SubGroup = TransactionSubGroup.Kurdestan });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.IranBorder.GetHashCode(), TransactionGroupName = "خەرجی مەرزی ئیران", IsCost = true, SubGroup = TransactionSubGroup.Iran });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.Customs.GetHashCode(), TransactionGroupName = "خەرجی گومرک", IsCost = true, SubGroup = TransactionSubGroup.Kurdestan });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.COC.GetHashCode(), TransactionGroupName = "خەرجی سی‌او‌سی", IsCost = true, SubGroup = TransactionSubGroup.Kurdestan });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.Deport.GetHashCode(), TransactionGroupName = "خەرجی اخراج", IsCost = true, SubGroup = TransactionSubGroup.Kurdestan });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.CustomsFee.GetHashCode(), TransactionGroupName = "خەرجی مصروفات", IsCost = true, SubGroup = TransactionSubGroup.Kurdestan });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.ExchangeFee.GetHashCode(), TransactionGroupName = "عموله", IsCost = true, SubGroup = TransactionSubGroup.Kurdestan });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.MoreFee.GetHashCode(), TransactionGroupName = "پارەی زیاده", IsCost = true, SubGroup = TransactionSubGroup.Kurdestan });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.CarImport.GetHashCode(), TransactionGroupName = "داهاتی داخڵکردنی سیاره", IsCost = false, SubGroup = TransactionSubGroup.Kurdestan });
            modelBuilder.Entity<TransactionGroup>().HasData(new TransactionGroup() { TransactionGroupId = TransactionGroupType.CarSale.GetHashCode(), TransactionGroupName = "فروش", IsCost = false });

            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 1, ContactType = ContactType.Company, ContactName = "سەنگەر احمد عبداللە" }); ;
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 2, ContactType = ContactType.Company, ContactName = "کومپانی کورد" });
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 3, ContactType = ContactType.Company, ContactName = "کومپانی نور" });
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 4, ContactType = ContactType.Merchant, CityId = 7, ContactName = "عبدالله مصطفی" });
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 5, ContactType = ContactType.Shipping, ContactName = "مهدی خرم ترابر" });
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 6, ContactType = ContactType.Shipping, ContactName = "محمد خالد" });
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 7, ContactType = ContactType.Mokhalas, ContactName = "مخلص 1" });
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 8, ContactType = ContactType.Personel, ContactName = "ابوبکر" });
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 9, ContactType = ContactType.Agent, ContactName = "مصطفی", AccountBalance = 500000 });
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 10, ContactType = ContactType.Driver, ContactName = "شوفیر 1" });
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 11, ContactType = ContactType.ShippingAgent, ContactName = "نماینده 1" });
            modelBuilder.Entity<Contact>().HasData(new Contact() { ContactId = 12, ContactType = ContactType.Banker, ContactName = "صراف 1" });

            modelBuilder.Entity<Transaction>().HasData(new Transaction() { TransactionId = 1, TransactionGroupId = 1, ContactId = 9, Amount = 500000, IsUSD = false, TransactionDate = new System.DateTime(2025, 07, 01) });
            modelBuilder.Entity<Receipt>().HasData(new Receipt() { ReceiptId = 1, ReceiptCount = 100, RemainCount = 100, CostAmount = 500000, BorderId = 1, CompanyId = 1, AgentId = 9, TransactionId = 1, CreatedDate = new System.DateTime(2025, 07, 01) });
            */
        }

        public async System.Threading.Tasks.Task<int> ExecuteRawSqlAsync(string sql, params object[] parameters)
        {
            return await Database.ExecuteSqlRawAsync(sql, parameters);
        }
    }
}