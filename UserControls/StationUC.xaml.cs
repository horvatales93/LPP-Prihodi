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
using System.Net.Http;
using Newtonsoft.Json;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TrolaInfo.UserControls
{
    public sealed partial class StationUC : UserControl
    {
        HttpClient httpClient = new HttpClient();
        public string stationName { get; set; }
        public string stationNumber { get; set; }
        private List<Bus> listBuses;
        private Station myStation;
        private bool refreshing;

        public StationUC()
        {
            this.InitializeComponent();         
        }

        public Station MyStation
        {
            get { return myStation; }
            set
            {
                myStation = value;
                stationName = myStation.name;
                stationNumber = myStation.number;
                listBuses = myStation.buses.Where<Bus>(b => b.arrivals.Capacity > 0).ToList();

                lvBuses.Items.Clear();

                if (listBuses.Capacity > 0)
                {
                    foreach (var bus in listBuses)
                    {
                        BusUC busUC = new BusUC() { ThisBus = bus, Margin = new Thickness(0, 1, 0, 0) };
                        lvBuses.Items.Add(busUC);
                    }
                }
                else
                {
                    TextBlock tblNoBuses = new TextBlock() { Text = "Ni prihodov", FontSize = 15, Padding = new Thickness(5) };
                    lvBuses.Items.Add(tblNoBuses);
                }
            }
        }


        public bool Refreshing
        {
            get { return refreshing; }
            set
            {
                refreshing = value;
                if (refreshing)
                {
                    progRingRefresh.IsActive = true;
                    stpRefresh.Visibility = Visibility.Visible;
                }
                else
                {
                    progRingRefresh.IsActive = false;
                    stpRefresh.Visibility = Visibility.Collapsed;
                }
            }
        }

    }
}
