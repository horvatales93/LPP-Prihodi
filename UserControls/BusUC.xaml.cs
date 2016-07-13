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
using Windows.UI;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TrolaInfo.UserControls
{
    public sealed partial class BusUC : UserControl
    {
        public BusUC()
        {
            this.InitializeComponent();
        }
        private Bus thisBus;
        private string number;
        private string direction;
        private List<string> arrivals;

        public Bus ThisBus
        {
            get { return thisBus; }
            set
            {
                thisBus = value;
                tblBusNumber.Text = thisBus.number;
                tblBusDirection.Text = thisBus.direction;
                foreach (var arrival in thisBus.arrivals)
                {
                    ContentPresenter cntpArrival = new ContentPresenter();
                    cntpArrival.Margin = new Thickness(0, 0, 5, 0);
                    cntpArrival.Padding = new Thickness(3, 0, 3, 0);
                    cntpArrival.Background = new SolidColorBrush(Colors.DarkGreen);

                    //Color col = (Color)Application.Current.Resources["SystemControlBackgroundAltHighBrush"];
                    //tblarrival.Foreground = new SolidColorBrush(col);

                    TimeSpan ts = TimeSpan.FromMinutes(Convert.ToDouble(arrival));
                    if (Convert.ToInt16(arrival) <= 60)
                    {
                        cntpArrival.Content = ts.ToString("%m") + "min";
                    }
                    else
                    {
                        cntpArrival.Content = (ts.ToString("%h") + "h " + ts.ToString("%m") + "min");
                    }

                    stpArrivals.Children.Add(cntpArrival);
                }
            }
        }

        public string Number
        {
            get { return number; }
            set
            {
                number = value;
                tblBusNumber.Text = number;
            }
        }

        public string Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                tblBusDirection.Text = direction;
            }
        }

        public List<string> Arrivals
        {
            get { return arrivals; }
            set
            {
                arrivals = value;
                foreach (var arrival in arrivals)
                {
                    stpArrivals.Children.Add(new TextBlock() { Text = arrival });
                }
            }
        }
    }
}
