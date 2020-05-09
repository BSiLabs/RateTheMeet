using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Mvvm;
using Prism.Autofac;
using System;
using MeetupSurvey.Root;
using MeetupSurvey.Account;
using MeetupSurvey.Survey;
using Autofac;
using MeetupSurvey.Dialogs;
using Prism.Navigation;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MeetupSurvey
{

    public partial class App
    {
        // TODO: <HACK> Dylan: need to be able to inject INaviationService into GlobalExceptionHandler to get nav path
        public static INavigationService Navigation { get; private set; }
        public static IContainer Container { get; private set; }

        public App() : this(null) { }

        public static MainPage MasterDetail;
        public App(IPlatformInitializer initializer) : base(initializer, false) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

//            if (Xamarin.Forms.Application.Current.MainPage is MasterDetailPage masterDetailPage)
//            {
//                masterDetailPage.IsGestureEnabled = true;
//            }

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewModelTypeName = viewType.FullName.Replace("Page", "ViewModel");
                var viewModelType = Type.GetType(viewModelTypeName);
                return viewModelType;
            });
            Navigation = this.NavigationService;
            await this.NavigationService.NavigateAsync("NavigationPage/StartupPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<StartupPage>();
            containerRegistry.RegisterForNavigation<LoginPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<SurveyListPage>();
            containerRegistry.RegisterForNavigation<SurveyPage>();
            containerRegistry.RegisterForNavigation<EditSurveyPage>();
            containerRegistry.RegisterForNavigation<ArchiveListPage>();
            containerRegistry.RegisterForNavigation<MyGroupsPage>();
            containerRegistry.RegisterForNavigation<PrizePage>();
            containerRegistry.RegisterForNavigation<SurveyResultPage>();
            containerRegistry.RegisterForNavigation<ActionSheetPage>();
            containerRegistry.RegisterForNavigation<AlertPage>();
            containerRegistry.RegisterForNavigation<ContextMenuPage>();
            containerRegistry.RegisterForNavigation<EditPrizePage>();
            containerRegistry.RegisterForNavigation<AboutPage>();
                  
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CoreModule>();
            builder.RegisterBuildCallback(container => Container = container);
            return new AutofacContainerExtension(builder);
        }
    }
}
