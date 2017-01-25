using CryptoKnight.Client.Core.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace CryptoKnight.Client.UI
{
    class PluginFactory
    {

        private const string PluginSandboxDomainName = "PluginSandboxDomain";

        private static IPlugin LoadAddIn(string assemblyName, AppDomain sandboxDomain)
        {
            var assembly = Assembly.Load(assemblyName);
            foreach (var type in assembly.GetTypes())
            {
                if (!type.GetInterfaces().Contains(typeof(IPlugin))) continue;
                return sandboxDomain.CreateInstanceAndUnwrap(
                    assembly.FullName,
                    type.FullName) as IPlugin;
            }
            return null;
        }

        public static IEnumerable<IPlugin> Get()
        {
            var permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            var ptInfo = new AppDomainSetup
            {
                ApplicationBase = "."
            };

            var strongName = typeof(IPlugin).Assembly.Evidence.GetHostEvidence<StrongName>();

            var pluginSandboxDomain = AppDomain.CreateDomain(
                PluginSandboxDomainName,
                AppDomain.CurrentDomain.Evidence,
                ptInfo,
                permSet,
                strongName);

            foreach (var file in new DirectoryInfo(".").GetFiles("*.dll"))
            {
                var assemblyName = file.Name.Replace(".dll", "");
                var plugin = LoadAddIn(assemblyName, pluginSandboxDomain);
                if (plugin == null) continue;
                Debug.WriteLine($@"Loaded DLL: {assemblyName}");
                yield return plugin;
            }
        }
    }
}
