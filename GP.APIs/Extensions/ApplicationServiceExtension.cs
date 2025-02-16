using GP.Repository.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;



using GP.Core.Entities.Identity;
using GP.Core.IRepository;
using GP.Service.Repository;
using GP.Repository.Repository;

namespace GP.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection Services)
        {
            
            Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            Services.AddScoped(typeof(IXRayReportRepository), typeof(XRayReportRepository));
            Services.AddScoped(typeof(IUserCartRepository), typeof(UserCartRepository));
            Services.AddScoped(typeof(IReviewRepository), typeof(ReviewRepository));
            Services.AddScoped(typeof(IProductRepository), typeof(ProductRepository));
            Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
            Services.AddScoped(typeof(IOrderItemRepository), typeof(OrderItemRepository));
            Services.AddScoped(typeof(INotificationRepository), typeof(NotificationRepository));
            Services.AddScoped(typeof(ICartItemsRepository), typeof(CartItemsRepository));
            Services.AddScoped(typeof(IAppointmentRepository), typeof(AppointmentRepository));
            Services.AddScoped(typeof(IDeliveryMethodRepository), typeof(DeliveryMethodRepository));
            return Services;

        }
    }
 }
