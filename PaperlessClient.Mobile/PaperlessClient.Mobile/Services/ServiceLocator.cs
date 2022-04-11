using Autofac;
using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Services
{
    internal class ServiceLocator
    {
        private static bool setupComplete;
        private static IContainer _container;

        public static void Setup()
        {
            if (setupComplete)
                throw new InvalidOperationException("Setup() was allready called. Only call this once!");

            Akavache.Registrations.Start("de.ddoering.paperless");

            var builder = new ContainerBuilder();
            builder.RegisterType<PersistenceService>().As<IPersistenceService>().SingleInstance();
            builder.RegisterType<NotificationService>().As<INotificationService>().SingleInstance();
            builder.RegisterType<ApiService>().As<IApiService>().SingleInstance();
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();

            //vms
            builder.RegisterType<SetupViewModel>().AsSelf();
            builder.RegisterType<UploadFileViewModel>().AsSelf();
            builder.RegisterType<LandingViewModel>().AsSelf();

            _container = builder.Build();
            setupComplete = true;
        }

        public static T Resolve<T>() where T : class
        {
            if(!setupComplete)
                throw new InvalidOperationException("You need to call Setup() before resolving the first service!");
            return _container.Resolve<T>();
        }
    }
}
