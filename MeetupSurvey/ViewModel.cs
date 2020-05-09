using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Autofac;
using MeetupSurvey.Logging.AppCenter;
using MeetupSurvey.Core;
using Prism.AppModel;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Newtonsoft.Json;
using MeetupSurvey.DTO;

namespace MeetupSurvey
{
    public abstract class ViewModel : ReactiveObject,
                                  INavigatingAware,
                                  INavigatedAware,
                                  IDestructible,
                                  IPageLifecycleAware,
                                  IConfirmNavigationAsync
    {
        private readonly ILogger _logger;

        CompositeDisposable deactivateWith;

        protected ViewModel()
        {
            _logger = App.Container.Resolve<ILogger>();
        }

        protected CompositeDisposable DeactivateWith
        {
            get
            {
                if (this.deactivateWith == null)
                    this.deactivateWith = new CompositeDisposable();

                return this.deactivateWith;
            }
        }

        public CompositeDisposable DestroyWith { get; } = new CompositeDisposable();


        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
            
        }


        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }


        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            LogNavigationEvent();
        }

        private void LogNavigationEvent()
        {
            string name = this.GetType().Name;
            _logger.WriteEvent("Navigated", new Dictionary<string, string>()
            {
                {"ViewModel", name},
            });
        }


        public virtual void OnAppearing()
        {
        }


        public virtual void OnDisappearing()
        {
            this.deactivateWith?.Dispose();
            this.deactivateWith = null;
        }


        public virtual void Destroy()
        {
            this.DestroyWith?.Dispose();
        }


        public virtual Task<bool> CanNavigateAsync(INavigationParameters parameters) => Task.FromResult(true);


        [Reactive] public bool IsBusy { get; protected set; }

        protected virtual void RegisterBusyCommand<T, U>(ReactiveCommand<T, U> command) => command
            .IsExecuting
            .Subscribe(x => this.IsBusy = x);


        protected virtual async Task GoBackAndRefresh(INavigationService navigationService, NavigationParameters parameters = null, bool shouldRefresh = true)
        {
            if (parameters == null) parameters = new NavigationParameters();
            if (shouldRefresh)
                parameters.Add(Infrastructure.KnownNavigationParameters.ShouldRefresh, true);
            await navigationService.GoBackAsync(parameters);
        }
    }
}
