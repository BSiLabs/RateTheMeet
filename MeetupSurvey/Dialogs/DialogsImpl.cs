using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MeetupSurvey.Core;
using ReactiveUI;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace MeetupSurvey.Dialogs
{
    public class DialogsImpl : IDialogs
    {
        private string contextResult = null;
        readonly ILocalize localize;
        public DialogsImpl(ILocalize localize)
            => this.localize = localize;



        public IObservable<string> ActionSheet(string title, List<ActionItem> itemList) => Observable.FromAsync<string>(async ct =>
        {
            var tcs = new TaskCompletionSource<string>();
            using (OnCancel(ct, () => tcs.TrySetCanceled()))
            {
                foreach (var item in itemList)
                    item.Action = PopAction(() => tcs.TrySetResult(item.Id));

                var vm = new ActionSheetViewModel()
                {
                    Title = title,
                    ItemList = itemList,
                    CancelText = "Cancel",//this.localize["Cancel"],
                    Cancel = PopAction(() => tcs.TrySetResult(null))
                };
                await PopupNavigation.Instance.PushAsync(new ActionSheetPage
                {
                    BindingContext = vm
                });
                return await tcs.Task;
            }
        });





        ContextMenuAction contextActionResult = null;
        public IObservable<ContextMenuAction> ContextAction(List<ActionItem> actionItems, VerticalOptions verticalOptions) => Observable.FromAsync<ContextMenuAction>(async (ca) =>
        {
            contextActionResult = null;
            var tcs = new TaskCompletionSource<ContextMenuAction>();
            using (OnCancel(ca, () => tcs.TrySetCanceled()))
            {
                foreach (var item in actionItems)
                {

                    item.ShowSeparator = true;
                    item.Action = ReactiveCommand.CreateFromTask(async () =>
                    {
                        contextActionResult = new ContextMenuAction() { Id = item.Id, Action = item.ContextAction };
                        await PopupNavigation.Instance.PopAsync();
                    });
                }

                actionItems.LastOrDefault().ShowSeparator = false;
                var vm = new ContextMenuViewModel()
                {
                    ItemList = actionItems,
                    Close = Action(() =>
                    {
                        tcs.TrySetResult(contextActionResult);
                    })
                };

                switch(verticalOptions)
                {
                    case VerticalOptions.Center:
                        vm.VerticalPlacement = LayoutOptions.Center;
                        break;
                    case VerticalOptions.Start:
                        vm.VerticalPlacement = LayoutOptions.Start;
                        break;
                    case VerticalOptions.End:
                        vm.VerticalPlacement = LayoutOptions.End;
                        break;
                }
                await PopupNavigation.Instance.PushAsync(new ContextMenuPage
                {
                    BindingContext = vm
                });
                return await tcs.Task;
            }
        });

        public IObservable<string> Toast(string error, string message) => Observable.FromAsync<string>(async ct =>
        {
            var tcs = new TaskCompletionSource<string>();
            using (OnCancel(ct, () => tcs.TrySetCanceled()))
            {
                var vm = new ToastViewModel()
                {
                    Error = error,
                    Message = message
                };
                await PopupNavigation.Instance.PushAsync(new ToastPage
                {
                    BindingContext = vm
                });
                return await tcs.Task;
            }
        });


        public IObservable<Unit> Alert(string title, string message, string ok = null) => Observable.FromAsync<Unit>(async ct =>
        {
            var tcs = new TaskCompletionSource<object>();
            using (OnCancel(ct, () => tcs.TrySetCanceled()))
            {
                var vm = new AlertViewModel
                {
                    Title = title,
                    Message = message,
                    OkText = ok ?? this.localize["Ok"],
                    Ok = PopAction(() => tcs.TrySetResult(Unit.Default))
                };
                await PopupNavigation.Instance.PushAsync(new AlertPage
                {
                    BindingContext = vm
                });
                await tcs.Task;
                return Unit.Default;
            }
        });


        public IObservable<bool> Confirm(string title, string message, string ok = null, string cancel = null) => Observable.FromAsync<bool>(async ct =>
        {
            var tcs = new TaskCompletionSource<bool>();
            using (OnCancel(ct, () => tcs.TrySetCanceled()))
            {
                var vm = new AlertViewModel
                {
                    Title = title,
                    Message = message,
                    OkText = ok ?? this.localize["Ok"],
                    Ok = PopAction(() => tcs.TrySetResult(true)),
                    Cancel = PopAction(() => tcs.TrySetResult(false)),
                    CancelText = cancel ?? this.localize["Cancel"],
                };
                await PopupNavigation.Instance.PushAsync(new AlertPage
                {
                    BindingContext = vm
                });
                return await tcs.Task;
            }
        });

        static ICommand Action(Action postAction) => ReactiveCommand.CreateFromTask(
            async _ =>
            {
                postAction();
            }
        );

        static ICommand PopAction(Action postAction) => ReactiveCommand.CreateFromTask(
            async _ =>
            {

                await PopupNavigation.Instance.PopAsync();
                postAction();
            }
        );


        static IDisposable OnCancel(CancellationToken cancelToken, Action setTcs)
             => cancelToken.Register(() =>
             {
                 // if already on the main thread, this will not block, otherwise it will, but this will only happen if we weren't on the main thread anyhow
                 // should watch for exceptions, but if the dialog won't close here, there is shit going on we need to know about
                 Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => await PopupNavigation.Instance.PopAsync());
                 setTcs?.Invoke();
             });


    }
}
