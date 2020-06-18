using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;



namespace BebeSBaba
{
    /// <summary>
    ///на разработку данного приложения ушло меньше 20 минут
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void OpenShitBtn_Click(object sender, RoutedEventArgs e)
        {
            //выбор файла
            Windows.Storage.Pickers.FileOpenPicker open =
                new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".rtf");

            // собсно открытие файла
            Windows.Storage.StorageFile file = await open.PickSingleFileAsync();

            if (file != null)//не даст словить NRE и похожую дичь
            {
                using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // грузит содержимое файла в нашу коробычку
                    ShitEditor.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, randAccStream);
                }
            }
            else //если дичь таки произошла
            {
                Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("Начальника, всё сломався!");
                await errorBox.ShowAsync();
            }
        }

        private async void SaveShitBtn_Click(object sender, RoutedEventArgs e)
        {
            //код ниже можно пихнуть в try-catch но делять я это не буду ибо х#й потом это отладишь

            //диалоговое окно сохранения файла
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });
            savePicker.SuggestedFileName = "New Shit";//дефолтное имя

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)//не даст словить NRE и похожую дичь
            {
                CachedFileManager.DeferUpdates(file);
                using (Windows.Storage.Streams.IRandomAccessStream randomAccessStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    ShitEditor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randomAccessStream);
                }

                //сообщаем винде что редактирование говна произведено
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status != FileUpdateStatus.Complete)
                {
                    //на случай если произошла х#йня
                    Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("Press F to " + file.Name);
                    await errorBox.ShowAsync();
                }
            }
            else //если дичь таки произошла
            {
                Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("Начальника, всё сломався!");
                await errorBox.ShowAsync();
            }
        }
    }
}