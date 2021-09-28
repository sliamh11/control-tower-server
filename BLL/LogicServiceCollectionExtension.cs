using BLL.Data_Objects;
using BLL.Interfaces;
using BLL.Logic;
using Microsoft.Extensions.DependencyInjection;


namespace BLL
{
    public static class LogicServiceCollectionExtension
    {
        public static IServiceCollection AddLogicDependancies(this IServiceCollection services)
        {
            // Singletons
            services.AddSingleton<ITowerManager, TowerManager>();
            services.AddSingleton<IStationsState, StationsState>();

            // Scoped
            services.AddScoped<IDepartureObj, DepartureObj>();
            services.AddScoped<ILandingObj, LandingObj>();

            // Transient
            services.AddTransient<IStationsLogic, StationsLogic>();
            services.AddTransient<ILandingLogic, LandingLogic>();
            services.AddTransient<IDepartureLogic, DepartureLogic>();

            return services;
        }
    }
}
