using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace UtilKits.Configuration
{
    public static class ConfigurationHelper
    {
        public static IConfiguration Config;

        public static IConfiguration BindConfigurationHelper(this IConfiguration configuration)
        {
            Config = configuration;

            return configuration;
        }

        /// <summary>
        /// TestProject 取得預設的 appsettings.json 內容
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetConfig()
        {
            string rootPath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;

            return GetConfig(rootPath, "appsettings.json");
        }

        /// <summary>
        /// 取得指定的 appsettings 內容
        /// </summary>
        /// <param name="path">位置</param>
        /// <param name="json">檔名</param>
        /// <returns></returns>
        public static IConfiguration GetConfig(string path, string json)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile(json, optional: false, reloadOnChange: true);

            return builder.Build();
        }

        /// <summary>
        /// 依Section Name 建置指定的泛型物件
        /// </summary>
        /// <typeparam name="T">指定的泛型</typeparam>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static T GenerateConfig<T>(string sectionName) where T : new()
        {
            T result = new T();

            ConfigurationBinder.Bind(
                Config.GetSection(sectionName),
                result);

            return result;
        }

    }
}
