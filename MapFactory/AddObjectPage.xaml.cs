using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Storage;
using System.Threading.Tasks;
using System.IO;

namespace MapFactory
{
    public partial class AddObjectPage : PhoneApplicationPage
    {
        public AddObjectPage()
        {
            InitializeComponent();
        }

        async private void button_Click(object sender, RoutedEventArgs e)
        {
            string objectString = this.textBoxName.Text;
            objectString += " " + BasicPage._longitude.ToString() + " " + BasicPage._latitude.ToString() + " " + BasicPage._altitude.ToString() + " ";
            objectString += this.textBoxDescription.Text + "\r\n";

            byte[] positionBytes = System.Text.Encoding.UTF8.GetBytes(objectString);

            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            StorageFile storageFile = await storageFolder.CreateFileAsync("objects.dat", CreationCollisionOption.OpenIfExists);

            using (var storageFileStream = await storageFile.OpenStreamForWriteAsync())
            {
                storageFileStream.Seek(0, SeekOrigin.End);
                await storageFileStream.WriteAsync(positionBytes, 0, positionBytes.Length);
            }

            NavigationService.GoBack();
        }
    }
}