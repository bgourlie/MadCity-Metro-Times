using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MadCityMetroTimes.Model;
using MadCityMetroTimes.ViewModels;
using Microsoft.Phone.Controls;

namespace MadCityMetroTimes
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var dataContext = new NewMainViewModel();
            using(var db = MadMetroDataContext.NewInstance())
            {
                dataContext.Initialize(db.BusStopRoutes.Where(bsr => bsr.IsTracking));
            }
            DataContext = dataContext;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectRoute.xaml", UriKind.Relative));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = (Pivot) sender;
        }
    }
}