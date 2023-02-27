using Application.Services.Implementations.BusinessMessageImpls;
using Application.Services.Interfaces;
using Autofac;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;

namespace Application.AutofacDI
{
    public class AutofacContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();
            
            builder.RegisterAssemblyTypes(System.Reflection.Assembly.GetExecutingAssembly())
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