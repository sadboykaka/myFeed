﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DryIocAttributes;
using myFeed.Events;
using myFeed.Interfaces;
using myFeed.Models;
using myFeed.Platform;
using PropertyChanged;
using ReactiveUI;

namespace myFeed.ViewModels
{
    [Reuse(ReuseType.Transient)]
    [AddINotifyPropertyChangedInterface]
    [ExportEx(typeof(ChannelItemViewModel))]
    public sealed class ChannelItemViewModel
    {
        private readonly ICategoryManager _categoryManager;
        private readonly IPlatformService _platformService;
        private readonly IMessageBus _messageBus;
        private readonly Channel _channel;

        public Interaction<Unit, bool> DeleteRequest { get; }
        public ReactiveCommand<Unit, Unit> Delete { get; }
        public ReactiveCommand<Unit, Unit> Open { get; }
        public ReactiveCommand<Unit, Unit> Copy { get; }

        public string Name => new Uri(_channel.Uri).Host;
        public string Url => _channel.Uri;
        public bool Notify { get; set; }

        public ChannelItemViewModel(
            ICategoryManager categoryManager,
            IPlatformService platformService,
            IMessageBus messageBus,
            Channel channel)
        {
            _categoryManager = categoryManager;
            _platformService = platformService;
            _messageBus = messageBus;
            _channel = channel;

            Notify = _channel.Notify;
            this.WhenAnyValue(x => x.Notify).Skip(1)
                .Do(notify => _channel.Notify = notify)
                .Select(notify => channel)
                .SelectMany(_categoryManager.Update)
                .Subscribe();
            
            DeleteRequest = new Interaction<Unit, bool>();
            Delete = ReactiveCommand.CreateFromTask(DoDelete);
            Copy = ReactiveCommand.CreateFromTask(
                () => _platformService.CopyTextToClipboard(Url)
            );

            Open = ReactiveCommand.CreateFromTask(DoOpen,
                this.WhenAnyValue(x => x.Url).Select(x => Uri
                    .IsWellFormedUriString(x, UriKind.Absolute)));
        }

        private async Task DoDelete() 
        {
            if (!await DeleteRequest.Handle(Unit.Default)) return;
            _messageBus.SendMessage(new ChannelDeleteEvent(_channel, this));
        }

        private async Task DoOpen()
        {
            var url = new Uri(Url);
            var builder = new UriBuilder(url) { Fragment = string.Empty };
            await _platformService.LaunchUri(builder.Uri);
        }
    }
}