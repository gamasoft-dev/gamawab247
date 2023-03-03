using System;
using ApiCustomization.ABC;
using Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCustomization
{
	public static class ServiceCollectionExtension
	{
        /// <summary>
        /// Utilize this to register other application services that do not utilize a scoped lifetime
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureApiCustomizationService(this IServiceCollection services, IConfiguration configuration)
        {
            ApiCustomizationIOObject(services, configuration);
        }

        private static void ApiCustomizationIOObject(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AlphaBetaConfig>(configuration.GetSection(nameof(AlphaBetaConfig)));

        }
    }
}

