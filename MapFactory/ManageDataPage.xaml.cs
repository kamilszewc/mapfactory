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
//using Windows.ApplicationModel;

namespace MapFactory
{
    public partial class ManageDataPage : PhoneApplicationPage
    {
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
    }
}