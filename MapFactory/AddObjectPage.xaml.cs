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
            string objectString = "Name: " + this.textBoxName.Text + "\r\n";
            objectString += "Position: " + BasicPage._longitude.ToString() + " " + BasicPage._latitude.ToString() + " " + BasicPage._altitude.ToString() + "\r\n";
            objectString += "Description: " + this.textBoxDescription.Text + "\r\n";
            objectString += "Icon: ";
            if (this.radioButton1.IsChecked == true) objectString += "1";
            else if (this.radioButton2.IsChecked == true) objectString += "2";
            else if (this.radioButton3.IsChecked == true) objectString += "3";
            else if (this.radioButton4.IsChecked == true) objectString += "4";
            else if (this.radioButton5.IsChecked == true) objectString += "5";
            else if (this.radioButton6.IsChecked == true) objectString += "6";
            else if (this.radioButton7.IsChecked == true) objectString += "7";
            else if (this.radioButton8.IsChecked == true) objectString += "8";
            else if (this.radioButton9.IsChecked == true) objectString += "9";
            else objectString += "---";
            objectString += "\r\n\r\n";

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