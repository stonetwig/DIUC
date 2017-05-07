using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Protection.PlayReady;
using Windows.Media.Render;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using DIUC.Delegates;

namespace DIUC.ViewModels
{
    internal class MainPageViewModel : ViewModelBase
    {
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
                    Play(_selectedChannel).Wait();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    throw new Exception("Something got fucked up");
                }
            }
        }

        private async Task Play(Channel channel)
        {
            if (_audioGraph == null)
            {
                if (SelectedDevice == null)
                {
                    SelectedDevice = Devices[0];
                }
                var settings = new AudioGraphSettings(AudioRenderCategory.Media) {PrimaryRenderDevice = SelectedDevice};
                var createResult = await AudioGraph.CreateAsync(settings);
                if (createResult.Status != AudioGraphCreationStatus.Success) throw new NotImplementedException();
                _audioGraph = createResult.Graph;
            }

            if (_deviceOutputNode == null)
            {
                var deviceResult = await _audioGraph.CreateDeviceOutputNodeAsync();
                if (deviceResult.Status != AudioDeviceNodeCreationStatus.Success) return;
                _deviceOutputNode = deviceResult.DeviceOutputNode;
            }

            //var file = await SelectPlaybackFile();
            //if (file == null) return;
            var soundFile = StorageFile.CreateStreamedFileFromUriAsync("stream.mp3", new Uri(channel.Url + "?" + Settings.PlayerCode), null);
            var streamResult = await _audioGraph.CreateFileInputNodeAsync(soundFile.GetResults());
            if (streamResult.Status != AudioFileNodeCreationStatus.Success)
            {
                throw new NotImplementedException();
            }

            _fileInputNode = streamResult.FileInputNode;
            _fileInputNode.AddOutgoingConnection(_deviceOutputNode);
            _fileInputNode.OutgoingGain = Volume / 100.0;

            _audioGraph.Start();
        }

        private async Task<IStorageFile> SelectPlaybackFile()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.MusicLibrary
            };
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".aac");
            picker.FileTypeFilter.Add(".wav");

            return await picker.PickSingleFileAsync();
            
        }

        private void Resume()
        {
            _audioGraph.Start();
        }

        private void Stop()
        {
            _audioGraph?.Stop();
        }

        public double Volume
        {
            get => _volume;
            set
            {
                if (value.Equals(_volume)) return;
                _volume = value;
                OnPropertyChanged();
                if (_fileInputNode != null)
                {
                    _fileInputNode.OutgoingGain = value / 100.0;
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
        private AudioGraph _audioGraph;
        private double _volume;
        private AudioFileInputNode _fileInputNode;
        private Channel _selectedChannel;
        private AudioDeviceOutputNode _deviceOutputNode;
        private AudioFileInputNode _streamInputNode;
    }
}
