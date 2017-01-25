using CryptoKnight.Client.Core.Plugin;
using GalaSoft.MvvmLight;

namespace CryptoKnight.Client.UI.ViewModel
{
    public class PluginViewModel : ViewModelBase
    {
        public IPlugin Plugin { get; }

        public string Name => Plugin.Name;

        public PluginViewModel(IPlugin plugin)
        {
            Plugin = plugin;
        }
    }
}
