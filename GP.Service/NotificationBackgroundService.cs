using GP.Core.Entities;
using GP.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static GP.Core.Entities.Notification;
using System.Net.Http.Json;
using System.Net.Http;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace GP.Service
{
  

    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
   
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Runs every hour

        public NotificationBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<StoreContext>();

                    await SendAppointmentReminders(dbContext);
                    await SendReviewReminders(dbContext);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task SendAppointmentReminders(StoreContext dbContext)
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);

            var upcomingAppointments = await dbContext.Appointments
                .Where(a => a.AppointmentDate.Date == tomorrow)
                .ToListAsync();

            foreach (var appointment in upcomingAppointments)
            {
                var notificationExists = await dbContext.notifications.AnyAsync(n =>
                    n.UserId == appointment.UserId &&
                    n.Message.Contains("Reminder for your appointment") &&
                    n.DeliveredAt.Date == DateTime.UtcNow.Date);

                if (!notificationExists)
                {
                    dbContext.notifications.Add(new Notification
                    {
                        UserId = appointment.UserId,
                        Message = $"Reminder for your appointment on {appointment.AppointmentDate:MMMM dd, yyyy} at {appointment.AppointmentDate:hh:mm tt}",
                        Status = Notification.MessageStatus.Unread,
                        DeliveredAt = DateTime.UtcNow
                    });

                    await dbContext.SaveChangesAsync();
                }
            }
        } 

        private async Task SendReviewReminders(StoreContext dbContext)
        {
            var pastAppointments = await dbContext.Appointments
                .Where(a => a.AppointmentDate < DateTime.UtcNow && !a.IsReviewed)
                .ToListAsync();

            foreach (var appointment in pastAppointments)
            {
                var notificationExists = await dbContext.notifications.AnyAsync(n =>
                    n.UserId == appointment.UserId &&
                    n.Message.Contains("How was your appointment?") &&
                    n.DeliveredAt.Date == DateTime.UtcNow.Date);

                if (!notificationExists)
                {
                    dbContext.notifications.Add(new Notification
                    {
                        UserId = appointment.UserId,
                        Message = $"How was your appointment on {appointment.AppointmentDate:MMMM dd, yyyy}? Leave a review!",
                        Status = Notification.MessageStatus.Unread,
                        DeliveredAt = DateTime.UtcNow
                    });
                }
                appointment.IsReviewed = true;
            }
            

            await dbContext.SaveChangesAsync();

                


                    
                
            }
        }
    }


