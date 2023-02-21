using System;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCustomization
{
	public static class ServiceCollectionExtension
	{
        /// <summary>
        /// Utilize this to register other application services that do not utilize a scoped lifetime
        /// </summary>
        /// <param name="services"></param>
        public static void AddApiCustomizationService(this IServiceCollection services)
        {
            services.AddHttpClient();
        }
    }
}

