using GalaSoft.MvvmLight;
using System.Net;

namespace CryptoKnight.Client.UI.ViewModel
{
    // ReSharper disable once InconsistentNaming
    public class IPEndPointViewModel : ViewModelBase
    {
        public IPEndPoint EndPoint { get; set; }


        public IPEndPointViewModel(IPEndPoint endPoint = null)
        {
            EndPoint = endPoint ?? new IPEndPoint(IPAddress.None, 0);
        }

        private string _address; // dummy

        public string Address {
            get { return EndPoint.Address.ToString(); }
            set {
                EndPoint.Address = IPAddress.Parse(value);
                Set(ref _address, value);
            }
        }

        private int _port; // dummy

        public int Port {
            get { return EndPoint.Port; }
            set {
                EndPoint.Port = value;
                Set(ref _port, value);
            }
        }
    }
}
