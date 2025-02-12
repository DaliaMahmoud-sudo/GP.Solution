
using GP.APIs.Extensions;
using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using GP.Repository.Data;
using GP.Service.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace GP.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Configure Service
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped(typeof(IXRayReportRepository), typeof(XRayReportRepository));
            builder.Services.AddScoped(typeof(IUserCartRepository), typeof(UserCartRepository));
            builder.Services.AddScoped(typeof(IReviewRepository), typeof(ReviewRepository));
            builder.Services.AddScoped(typeof(IProductRepository), typeof(ProductRepository));
            builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
            builder.Services.AddScoped(typeof(IOrderItemRepository), typeof(OrderItemRepository));
            builder.Services.AddScoped(typeof(INotificationRepository), typeof(NotificationRepository));
            builder.Services.AddScoped(typeof(ICartItemsRepository), typeof(CartItemsRepository));
            builder.Services.AddScoped(typeof(IAppointmentRepository), typeof(AppointmentRepository));
                                                                             

            builder.Services.AddIdentityServices(builder.Configuration);
            #endregion


            var app = builder.Build();
            #region Update-Database
            using var Scope = app.Services.CreateScope();
            var Services = Scope.ServiceProvider;
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {

                var DbContext = Services.GetRequiredService<StoreContext>();
                await DbContext.Database.MigrateAsync();

               
                var UserManager = Services.GetRequiredService<UserManager<AppUser>>();
                await AdminSeed.SeedAdminAsync(UserManager);
             //   await StoreContextSeed.SeedAsync(DbContext);
            }
            catch (Exception ex)
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "an error accured during applying the migration");

            }
            #endregion


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
