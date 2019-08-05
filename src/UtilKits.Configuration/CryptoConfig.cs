using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace UtilKits.Configuration
{
    public class CryptoConfig
    {
        public string HashKey { get; set; }
        public bool EncryptedConnString { get; set; }
    }

    public static class CryptoConfigExtension
    {
        public static void BindConfig(this IConfiguration configuration, CryptoConfig config)
        {
            ConfigurationBinder.Bind(
                configuration.GetSection("CryptoConfig"),
                config);
        }
    }
}
