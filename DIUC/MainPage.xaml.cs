using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DIUC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel _viewModel;
        private ListenkeyViewModel _listenkeyViewModel;

        public MainPage()
        {
                this.InitializeComponent();
                this.Loaded += OnLoaded;
        }

        public MainPage(ListenkeyViewModel vm)
        {
            _listenkeyViewModel = vm;
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            _listenkeyViewModel = new ListenkeyViewModel();
            await _listenkeyViewModel.GetKey();
            _viewModel = new MainPageViewModel(_listenkeyViewModel);
            this.DataContext = _viewModel;
            await _viewModel.InitializeAsync();
        }
    }
}
