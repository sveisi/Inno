using Inno.Data;
using Inno.Helper;
using Inno.Helper.ConventionalMetadataProviders;
using Inno.MappingProfiles;
using Inno.Services;
using Inno.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace Inno
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ExceptionFilter>();
            })
                .AddRazorRuntimeCompilation();

            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddDbContext<InnoContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("InnoContext")));

            services.AddScoped<DocumentDbService, DocumentDbService>();
            services.AddScoped<IUserContextService, UserContextService>();

            //اضافه کردن خودکار سرویس ها
            var assembly = typeof(ICategoryService).Assembly;
            var serviceTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"));
            foreach (var implementationType in serviceTypes)
            {
                var interfaceType = implementationType.GetInterfaces()
                    .FirstOrDefault(i => i.Name == $"I{implementationType.Name}");

                if (interfaceType != null)
                    services.AddScoped(interfaceType, implementationType);
            }

            services.AddIdentity<Models.User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 0;
            })
                .AddEntityFrameworkStores<InnoContext>()
                .AddDefaultTokenProviders();

            // Configure supported cultures and localization options
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), typeof(Startup));

            services.AddMvc(options =>
            {
                options.SetConventionalMetadataProviders<Resources.SharedResource, Resources.ValidationMessages, Resources.BindingMessages>();
            })
              .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, options => options.ResourcesPath = "Resources")
              .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "fa" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // ایجاد نسخه‌های قابل ویرایش از فرهنگ‌ها
            var faCulture = (CultureInfo)CultureInfo.GetCultureInfo("fa").Clone();
            faCulture.NumberFormat.NumberDecimalSeparator = ".";
            faCulture.NumberFormat.NumberGroupSeparator = ",";

            var supportedCultures = new[] { faCulture };

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(faCulture),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            app.UseRequestLocalization(localizationOptions);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}