using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace UtilKits.Database
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// 將Dapper讀出來的日期全部都使用UTC時間
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection AddDapperDateTimeUtc(this IServiceCollection service)
        {
            SqlMapper.AddTypeHandler(new SqlTypeDateTimeUtcHandler());

            return service;
        }
    }
}
