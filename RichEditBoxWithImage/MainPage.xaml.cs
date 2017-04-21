using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RichEditBoxWithImage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void save(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we
                // finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);

                // Let Windows know that we're finished changing the file so the
                // other app can update the remote version of the file.
                Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status != Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                    await errorBox.ShowAsync();
                }
            }
        }

        private async void InsertImage(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();


            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))

            {
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                editor.Document.Selection.InsertImage(bitmapImage.PixelWidth, bitmapImage.PixelHeight, 0, VerticalCharacterAlignment.Baseline, "image", fileStream);

            }
        }

        private async void editor_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //if (e.Key == Windows.System.VirtualKey.V && e.Key == Windows.System.VirtualKey.Control)
            //{
                //var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
                //if (dataPackageView.Contains(StandardDataFormats.Bitmap))
                //{
                //IRandomAccessStreamReference imageReceived = null;
                //    try
                //    {
                //    imageReceived = await dataPackageView.GetBitmapAsync();
                //    }
                //    catch (Exception ex)
                //    { }
                //    if (imageReceived != null)
                //    {
                //        using (var imageStream = await imageReceived.OpenReadAsync())
                //        {
                //            var bitmapImage = new BitmapImage();
                //            bitmapImage.SetSource(imageStream);
                //            image.Source = bitmapImage;
                //            editor.Document.Selection.InsertImage((int)bitmapImage.PixelWidth, (int)bitmapImage.PixelHeight, 0, Windows.UI.Text.VerticalCharacterAlignment.Baseline, "Image", imageStream);
                //            e.Handled = true;
                //            Clipboard.Clear();
                //        }
                //    }
                //}
                //else
                //{
                //    //For Exception  
                //}
            //}
        }
    }
}
