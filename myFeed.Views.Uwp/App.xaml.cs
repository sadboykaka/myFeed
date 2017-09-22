﻿using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using myFeed.Views.Uwp.Views;

namespace myFeed.Views.Uwp
{
    public sealed partial class App : Application
    {
        public App() => InitializeComponent();

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            UnhandledException += (_, args) => Debug.WriteLine(args.Message);
            if (Window.Current.Content == null) Window.Current.Content = new Frame();
            Window.Current.Activate();

            var frame = (Frame)Window.Current.Content;
            if (frame.Content == null) frame.Navigate(typeof(MenuView));

            /*
            // Register notifications.
            var settings = SettingsService.GetInstance().GetSettings();
            BackgroundTasksManager.RegisterNotifier(settings.NotificationServiceCheckTime);

            // Do stuff for navigation from fav tile.
            switch (e.TileId)
            {
                case "App":
                    break;
                case "Favorites":
                    await Task.Delay(300);
                    if (NavigationPage.NavigationFrame.CurrentSourcePageType != typeof(Fave.FavePage))
                        NavigationPage.NavigationFrame.Navigate(typeof(Fave.FavePage));
                    break;
                default:
                    await Task.Delay(300);
                    if (NavigationPage.NavigationFrame.CurrentSourcePageType != typeof(Fave.FavePage))
                        NavigationPage.NavigationFrame.Navigate(typeof(Fave.FavePage));
                    var articles = await Fave.FaveManager.GetInstance().LoadArticles();
                    var target = articles.FirstOrDefault(i => i.GetModel().GetTileId() == e.TileId);
                    if (target != null)
                        Fave.FavePage.NavigationFrame.Navigate(
                            typeof(ArticlePage), target);
                    break;
            }
            */
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            /*
            if (args.Kind != ActivationKind.ToastNotification) return;
            var arraySplit = ((ToastNotificationActivatedEventArgs)args).Argument.Split(';');
            (var id, var categoryTitle) = (arraySplit[0], arraySplit[1]);

            // Build content frame.
            var rootFrame = await CreateRootFrameAsync();
            if (rootFrame.Content == null)
                rootFrame.Navigate(typeof(NavigationPage));

            // Navigate NavigationPage to FeedCategoriesPage if it's not present.
            if (NavigationPage.NavigationFrame.CurrentSourcePageType != typeof(FeedCategoriesPage))
                NavigationPage.NavigationFrame.Navigate(typeof(FeedCategoriesPage));

            // Find appropriate category.
            var manager = FeedManager.GetInstance();
            var categories = await manager.ReadCategories();
            var category = categories.Categories.FirstOrDefault(i => i.Title == categoryTitle);
            if (category == null) return;

            // Retrieve data for category.
            var orderedFeed = await manager.RetrieveFeedAsync(category);
            var targetItem = orderedFeed.FirstOrDefault(i => i.GetModel().GetTileId() == id);
            if (targetItem == null) return;

            // Navigate and mark as read.
            FeedCategoriesPage.NavigationFrame.Navigate(
                typeof(ArticlePage), targetItem);
            targetItem.MarkAsRead();
            */
        }
    }
}