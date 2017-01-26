using CryptoKnight.Library.Network;
using CryptoKnight.Library.Network.ProtocolMessages.Common;
using CryptoKnight.Library.Network.ProtocolMessages.Server;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CryptoKnight.Client.UI.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        // TODO: remove default user 
        // default user is used to not have to type the mail / password
        // every time 
        private static readonly User DefaultUser = new User
        {
            Email = "user@host.com",
            PasswordHash = "user"
        };

        // default endpoint (can also be removed later on)
        private const string Localhost = "127.0.0.1";
        private const int Port = 1991;
        private static readonly IPEndPoint DefaultEndpoint = new IPEndPoint(IPAddress.Parse(Localhost), Port);


        private readonly Encoding _encoding = Encoding.Unicode;

        public UserViewModel User { get; }

        public KeyViewModel Key { get; }

        public IPEndPointViewModel EndPoint { get; }

        private string _connectionStatus;

        public string ConnectionStatus {
            get { return _connectionStatus; }
            set { Set(ref _connectionStatus, value); }
        }

        private string _password;
        public string Password {
            get { return _password; }
            set { Set(ref _password, value); }
        }

        private string _originalText;
        public string OriginalText {
            get { return _originalText; }
            set { Set(ref _originalText, value); }
        }

        private string _convertedText;
        public string ConvertedText {
            get { return _convertedText; }
            set { Set(ref _convertedText, value); }
        }

        private bool _connected;
        public bool Connected {
            get { return _connected; }
            set { Set(ref _connected, value); }
        }

        public PluginViewModel SelectedPlugin { get; set; }

        public ObservableCollection<PluginViewModel> Plugins { get; set; }

        public RelayCommand ConnectCommand { get; }

        public RelayCommand DisconnectCommand { get; }

        public RelayCommand EncryptCommand { get; }

        public RelayCommand DecryptCommand { get; }

        private readonly LicenseClient _licenseClient;


        public MainViewModel()
        {
            User = new UserViewModel(DefaultUser);
            Key = new KeyViewModel();
            EndPoint = new IPEndPointViewModel(DefaultEndpoint);
            Connected = false;

            ConnectCommand = new RelayCommand(async () => await Task.Run(() => Connect()));
            DisconnectCommand = new RelayCommand(async () => await Task.Run(() => Disconnect()));
            EncryptCommand = new RelayCommand(async () => await Task.Run(() => EncryptText()));
            DecryptCommand = new RelayCommand(async () => await Task.Run(() => DecryptText()));

            _licenseClient = new LicenseClient(EndPoint.EndPoint);
            _licenseClient.Connected += LicenseClientOnConnected;
            _licenseClient.Disconnected += LicenseClientOnDisconnected;
            _licenseClient.LoginResponse += OnLoginResponse;

            Plugins = new ObservableCollection<PluginViewModel>();
            foreach (var plugin in PluginFactory.Get().ToList())
            {
                Plugins.Add(new PluginViewModel(plugin));
            }
        }

        private void Connect()
        {
            ConnectionStatus = Properties.Resources.Connecting;
            _licenseClient.Start();
        }

        private void Disconnect()
        {
            Connected = false;
            ConnectionStatus = Properties.Resources.Disconnected;
            _licenseClient.Stop();
        }

        private void EncryptText()
        {
            if (SelectedPlugin?.Plugin == null || OriginalText == null) return;
            var plugin = SelectedPlugin.Plugin;
            var data = plugin.Encrypt(OriginalText, Password);
            if (data == null) return;
            ConvertedText = Convert.ToBase64String(data);
        }

        private void DecryptText()
        {
            if (SelectedPlugin?.Plugin == null || OriginalText == null) return;
            var plugin = SelectedPlugin.Plugin;
            try
            {
                var data = plugin.Decrypt(Convert.FromBase64String(OriginalText), Password);
                ConvertedText = data;
            }
            catch (Exception)
            {
                // invalid string size
                // "Convert.FromBase64String" throws exception if
                // the string does not have a valid base64 length
            }
        }

        private void LicenseClientOnDisconnected(TcpSocket server)
        {
            Disconnect();
        }

        private void LicenseClientOnConnected(TcpSocket server)
        {
            Connected = true;
            ConnectionStatus = Properties.Resources.Connected;
            // TODO: change once the server persistence is done
            // for testing purposes (till the persistence is done) we request
            // a new key everytime we connect (login will fail after 3 connects
            // if the server was not restarted since the keystore will be empty)
            // in the final version this will be:
            _licenseClient.Login(User.User, Key.Key);
        }

        private void OnLoginResponse(LoginResponseMessage message)
        {
            ConnectionStatus = message.LoggedIn
                ? Properties.Resources.LicenseVerified
                : Properties.Resources.LicenseDenied;
        }
        
    }
}