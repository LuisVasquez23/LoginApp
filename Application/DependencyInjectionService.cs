using Application.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddTransient<IAuthService, AuthService>();

            return services;
        }
    }
}
