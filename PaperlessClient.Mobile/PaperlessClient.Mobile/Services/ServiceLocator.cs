using Autofac;
using PaperlessClient.Mobile.Services.Abstraction;
using PaperlessClient.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Services
{
    public class ServiceLocator
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
            builder.RegisterType<TenantService>().As<ITenantService>().SingleInstance();
            builder.RegisterType<ApiService>().As<IApiService>().SingleInstance();
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            builder.RegisterType<FileUploadQueueService>().As<IFileUploadQueueService>().SingleInstance();
            builder.RegisterType<DocumentService>().As<IDocumentService>();
            builder.RegisterType<DocumentTypeService>().As<IDocumentTypeService>();
            builder.RegisterType<TagService>().As<ITagService>();
            builder.RegisterType<CorrespondentService>().As<ICorrespondentService>();
            builder.RegisterType<PreferenceService>().As<IPreferenceService>().SingleInstance();

            //vms
            builder.RegisterType<AppShellViewModel>().AsSelf();
            builder.RegisterType<SetupViewModel>().AsSelf();
            builder.RegisterType<UploadFileViewModel>().AsSelf();
            builder.RegisterType<LandingViewModel>().AsSelf();
            builder.RegisterType<TennantListViewModel>().AsSelf();
            builder.RegisterType<DocumentListViewModel>().AsSelf();
            builder.RegisterType<DocumentViewerViewModel>().AsSelf();
            builder.RegisterType<PreferencesViewModel>().AsSelf();
            builder.RegisterType<LockViewModel>().AsSelf();

            //platform specifics
            builder.Register<IAppAuthService>(c => DependencyService.Get<IAppAuthService>());

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
