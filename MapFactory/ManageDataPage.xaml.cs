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

            try
            {
                StorageFile storageFile = await storageFolder.GetFileAsync("tracking.dat");
                this.textBlockStatus.Text = "database is filled with data";
            }
            catch (System.IO.FileNotFoundException)
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

                using (StreamReader streamReader = new StreamReader(file))
                {
                    emailComposeTask.Body += streamReader.ReadToEnd();
                }

            //    StorageFile storageFile = await storageFolder.GetFileAsync("tracking.dat");
            //    var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(storageFile);
            //    EmailAttachment emailAttachment = new EmailAttachment("tracking.dat", stream);
            //    emailMessage.Attachments.Add(emailAttachment);
            //    await EmailManager.ShowComposeNewEmailAsync(emailMessage);

                await ClearData();
            }
            catch (System.IO.FileNotFoundException)
            {
                this.textBlockStatus.Text = "cannot send empty file";
            }
            
            emailComposeTask.Show();

        }

        async private void buttonClearData_Click(object sender, RoutedEventArgs e)
        {
            await ClearData();
        }

        async private Task ClearData()
        {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile storageFile = await storageFolder.CreateFileAsync("tracking.dat", CreationCollisionOption.OpenIfExists);
            await storageFile.DeleteAsync();

            this.textBlockStatus.Text = "database is empty";
        }
    }
}