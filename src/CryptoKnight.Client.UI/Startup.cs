﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CryptoKnight.Client.Core.Plugin;

namespace CryptoKnight.Client.UI
{
    class Startup
    {
        static IPlugin LoadAddIn(string assemblyName, AppDomain sandboxDomain)
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

        public static void Start()
        {
            var permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            AppDomainSetup ptInfo = new AppDomainSetup
            {
                ApplicationBase = "."
            };

            var strongName = typeof(IPlugin).Assembly.Evidence.GetHostEvidence<StrongName>();

            var sandboxDomain = AppDomain.CreateDomain("SandboxDomain",
                AppDomain.CurrentDomain.Evidence,
                ptInfo,
                permSet,
                strongName);

            foreach (var file in new DirectoryInfo("./Plugin").GetFiles("*.dll"))
            {
                var assemblyName = file.Name.Replace(".dll", "");
                Console.WriteLine($@"Loaded DLL: {assemblyName}");
                var addIn = LoadAddIn(assemblyName, sandboxDomain);
                if (addIn == null) continue;

                Console.WriteLine($@"{file.Name}: {addIn.Decrypt(addIn.Encrypt("Blob"))}");
            }

        }
    }
}