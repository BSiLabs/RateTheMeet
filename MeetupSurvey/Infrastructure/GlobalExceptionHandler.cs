using System;
using System.Reactive.Threading.Tasks;
using Autofac;
using HttpTracer;
using MeetupSurvey.Core;
using MeetupSurvey.Device.Impl;
using MeetupSurvey.Root;
using ReactiveUI;
using Xamarin.Essentials;
using ILogger = MeetupSurvey.Core.ILogger;
using System.Collections.Generic;
using Prism.Navigation;
using Xamarin.Forms;
using System.Reactive.Linq;

namespace MeetupSurvey.Infrastructure
{
    public class GlobalExceptionHandler : IObserver<Exception>, IStartable
    {
        readonly ILogger logger;
        readonly IDialogs dialogs;
        readonly ILocalize localize;


        public GlobalExceptionHandler(ILogger logger,
                                      IDialogs dialogs,
                                      ILocalize localize)
        {
            this.logger = logger;
            this.dialogs = dialogs;
            this.localize = localize;
        }


        public void Start() => RxApp.DefaultExceptionHandler = this;

        public void OnCompleted() { }

        public void OnError(Exception error) { }// => HandleException(logger, error);

        public void OnNext(Exception value) => HandleException(logger, value);

        public async void HandleException(ILogger logger, Exception exception)
        {
            Console.WriteLine(exception);
            var properties = new Dictionary<string, string>();
            try
            {
                var isOnline = Connectivity.NetworkAccess.Equals(NetworkAccess.Internet);

                string navPath = GetNavPath();

                properties.Add("Context", $"Online:{isOnline};NavPath:{navPath}");
                logger.WriteCrash(exception, properties);
                await this.dialogs.Alert("Woops!", "Woops! There seems to have been an error processing your request");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logger Error: " + ex.ToString());
            }
        }

        private static string GetNavPath()
        {
            string navPath;
            try
            {
                navPath = App.Navigation.GetNavigationUriPath();
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("GetNavigationUriPath Error: " + ex.ToString());
                if (App.Current.MainPage is NavigationPage navigationPage)
                    navPath = navigationPage.CurrentPage.GetType().Name;
                else
                    navPath = App.Current.MainPage.GetType().Name;
            }
            return navPath;
        }
    }
}
