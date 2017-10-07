using System;
using System.Threading.Tasks;
using myFeed.Services.Abstractions;
using myFeed.ViewModels.Extensions;

namespace myFeed.ViewModels.Implementations
{
    public sealed class SettingsViewModel
    {
        public SettingsViewModel(
            IOpmlService opmlService,
            IDialogService dialogService,
            ISettingsService settingsService,
            IPlatformService platformService,
            ITranslationsService translationsService)
        {
            Theme = new ObservableProperty<string>();
            FontSize = new ObservableProperty<int>();
            LoadImages = new ObservableProperty<bool>();
            NotifyPeriod = new ObservableProperty<int>();
            NeedBanners = new ObservableProperty<bool>();
            ImportOpml = new ObservableCommand(opmlService.ImportOpmlFeeds);
            ExportOpml = new ObservableCommand(opmlService.ExportOpmlFeeds);
            Reset = new ObservableCommand(async () =>
            {
                var response = await dialogService.ShowDialogForConfirmation(
                    translationsService.Resolve("ResetAppNoRestore"),
                    translationsService.Resolve("Notification"));
                if (response) await platformService.ResetApp();
            });
            Load = new ObservableCommand(async () =>
            {
                NotifyPeriod.Value = await settingsService.Get<int>("NotifyPeriod");
                NeedBanners.Value = await settingsService.Get<bool>("NeedBanners"); 
                LoadImages.Value = await settingsService.Get<bool>("LoadImages");
                FontSize.Value = await settingsService.Get<int>("FontSize");
                Theme.Value = await settingsService.Get<string>("Theme");
                
                Subscribe(Theme, "Theme", platformService.RegisterTheme);
                Subscribe(FontSize, "FontSize", o => Task.CompletedTask);
                Subscribe(LoadImages, "LoadImages", o => Task.CompletedTask);
                Subscribe(NeedBanners, "NeedBanners", o => Task.CompletedTask);
                Subscribe(NotifyPeriod, "NotifyPeriod", platformService.RegisterBackgroundTask);
                
                void Subscribe<T>(ObservableProperty<T> prop, string key, Func<T, Task> clb) where T : IConvertible
                {
                    prop.PropertyChanged += async (o, args) =>
                    {
                        await clb.Invoke(prop.Value);
                        await settingsService.Set(key, prop.Value);
                    };
                }
            });
        }

        /// <summary>
        /// Selected theme.
        /// </summary>
        public ObservableProperty<string> Theme { get; }

        /// <summary>
        /// Download images or not?
        /// </summary>
        public ObservableProperty<bool> LoadImages { get; }

        /// <summary>
        /// True if need banners.
        /// </summary>
        public ObservableProperty<bool> NeedBanners { get; }

        /// <summary> 
        /// True if notifications needed. 
        /// </summary>
        public ObservableProperty<int> NotifyPeriod { get; }

        /// <summary>
        /// Selected font size.
        /// </summary>
        public ObservableProperty<int> FontSize { get; }

        /// <summary>
        /// Imports feeds from Opml.
        /// </summary>
        public ObservableCommand ImportOpml { get; }

        /// <summary>
        /// Exports feeds to Opml.
        /// </summary>
        public ObservableCommand ExportOpml { get; }

        /// <summary>
        /// Resets application settings.
        /// </summary>
        public ObservableCommand Reset { get; }

        /// <summary>
        /// Loads settings into UI.
        /// </summary>
        public ObservableCommand Load { get; }
    }
}