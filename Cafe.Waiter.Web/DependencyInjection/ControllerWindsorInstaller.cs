using Cafe.Waiter.Web.Controllers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Cafe.Waiter.Web.DependencyInjection
{
    public class ControllerWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var controllerBasedOnDescriptor = Classes
                .FromAssemblyContaining<TabController>()
                .InSameNamespaceAs<TabController>()
                .WithServiceSelf()
                .WithServiceAllInterfaces();

            container.Register(
                Classes
                    .FromAssemblyContaining<MessageBus>()
                    .InSameNamespaceAs<MessageBus>()
                    .WithServiceSelf()
                    .WithServiceAllInterfaces(),
                SetControllerLifestyle(controllerBasedOnDescriptor)
            );
        }

        private BasedOnDescriptor SetControllerLifestyle(BasedOnDescriptor controllerBasedOnDescriptor)
        {
            return ControllerLifestyleConfigurator.Instance.SetLifestyle(controllerBasedOnDescriptor);
        }
    }
}