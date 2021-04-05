using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.ApplicationModel.Background;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Background {
    public sealed class Tasks : XamlRenderingBackgroundTask {

        protected override async void OnRun(IBackgroundTaskInstance taskInstance) {

            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            var rtb = new RenderTargetBitmap();
            try {
                var grid = new Grid();
                grid.Children.Add(new TextBox() { Text = "test" });
                var chart = new RadCartesianChart();


                await rtb.RenderAsync(grid);
                await rtb.RenderAsync(chart);

                var pixelBuffer = await rtb.GetPixelsAsync();
                var pixels = pixelBuffer.ToArray();
                var displayInformation = DisplayInformation.GetForCurrentView();
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("test.png",
                    CreationCollisionOption.ReplaceExisting);
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                         BitmapAlphaMode.Premultiplied,
                                         (uint)rtb.PixelWidth,
                                         (uint)rtb.PixelHeight,
                                         displayInformation.RawDpiX,
                                         displayInformation.RawDpiY,
                                         pixels);
                    await encoder.FlushAsync();
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }

            deferral.Complete();
        }
    }
}
