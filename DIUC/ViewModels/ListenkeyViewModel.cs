using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DIUC.ViewModels
{
    public class ListenkeyViewModel : ViewModelBase
    {

        public string Listenkey
        {
            get => _listenkey;
            set
            {
                if (_listenkey == value) return;
                _listenkey = value;
                OnPropertyChanged();
                #pragma warning disable 4014
                StoreListenKeyAsync(_listenkey);
                #pragma warning restore 4014
            }
        }

        private string _listenkey;

        public ListenkeyViewModel()
        {
        }

        public async Task GetKey()
        {
            try
            {
                var listenKeyTask = RetrieveListenKey();
                _listenkey = await listenKeyTask;
            }
            catch (Exception e)
            {
            }
        }

        public ListenkeyViewModel(string key)
        {
            _listenkey = key;
        }

        public static async Task StoreListenKeyAsync(string key)
        {
                var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var listenkeyFile = await storageFolder.CreateFileAsync("listenkey.txt",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(listenkeyFile, key);
        }

        public static async Task<string> RetrieveListenKey()
        {
            try
            {
                var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var listenkeyFile = await storageFolder.GetFileAsync("listenkey.txt");
                return await Windows.Storage.FileIO.ReadTextAsync(listenkeyFile);
            }
            catch (Exception e)
            {
                throw new Exception("Could not load file");
            }
        }

        public static async Task RemoveListenKeyFile()
        {
            try
            {
                var listenkeyFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("listenkey.txt");
                await listenkeyFile.DeleteAsync(StorageDeleteOption.Default);
            }
            catch (Exception e)
            {
                throw new Exception("The file trying to be deleted does either not exist or cannot be deleted: " + e);
            }
        }

        public bool IsListenKeySet()
        {
            return !string.IsNullOrEmpty(Listenkey);
        }
    }
}
