using CryptoKnight.Library.Network.ProtocolMessages.Common;
using GalaSoft.MvvmLight;
using CryptoKnight.Library.Network;

namespace CryptoKnight.Client.UI.ViewModel
{
    public class UserViewModel : ViewModelBase
    {
        public User User { get; set; }

        private string _dummy;

        public string Email {
            get { return User.Email; }
            set {
                User.Email = value;
                Set(ref _dummy, value);
            }
        }

        private string _password;
        public string Password {
            get { return _password; }
            set {
                User.PasswordHash = value?.ComputeMd5Hash();
                Set(ref _password, value);
            }
        }

        public UserViewModel(User user = null)
        {
            User = user ?? new User();
        }


    }
}
