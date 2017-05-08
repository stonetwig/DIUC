using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Core;
using Windows.Media.Playback;
using DIUC.Delegates;

namespace DIUC.ViewModels
{
    internal class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            ResumeCommand = new DelegateCommand(Resume);
            StopCommand = new DelegateCommand(Stop);
            Settings = new Settings();
        }

        public Settings Settings { get; }

        public DelegateCommand ResumeCommand { get; }
        public DelegateCommand StopCommand { get; }
        public async Task InitializeAsync()
        {
            var outputDevices = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);
            var devices = outputDevices.Where(d => d.IsEnabled).ToList();
            Devices = new ObservableCollection<DeviceInformation>(devices);
            SelectedDevice = Devices.FirstOrDefault(d => d.IsDefault);
            Volume = 70.0;
        }

        public DeviceInformation SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (_selectedDevice == value) return;
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }

        public Channel SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                if (_selectedChannel == value) return;
                _selectedChannel = value;
                OnPropertyChanged(nameof(SelectedChannel));
                try
                {
                    Play(_selectedChannel);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    throw new Exception("Something got fucked up");
                }
            }
        }

        private void Play(Channel channel)
        {
            var channelUrl = new Uri(channel.Url + "?" + Settings.PlayerCode);
            if (_mediaPlayer == null)
            {
                _mediaPlayer = new MediaPlayer();
            }
            _mediaPlayer.Source = MediaSource.CreateFromUri(channelUrl);
            Resume();
        }

        private void Resume()
        {
            _mediaPlayer?.Play();
        }

        private void Stop()
        {
            _mediaPlayer?.Pause();
        }

        public double Volume
        {
            get => _volume;
            set
            {
                if (value.Equals(_volume)) return;
                _volume = value;
                OnPropertyChanged();
                if (_mediaPlayer != null)
                {
                    _mediaPlayer.Volume = value / 100.0;
                }
            }
        }

        public ObservableCollection<DeviceInformation> Devices { get; set; }
        public ObservableCollection<Channel> Channels { get; set; } = new ObservableCollection<Channel>()
        {
            new Channel() {Name = "Chill & Tropical House", Url = "http://prem2.di.fm:80/chillntropicalhouse_hi"},
            new Channel() {Name = "Indie Dance", Url = "http://prem2.di.fm:80/indiedance_hi"},
            new Channel() {Name = "Progressive", Url = "http://prem2.di.fm:80/progressive_hi"},
            new Channel() {Name = "Electro House", Url = "http://prem2.di.fm:80/electrohouse_hi"},
            new Channel() {Name = "Nightcore", Url = "http://prem2.di.fm:80/nightcore_hi"},
            new Channel() {Name = "Mainstage", Url = "http://prem2.di.fm:80/mainstage_hi"},
        };

        private DeviceInformation _selectedDevice;
        private MediaPlayer _mediaPlayer;
        private double _volume;
        private Channel _selectedChannel;
    }
}
