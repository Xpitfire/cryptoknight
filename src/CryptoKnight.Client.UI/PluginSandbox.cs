using CryptoKnight.Client.Core.Plugin;
using CryptoKnight.Server.Core;
using System;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace CryptoKnight.Client.UI
{
    public class PluginSandbox
    {
        private const string SandboxDomainName = "PluginSandboxDomain";
        private static readonly AppDomain SandboxDomain;

        static PluginSandbox()
        {
            var permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            var ptInfo = new AppDomainSetup
            {
                ApplicationBase = "."
            };

            var strongName = typeof(IPlugin).Assembly.Evidence.GetHostEvidence<StrongName>();

            SandboxDomain = AppDomain.CreateDomain(
                SandboxDomainName,
                AppDomain.CurrentDomain.Evidence,
                ptInfo,
                permSet,
                strongName);
        }

        public static IPlugin LoadAddIn(byte[] addIn, string password)
        {
            var assembly = SandboxDomain.Load(DataProtection.Decrypt(addIn, password));
            foreach (var type in assembly.GetTypes())
            {
                if (!type.GetInterfaces().Contains(typeof(IPlugin))) continue;
                return assembly.CreateInstance(type.FullName) as IPlugin;
            }
            return null;
        }
    }
}
