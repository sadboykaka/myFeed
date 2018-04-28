using System;
using System.Linq;
using System.Reactive;
using DryIocAttributes;
using myFeed.Interfaces;
using myFeed.Models;
using myFeed.Platform;
using PropertyChanged;
using ReactiveUI;

namespace myFeed.ViewModels
{
    [Reuse(ReuseType.Transient)]
    [AddINotifyPropertyChangedInterface]
    [ExportEx(typeof(FeedGroupViewModel))]
    public sealed class FeedGroupViewModel
    {
        public IReactiveDerivedList<FeedItemViewModel> Items { get; }
        public ReactiveCommand<Unit, Unit> Modify { get; }
        public ReactiveCommand<Unit, Unit> Fetch { get; }

        public bool IsLoading { get; private set; } = true;
        public bool IsEmpty { get; private set; }
        public string Title { get; }

        public FeedGroupViewModel(
            Func<Article, FeedItemViewModel> factory,
            INavigationService navigationService,
            IFeedStoreService feedStoreService,
            ISettingManager settingManager,
            Category category)
        {
            var showRead = true;
            var cache = new ReactiveList<FeedItemViewModel> {ChangeTrackingEnabled = true};
            Items = cache.CreateDerivedCollection(x => x, x => !(!showRead && x.Read));
            Items.CountChanged.Subscribe(x => IsEmpty = x == 0);
            Title = category.Title;
            
            Modify = ReactiveCommand.CreateFromTask(() => navigationService.Navigate<ChannelViewModel>());
            Fetch = ReactiveCommand.CreateFromTask(async () =>
            {
                IsLoading = true;
                var settings = await settingManager.Read();
                var response = await feedStoreService.Load(category.Channels);
                var viewModels = response.Select(factory).ToList();
                showRead = settings.Read;
                cache.Clear();
                cache.AddRange(viewModels);
                IsLoading = false;
            });
        }
    }
}