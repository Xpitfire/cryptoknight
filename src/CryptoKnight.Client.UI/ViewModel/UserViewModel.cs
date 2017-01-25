using CryptoKnight.Library.Network.ProtocolMessages.Common;
using GalaSoft.MvvmLight;

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

        public string PasswordHash {
            get { return User.PasswordHash; }
            set {
                User.PasswordHash = value;
                Set(ref _dummy, value);
            }
        }

        public UserViewModel(User user = null)
        {
            User = user ?? new User();
        }


    }
}
