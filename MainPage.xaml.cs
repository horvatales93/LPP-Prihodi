using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TrolaInfo.Classes;
using TrolaInfo.UserControls;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TrolaInfo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<string> favorites;
        
        public List<string> Favorites
        {
            get { return favorites; }
            set
            {
                favorites = value;
                ApplicationData.Current.LocalSettings.Values["listOfFavorites"] = favorites.ToArray();

            }
        }
        public MainPage()
        {
            this.InitializeComponent();

            if (ApplicationData.Current.LocalSettings.Values["listOfFavorites"] != null)
            {
                favorites = ((string[])ApplicationData.Current.LocalSettings.Values["listOfFavorites"]).ToList();
            }
            else
            {
                Favorites = new List<string>() { "BAVARSKI DVOR", "Askerceva" };
            }

            showFavorites();
        }

        private void showFavorites()
        {
            lvPriljubljene.Items.Clear();
            List<string> tempFavorites = Favorites;
            foreach (var favorite in tempFavorites)
            {
                FavoriteUC favoriteItem = new FavoriteUC() { FavoriteName = favorite };
                favoriteItem.deleteFavoriteClicked += OnDeleteFavoriteClicked;
                favoriteItem.queryFavoriteClicked += OnQueryFavoriteClicked;
                lvPriljubljene.Items.Add(favoriteItem);
            }
        }

        private void OnDeleteFavoriteClicked(object sender,FavoriteUC favorite)
        {
            List<string> tempFavorites = Favorites;
            if (tempFavorites.Contains(favorite.FavoriteName) && tempFavorites.Count > 1)
            {
                tempFavorites.Remove(favorite.FavoriteName);
                Favorites = tempFavorites;
                lvPriljubljene.Items.Remove(favorite);
            }
        }

        private void OnQueryFavoriteClicked(object sender,FavoriteUC favorite)
        {
            this.Frame.Navigate(typeof(StationsPage), favorite.FavoriteName);
        }

        private void queryBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = StationNamesSource.stationNames.Where(s => s.ToLower().StartsWith(sender.Text.ToLower())).ToList();
            }
        }



        private void queryBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            this.Frame.Navigate(typeof(StationsPage),args.QueryText);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ShemaPage));
        }
    }
}
