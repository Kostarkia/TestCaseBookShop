using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using TestCaseBookShop.Models;
using TestCaseBookShop.Models.Data.Auth;
using TestCaseBookShop.Models.Data.Enum;
using TestCaseBookShop.Util;
using static TestCaseBookShop.Services.TokenGenerateService;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        //authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Token:Issuer"],
                ValidAudience = builder.Configuration["Token:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"])),
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddScoped<ITokenService, TokenService>();

        builder.Services.AddAuthorization();

        // Configure Serilog
        var _logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext().CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(_logger);


        var app = builder.Build();

        using (var serviceScope = app.Services.CreateScope())
        {
            try
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                var defaultUser = "ferhatbolat@carettaerp.com";

                if (!context.Users.Any())
                {
                    var _password = EncryptionDecryptionUtil.Encrypt("caretta");

                    var userData = new User
                    {
                        ID = Guid.NewGuid(),
                        Password = _password,
                        CreatedAt = DateTimeOffset.UtcNow,
                        UserName = defaultUser,
                        RecordState = RecordState.Active,
                    };

                    context.Users.Add(userData);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
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