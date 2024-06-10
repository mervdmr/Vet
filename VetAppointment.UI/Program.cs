using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VetAppointment.Lib.App.Helper;
using VetAppointment.Lib.App.Model;
using VetAppointment.Lib.Infra;
using VetAppointment.Lib.Infra.SeedData;
using VetAppointment.Lib.Infra.UnitOfWork;

namespace VetAppointment.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IServiceCollection services = builder.Services;


            services.AddDbContext<VetAppointmentDbContext>(options =>
                        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IClaimsHelper, ClaimsHelper>();
            services.AddTransient<IAppContext, Lib.App.Model.AppContext>();

            services.AddOptions();
            services.AddMvcCore().AddApiExplorer();
            services.AddHttpClient();
            #region Authentication

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.LoginPath = new PathString("/login");
                options.LogoutPath = new PathString("/logout");
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.Name = ".WebAppointment";
                options.Cookie.Path = "/";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
            #endregion

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var scope = app.Services.CreateScope();
                var serviceProvider = scope.ServiceProvider;
                var database = serviceProvider.GetRequiredService<VetAppointmentDbContext>();
                database.Database.EnsureCreated();
                SeedData.Initialize(database).GetAwaiter().GetResult();

            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
