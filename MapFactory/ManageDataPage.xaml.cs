using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Windows.Storage;
using System.Threading.Tasks;
using System.IO;
using OxyPlot;
using OxyPlot.Series;
using System.Globalization;
using System.Windows.Threading;
//using Windows.ApplicationModel;

namespace MapFactory
{
    public partial class ManageDataPage : PhoneApplicationPage
    {
        public static double[] longitude;
        public static double[] latitude;

        public ManageDataPage()
        {
            InitializeComponent();
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            bool isFileTracking = false;
            bool isFileObjects = false;
            try
            {
                StorageFile storageFileTracking = await storageFolder.GetFileAsync("tracking.dat");
                isFileTracking = true;
            }
            catch
            {}

            try
            {
                StorageFile storageFileObjects = await storageFolder.GetFileAsync("objects.dat");
                isFileObjects = true;
            }
            catch
            {}
            
            if ( (isFileTracking == true) || (isFileObjects == true))
            {
                this.textBlockStatus.Text = "database is filled with data";
            }
            else
            {
                this.textBlockStatus.Text = "database is empty";
            }

            GenerateMap();
        }

        

        async private void buttonSendData_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.Subject = "MapFactory data";
            emailComposeTask.Body = "Thanks for using MapFactory \r\n \r\n";

            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            try
            {
                var file = await storageFolder.OpenStreamForReadAsync("tracking.dat");

                emailComposeTask.Body += "Trails:\r\n";
                using (StreamReader streamReader = new StreamReader(file))
                {
                    emailComposeTask.Body += streamReader.ReadToEnd();
                }

                
            }
            catch (System.IO.FileNotFoundException)
            {
                this.textBlockStatus.Text = "cannot send empty tracking database";
            }

            emailComposeTask.Body += "\r\n";

            try
            {
                var file = await storageFolder.OpenStreamForReadAsync("objects.dat");

                emailComposeTask.Body += "Objects:\r\n";
                using (StreamReader streamReader = new StreamReader(file))
                {
                    emailComposeTask.Body += streamReader.ReadToEnd();
                }

                await ClearData();
            }
            catch (System.IO.FileNotFoundException)
            {
                this.textBlockStatus.Text = "cannot send empty objects database";
            }

            emailComposeTask.Show();

            await ClearData();
        }

        async private void buttonClearData_Click(object sender, RoutedEventArgs e)
        {
            await ClearData();
        }

        async private Task ClearData()
        {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            StorageFile storageFileTracking = await storageFolder.CreateFileAsync("tracking.dat", CreationCollisionOption.OpenIfExists);
            await storageFileTracking.DeleteAsync();

            StorageFile storageFileObjects = await storageFolder.CreateFileAsync("objects.dat", CreationCollisionOption.OpenIfExists);
            await storageFileObjects.DeleteAsync();

            this.textBlockStatus.Text = "database is empty";
        }

        async private void GenerateMap()
        {
            string trackingDataString = "";

            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            var file = await storageFolder.OpenStreamForReadAsync("tracking.dat");

            using (StreamReader streamReader = new StreamReader(file))
            {
                trackingDataString += streamReader.ReadToEnd();
            }

            string[] trackingDataLinesString = trackingDataString.Trim().Split('\n');

            double[] longitude = new double[trackingDataLinesString.Length];
            double[] latitude = new double[trackingDataLinesString.Length];

            for (int i = 0; i < trackingDataLinesString.Length; i++)
            {
                longitude[i] = Convert.ToDouble(trackingDataLinesString[i].Split()[0]);
                latitude[i] = Convert.ToDouble(trackingDataLinesString[i].Split()[1]);
            }

            PlotModel plotModel = new PlotModel();

            var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 3, MarkerFill = OxyColor.Parse("#FF573110") };

            //plotModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, longitude[0].ToString())); 

            try
            {
                for (int i = 0; i < latitude.Length; i++)
                {
                    scatterSeries.Points.Add(new ScatterPoint(longitude[i], latitude[i]));
                }

                plotModel.Series.Add(scatterSeries);
            }
            catch (Exception ex)
            {
                plotModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, ex.Message));
            }

            this.plot.Model = plotModel;

        }

    }


}