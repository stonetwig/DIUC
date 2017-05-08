using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIUC.ViewModels
{
    class Settings : ViewModelBase
    {
        public string PlayerCode
        {
            get => _playerCode;
            set
            {
                if (_playerCode == value) return;
                _playerCode = value;
                OnPropertyChanged();
                SaveToSettingsFile(nameof(PlayerCode), value);
            }
        }

        private static void SaveToSettingsFile(string playerCodeName, string value)
        {
            
        }

        private string _playerCode;
    }
}
