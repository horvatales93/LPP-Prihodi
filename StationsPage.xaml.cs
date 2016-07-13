using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TrolaInfo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StationsPage : Page
    {
        HttpClient httpClient = new HttpClient();
        private string query;
        private StationUC currentStation;
        private List<string> favorites = ((string[])ApplicationData.Current.LocalSettings.Values["listOfFavorites"]).ToList();

        public StationUC CurrentStation
        {
            get { return currentStation; }
            set
            {
                currentStation = value;
                tblStationName.Text = currentStation.stationName;
                tblStationName.Visibility = Visibility.Visible;
                if (!Favorites.Contains(currentStation.stationName))
                {
                    btnAddToFavorites.Visibility = Visibility.Visible;
                }
                else
                {
                    btnAddToFavorites.Visibility = Visibility.Collapsed;
                }                
            }
        }

        public string Query
        {
            get { return query; }
            set
            {
                query = value;
                queryBox.Text = query;
                sendRequest(query);
            }
        }

        public List<string> Favorites
        {
            get { return favorites; }
            set
            {
                favorites = value;
                ApplicationData.Current.LocalSettings.Values["listOfFavorites"] = favorites.ToArray();
            }
        }

        public StationsPage()
        {
            this.InitializeComponent();

        }

        private async void sendRequest(string query)
        {
            progBarStationsPage.IsIndeterminate = true;
            //setting Accept DefaultRequestHeaders to application/json so it recieves json object
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(String.Format("http://www.trola.si/{0}", query));

                //Throws exception if EnsureSuccessStatusCode returns False
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();

                if (responseString.StartsWith("{\"error"))
                {

                    //creating objects from response string
                    Error DeserializedError = JsonConvert.DeserializeObject<Error>(responseString);

                    ShowError(DeserializedError);
                }
                else
                {

                    Stations deserializedStations = JsonConvert.DeserializeObject<Stations>(responseString);

                    ShowStations(deserializedStations);
                }

            }
            catch (HttpRequestException hre)
            {
                Error err = new Error() { error = "Ni dostopa do podatkov. Preverite internetno povezavo." };
                ShowError(err);
            }
            catch (Exception e)
            {
                Error err = new Error() { error = e.Message };
                ShowError(err);
            }
        }
        private void ShowStations(Stations deserializedStations)
        {
            progBarStationsPage.IsIndeterminate = false;
            btnAddToFavorites.Visibility = Visibility.Visible;
            btnRefresh.Visibility = Visibility.Visible;
            Pivot pvtStations = new Pivot();
            pvtStations.SelectionChanged += PvtStations_SelectionChanged;
            foreach (var station in deserializedStations.stations)
            {
                PivotItem pvtItemStation = new PivotItem() { Header = station.number };
                StationUC stationUC = new StationUC() { MyStation = station };
                pvtItemStation.Content = stationUC;
                pvtStations.Items.Add(pvtItemStation);
            }
            contPresStations.Content = pvtStations;
        }

        private void PvtStations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentStation=(((sender as Pivot).SelectedItem as PivotItem).Content as StationUC);
        }

        private void ShowError(Error er)
        {
            progBarStationsPage.IsIndeterminate = false;
            tblStationName.Visibility = Visibility.Collapsed;
            btnAddToFavorites.Visibility = Visibility.Collapsed;
            btnRefresh.Visibility = Visibility.Collapsed;
            TextBlock err = new TextBlock() { Text = er.error };
            contPresStations.Content = err;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Query = e.Parameter.ToString();
        }

        private void queryBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Query = args.QueryText;
        }

        private void queryBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = StationNamesSource.stationNames.Where(s => s.ToLower().StartsWith(sender.Text.ToLower())).ToList();
            }
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            currentStation.Refreshing = true;
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(String.Format("http://www.trola.si/{0}", CurrentStation.stationNumber));

                //Throws exception if EnsureSuccessStatusCode returns False
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();

                if (responseString.StartsWith("{\"error"))
                {

                    //creating objects from response string
                    Error DeserializedError = JsonConvert.DeserializeObject<Error>(responseString);
                    handleRefreshingError(DeserializedError);
                    
                }
                else
                {
                    Stations deserializedStations = JsonConvert.DeserializeObject<Stations>(responseString);
                    currentStation.MyStation = deserializedStations.stations[0];
                    currentStation.Refreshing = false;
                }

            }
            catch (HttpRequestException hre)
            {
                Error err = new Error() { error = hre.Message };
                handleRefreshingError(err);
            }
            catch (Exception ex)
            {
                Error err = new Error() { error = ex.Message };
                handleRefreshingError(err);
            }
        }

        private bool saveFavorite(string favorite)
        {
            List<string> tempFavorites = Favorites;
            if (tempFavorites.Capacity < 20)
            {
                if (!tempFavorites.Contains(favorite))
                {
                    tempFavorites.Add(favorite);
                    Favorites = tempFavorites;
                    return true;
                }
            }
            return false;
        }

        private void handleRefreshingError(Error err)
        { 
            currentStation.Refreshing = false;
        }
        private void btnShema_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ShemaPage));
        }

        private void btnPriljubljene_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void btnAddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            bool savedSuccessfully = saveFavorite(CurrentStation.stationName);
            if (savedSuccessfully)
            {
                (sender as Button).Visibility = Visibility.Collapsed;
            }
        }
    }
}
