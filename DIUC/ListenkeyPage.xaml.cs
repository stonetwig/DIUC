using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DIUC.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DIUC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListenkeyPage : Page
    {

        private ListenkeyViewModel _viewModel;

        public ListenkeyPage()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void ListenKey_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            var key = textbox.Text;
            if (!string.IsNullOrEmpty(key))
            {
                SetListenKey(key);
            }
        }

        private void SetListenKey(string key)
        {
            _viewModel.Listenkey = key;
            this.Frame.Navigate(typeof(MainPage), _viewModel);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new ListenkeyViewModel();
            this.DataContext = _viewModel;
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            var key = ListenKey.Text;
            if (!string.IsNullOrEmpty(key))
            {
                SetListenKey(key);
            }
        }
    }
}
