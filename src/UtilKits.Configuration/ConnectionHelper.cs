using System;
using Microsoft.Extensions.Configuration;
using UtilKits.Crypto;

namespace UtilKits.Configuration
{
    public static class ConnectionHelper
    {
        /// <summary>
        /// 緩存設定加密設定檔
        /// </summary>
        private static Lazy<CryptoConfig> _cryptoConfig = new Lazy<CryptoConfig>(() =>
        {
            var config = new CryptoConfig();
            ConfigurationHelper.Config.BindConfig(config);

            return config;
        });

        /// <summary>
        /// 加密設定檔
        /// </summary>
        public static CryptoConfig Crypto => _cryptoConfig.Value;

        /// <summary>
        /// 取得資料庫連線字串
        /// </summary>
        /// <param name="name">連線字串名稱</param>
        /// <returns>資料庫連線字串</returns>
        public static string GetConnectionString(string name)
        {
            if (ConfigurationHelper.Config == null)
                throw new ArgumentNullException("Configuration", "Do not load appSettings.json.");

            var connectionString = ConfigurationHelper.Config.GetConnectionString(name);

            if (!string.IsNullOrEmpty(connectionString) && Crypto.EncryptedConnString)
            {
                var hashKey = Crypto.HashKey;

                if (string.IsNullOrEmpty(hashKey))
                    throw new ArgumentNullException("HashKey", "HashKey is not setting.");

                connectionString = Cryptographer.Custom.Decrypt(connectionString, hashKey);
            }

            return connectionString;
        }
    }
}
