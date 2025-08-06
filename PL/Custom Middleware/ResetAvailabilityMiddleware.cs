using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BLL.Interfaces;
using System;
using System.Threading.Tasks;

namespace PL.Middleware 
{
    public class ResetAvailabilityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResetAvailabilityMiddleware> _logger;
        
        private static DateTime _lastResetDate = DateTime.MinValue; 

        public ResetAvailabilityMiddleware(RequestDelegate next, ILogger<ResetAvailabilityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IServiceScopeFactory scopeFactory)
        {
           
            if (DateTime.Now.Date > _lastResetDate.Date) 
            {
                
                lock (this) 
                {
                    if (DateTime.Now.Date > _lastResetDate.Date) 
                    {
                        _logger.LogInformation("Midnight passed. Attempting to reset daily availability via Middleware.");

                        using (var scope = scopeFactory.CreateScope())
                        {
                            var menuItemService = scope.ServiceProvider.GetRequiredService<IMenuItemService>();
                            try
                            {
                                menuItemService.ResetDailyAvailabilityAtMidnight().Wait(); 
                                _lastResetDate = DateTime.Now.Date; 
                                _logger.LogInformation("Daily availability reset via Middleware completed successfully for {date}.", _lastResetDate.ToShortDateString());
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred while resetting daily availability via Middleware.");
                            }
                        }
                    }
                }
            }

            await _next(context); 
        }
    }
}