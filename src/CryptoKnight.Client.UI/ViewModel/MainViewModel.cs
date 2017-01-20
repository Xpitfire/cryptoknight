using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoKnight.Client.Core.Plugin;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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
        private IPlugin EncryptionPlugin { get; set; }
        private byte[] EncryptedData { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            OpenButtonName = Properties.Resources.OpenButton;
            OpenLabelName = Properties.Resources.SelectedFile;
            OriginalTextLabel = Properties.Resources.OriginalText;
            ConvertedTextLabel = Properties.Resources.ConvertedText;
            EncryptButtonName = Properties.Resources.Encrypt;
            DecryptButtonName = Properties.Resources.Decrypt;

            OpenCommand = new RelayCommand(OpenFileSelectionDialogAsync);
            EncryptCommand = new RelayCommand(EncryptTextAsync);
            DecryptCommand = new RelayCommand(DecryptTextAsync);
            EncryptionPlugin = PluginFactory.Get().First();

            Reset();
        }

        private async void EncryptTextAsync()
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(OriginalText)) return;
                EncryptedData = string.IsNullOrEmpty(ConvertedText)
                    ? EncryptionPlugin.Encrypt(OriginalText)
                    : EncryptionPlugin.Encrypt(ConvertedText);
                ConvertedText = Encoding.UTF8.GetString(EncryptedData);
            });
        }

        private async void DecryptTextAsync()
        {
            await Task.Run(() =>
            {
                //if (OriginalText == null || OriginalText.Length <= 0) return;
                if (EncryptedData == null || EncryptedData.Length <= 0) return;
                ConvertedText = EncryptionPlugin.Decrypt(EncryptedData);
            });
        }

        private void Reset()
        {
            SelectedFileLabel = string.Empty;
            OriginalText = string.Empty;
            ConvertedText = string.Empty;
            EncryptedData = null;
        }

        private async void OpenFileSelectionDialogAsync()
        {
            await Task.Run(() =>
            {
                var fileDialog = new Microsoft.Win32.OpenFileDialog {Filter = "Text Files (*.txt)|*.txt"};
                var result = fileDialog.ShowDialog();
                if (result == false) return;
                SelectedFileLabel = fileDialog.FileName;
                OriginalText = File.ReadAllText(SelectedFileLabel);
            });
        }

        public string OpenButtonName { get; private set; }
        public string OpenLabelName { get; private set; }
        public string EncryptButtonName { get; private set; }
        public string DecryptButtonName { get; private set; }

        private string _originalText;
        public string OriginalText
        {
            get { return _originalText; }
            set { Set(ref _originalText, value); }
        }
        private string _convertedText;
        public string ConvertedText {
            get { return _convertedText; }
            set { Set(ref _convertedText, value); }
        }

        public string OriginalTextLabel { get; private set; }
        public string ConvertedTextLabel { get; private set; }

        private string _selectedFileLabel;
        public string SelectedFileLabel
        {
            get { return _selectedFileLabel; }
            set { Set(ref _selectedFileLabel, value); }
        }


        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand EncryptCommand { get; private set; }
        public RelayCommand DecryptCommand { get; private set; }
    }
}