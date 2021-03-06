﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using CryptoKnight.Server.KeyGenerator;
using System.IO;
using CryptoKnight.Library.Network;
using System;

namespace CryptoKnight.Server.Core
{
    internal static class KeyStore
    {
        public const string KeyTemplate = "kkkk-kkkkkkk-kk-kkkkkkk";
        private const string KeyStoreFileName = "keystore.user";

        private static readonly object sync;
        private static readonly IDictionary<User, LicenseGroup> AvailableLicenseActivations;

        static KeyStore()
        {
            sync = new object();
            if (!File.Exists(KeyStoreFileName))
            {
                AvailableLicenseActivations = new ConcurrentDictionary<User, LicenseGroup>();
            }
            else
            {
                try
                {
                    var protectedData = File.ReadAllBytes(KeyStoreFileName);
                    var bytes = DataProtectionApi.Unprotect(protectedData);
                    AvailableLicenseActivations = bytes.ToType<IDictionary<User, LicenseGroup>>();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }

        internal static bool Contains(User user, Key key)
        {
            return AvailableLicenseActivations.ContainsKey(user)
                && AvailableLicenseActivations[user].Keys.Contains(key);
        }

        internal static bool RegisterUser(User user)
        {
            if (AvailableLicenseActivations.Keys.Contains(user)) return false;
            var licenseGroup = new LicenseGroup();
            var generator = new Generator
            {
                Template = KeyTemplate
            };
            for (var i = 0; i < LicenseGroup.MaxLicenseKeys; i++)
            {
                licenseGroup.Keys.Add(generator.CreateKey());
            }
            AvailableLicenseActivations.Add(user, licenseGroup);
            var protectedData = DataProtectionApi.Protect(AvailableLicenseActivations.ToBytes());

            lock (sync)
            {
                File.WriteAllBytes(KeyStoreFileName, protectedData);
            }
            return true;
        }

        internal static Key RequestLicenseKey(User user)
        {
            if (string.IsNullOrEmpty(user?.Email)
                || string.IsNullOrEmpty(user.PasswordHash)) return null;
            if (!AvailableLicenseActivations.ContainsKey(user)) return null;
            if (AvailableLicenseActivations[user].Available <= 0) return null;
            var index = AvailableLicenseActivations[user].Available -= 1;
            var key = AvailableLicenseActivations[user].Keys[index];
            key.Used = true;
            return key;
        }

        internal static void ShowInfo()
        {
            lock (sync)
            {
                // TODO: remove this code in an productive environment
                foreach (var pair in AvailableLicenseActivations)
                {
                    Console.WriteLine($"User<email>: {pair.Key.Email}");
                    foreach (var key in pair.Value.Keys)
                    {
                        Console.WriteLine($"-- <key>: {key.Code}");
                    }
                }
            }
        }
    }
}
