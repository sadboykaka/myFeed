﻿using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DryIocAttributes;
using myFeed.Interfaces;
using myFeed.Models;
using PropertyChanged;
using ReactiveUI;

namespace myFeed.ViewModels
{
    [Reuse(ReuseType.Transient)]
    [ExportEx(typeof(SearchViewModel))]
    [AddINotifyPropertyChangedInterface]
    public sealed class SearchViewModel
    {
        private readonly Func<FeedlyItem, SearchItemViewModel> _factory;
        private readonly ISearchService _searchService;

        public ReactiveList<SearchItemViewModel> Items { get; }
        public Interaction<Exception, bool> Error { get; }
        public ReactiveCommand<Unit, Unit> Fetch { get; }

        public string SearchQuery { get; set; } = string.Empty;
        public bool IsGreeting { get; private set; } = true;
        public bool IsLoading { get; private set; }
        public bool IsEmpty { get; private set; }

        public SearchViewModel(
            Func<FeedlyItem, SearchItemViewModel> factory,
            ISearchService searchService)
        {
            _factory = factory;
            _searchService = searchService;
            Items = new ReactiveList<SearchItemViewModel>();
            Fetch = ReactiveCommand.CreateFromTask(DoFetch);
            this.WhenAnyValue(x => x.SearchQuery)
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Throttle(TimeSpan.FromSeconds(0.8))
                .Select(x => Unit.Default)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(Fetch);

            Fetch.IsExecuting.Skip(1)
                .Do(x => IsGreeting = false)
                .Subscribe(x => IsLoading = x);
            Items.CountChanged
                .Select(count => count == 0)
                .Subscribe(x => IsEmpty = x);

            Error = new Interaction<Exception, bool>();
            Fetch.ThrownExceptions
                .SelectMany(x => Error.Handle(x))
                .Where(retry => retry)
                .Select(x => Unit.Default)
                .InvokeCommand(Fetch);
        }

        private async Task DoFetch()
        {
            var search = await _searchService.Search(SearchQuery);
            var viewModels = search.Results.Select(_factory);
            Items.Clear();
            Items.AddRange(viewModels);
        }
    }
}