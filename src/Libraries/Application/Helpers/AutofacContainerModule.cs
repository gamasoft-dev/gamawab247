using System.Reflection;
using Application.Services.Implementations.BusinessMessageImpls;
using Application.Services.Interfaces;
using Autofac;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Module = Autofac.Module;

namespace Application.Helpers
{
    public class AutofacContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();
            
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(IBusinessMessageMgtService<,>))
                .AsImplementedInterfaces();
            
            builder.RegisterAssemblyTypes(typeof(IAutoDependencyService).Assembly)
                .AssignableTo<IAutoDependencyService>()
                .As<IAutoDependencyService>()
                .AsImplementedInterfaces().InstancePerLifetimeScope();


            base.Load(builder);
        }
    }
}