using CryptoKnight.Library.Network.ProtocolMessages.Common;
using GalaSoft.MvvmLight;

namespace CryptoKnight.Client.UI.ViewModel
{
    public class KeyViewModel : ViewModelBase
    {
        public Key Key { get; set; }

        private string _code; // dummy

        public string Code {
            get { return Key.Code; }
            set {
                Key.Code = value;
                Set(ref _code, value);
            }
        }

        public KeyViewModel(Key key = null)
        {
            Key = key ?? new Key();
        }

    }
}
