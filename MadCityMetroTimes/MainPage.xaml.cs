﻿using System;
using System.Windows;
using System.Windows.Navigation;
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

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            Loaded += MainPage_Loaded;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var dataContext = new NewMainViewModel();
            using(var db = MadMetroDataContext.NewInstance())
            {
                //var busStops = from bsr in db.BusStopRoutes select bsr.
            }
            this.DataContext = dataContext;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectRoute.xaml", UriKind.Relative));
        }
    }
}