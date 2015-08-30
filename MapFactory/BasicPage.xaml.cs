using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace MapFactory
{
    public partial class BasicPage : PhoneApplicationPage
    {
        private Geoposition _geoposition;
        private PositionStatus _positionStatus;

        private double _longitude;
        private double _latitude;
        private double _altitude;

        private bool _isTrackingStarted = false;

        public BasicPage()
        {
            InitializeComponent();

            //_geolocator = new Geolocator();
            //_geolocator.ReportInterval = 2000;
            //_geolocator.MovementThreshold = 1;
            //_geolocator.DesiredAccuracyInMeters = 1;

            //this.textBlockStateInfo.Text = "Status: " + _geolocator.LocationStatus.ToString();
            
            //_geolocator.PositionChanged += OnPositionChanged;
            //_geolocator.StatusChanged += OnStatusChanged;

            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (App.Geolocator == null)
            {
                App.Geolocator = new Geolocator();
                App.Geolocator.DesiredAccuracy = PositionAccuracy.High;
                App.Geolocator.MovementThreshold = 1; // The units are meters.
                App.Geolocator.PositionChanged += OnPositionChanged;
                App.Geolocator.StatusChanged += OnStatusChanged;
            }

            this.textBlockStateInfo.Text = "Status: " + App.Geolocator.LocationStatus.ToString();
        }

        protected override void OnRemovedFromJournal(System.Windows.Navigation.JournalEntryRemovedEventArgs e)
        {
            App.Geolocator.PositionChanged -= OnPositionChanged;
            App.Geolocator = null;
        }

        private void buttonStartStopTracking_Click(object sender, RoutedEventArgs e)
        {
            if (this._isTrackingStarted == false)
            {
                this._isTrackingStarted = true;
                this.buttonStartStopTracking.Content = "Stop tracking";
                this.buttonManageData.IsEnabled = false;
            }
            else
            {
                this._isTrackingStarted = false;
                this.buttonStartStopTracking.Content = "Start tracking";
                this.buttonManageData.IsEnabled = true;
            }
        }

        private void buttonAddObject_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddObjectPage.xaml", UriKind.Relative));
        }

        private void buttonManageData_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ManageDataPage.xaml", UriKind.Relative));
        }


        private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke( () =>
            {
                _geoposition = e.Position;
                UpdateData(_geoposition);
            });
        }

        private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                _positionStatus = e.Status;
                this.textBlockStateInfo.Text = "Status: " + _positionStatus.ToString();

                if (_positionStatus == PositionStatus.Ready)
                {
                    this.buttonStartStopTracking.IsEnabled = true;
                    this.buttonAddObject.IsEnabled = true;
                }
                else
                {
                    this.buttonStartStopTracking.IsEnabled = false;
                    this.buttonAddObject.IsEnabled = false;
                }
            });
        }

        private void UpdateData(Geoposition geoposition)
        {
            _longitude = (double)geoposition.Coordinate.Longitude;
            _latitude = (double)geoposition.Coordinate.Latitude;
            _altitude = (double)geoposition.Coordinate.Altitude;

            this.textBlockLongitude.Text = "Longitude: " + _longitude.ToString();
            this.textBlockLatitude.Text = "Latitude: " + _latitude.ToString();
            this.textBlockAltitude.Text = "Altitude: " + _altitude.ToString();

            //_mapIconPosition.Location = geoposition.Coordinate.Point;

            //this.map.Center = geoposition.Coordinate.Point;
            //this.map.ZoomLevel = 15;
            //this.map.LandmarksVisible = true;
            //this.map.PedestrianFeaturesVisible = true;

            if (this._isTrackingStarted == true)
            {
                WritePositionToFile();
            }
        }

        private async Task WritePositionToFile()
        {
            string positionString = _longitude.ToString() + " " + _latitude.ToString() + " " + _altitude.ToString() + "\r\n";
            byte[] positionBytes = System.Text.Encoding.UTF8.GetBytes(positionString);

            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            StorageFile storageFile = await storageFolder.CreateFileAsync("tracking.dat", CreationCollisionOption.OpenIfExists);

            //await Windows.Storage.FileIO.AppendTextAsync(storageFile, positionString);

            using (var storageFileStream = await storageFile.OpenStreamForWriteAsync())
            {
                storageFileStream.Seek(0, SeekOrigin.End);
                await storageFileStream.WriteAsync(positionBytes, 0, positionBytes.Length);
            }
        }
    }
}