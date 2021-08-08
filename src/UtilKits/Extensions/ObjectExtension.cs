using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UtilKits.Serializer;

namespace UtilKits.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 將物件轉換為Json字串
        /// </summary>
        /// <param name="source">要轉換的物件</param>
        /// <returns>Json 字串</returns>
        public static string ToJson<T>(this T source) where T : class
        {
            return JsonHelper.Instance.Serialize(source);
        }

        /// <summary>
        /// 將物件轉換為XML
        /// </summary>
        /// <param name="source">要轉換的物件</param>
        /// <returns>XML</returns>
        public static string ToXml<T>(this T source) where T : class
        {
            return XmlHelper.Instance.Serialize(source);
        }

        /// <summary>
        /// 將物件轉換為XML
        /// </summary>
        /// <param name="source">要轉換的物件</param>
        /// <param name="namespaceUrl">Namespace Url</param>
        /// <returns>XML</returns>
        public static string ToXml<T>(this T source, string namespaceUrl) where T : class
        {
            return XmlHelper.Instance.Serialize(source, namespaceUrl);
        }

        /// <summary>
        /// 將物件轉換為Dictionary
        /// </summary>
        /// <param name="source">要轉換的物件</param>
        /// <returns></returns>
        public static IDictionary<string, string> ToStringDictionary<T>(this T source)
        {
            return source.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(source)?.ToString());
        }

        /// <summary>
        /// 將指定的System.Object強制轉換為Int32，Exception回傳0
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Int32</returns>
        public static int ToInt(this object source, int defaultValue = 0)
        {
            try
            {
                int result = defaultValue;
                if (!(source == DBNull.Value || source == null))
                    result = Convert.ToInt32(source);
                return result;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 將指定的System.Object強制轉換為Int32，Exception回傳0
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Int32</returns>
        public static short ToShort(this object source, short defaultValue = 0)
        {
            try
            {
                short result = defaultValue;
                if (!(source == DBNull.Value || source == null))
                    result = Convert.ToInt16(source);
                return result;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 將指定的System.Object強制轉換為Int32，Exception回傳0
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Int32</returns>
        public static byte ToByte(this object source, byte defaultValue = 0)
        {
            try
            {
                byte result = defaultValue;
                if (!(source == DBNull.Value || source == null))
                    result = Convert.ToByte(source);
                return result;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 將指定的System.Object強制轉換為Long
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Long</returns>
        public static long ToLong(this object source, long defaultValue = 0)
        {
            long result = defaultValue;
            long.TryParse(source.ToString(), out result);
            return result;
        }

        /// <summary>
        /// 將指定的System.Object轉換為byte?
        /// </summary>
        /// <param name="source">System.Objec</param>
        /// <returns>byte?</returns>
        public static byte? ParseByte(this object source, byte? defaultValue = null)
        {
            byte? result = defaultValue;
            if (!(source == DBNull.Value || source == null))
                result = Convert.ToByte(source);
            return result;
        }

        /// <summary>
        /// 將指定的System.Object轉換為Int16?
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Int16?</returns>
        public static short? ParseShort(this object source, short? defaultValue = null)
        {
            short? result = defaultValue;
            if (!(source == DBNull.Value || source == null))
                result = Convert.ToInt16(source);
            return result;
        }

        /// <summary>
        /// 將指定的System.Object轉換為Int32?
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Int32?</returns>
        public static int? ParseInt(this object source, int? defaultValue = null)
        {
            int? result = defaultValue;
            if (!(source == DBNull.Value || source == null))
                result = Convert.ToInt32(source);
            return result;
        }

        /// <summary>
        /// 將指定的System.Object轉換為Int64?
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Int64?</returns>
        public static long? ParseLong(this object source, long? defaultValue = null)
        {
            long? result = defaultValue;
            if (!(source == DBNull.Value || source == null))
                result = Convert.ToInt64(source);
            return result;
        }

        /// <summary>
        /// 將指定的System.Object強制轉換為Boolean，Exception傳回false
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Boolean</returns>
        public static bool ToBoolean(this object source, bool defaultValue = false)
        {
            try
            {
                bool result = defaultValue;
                if (!(source == null || source == DBNull.Value))
                    result = source.ToString().IsNumber()
                        ? (source.ToString() == "1")
                        : Convert.ToBoolean(source);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 將指定的System.Object轉換為Boolean?
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Boolean?</returns>
        public static bool? ParseBoolean(this object source, bool? defaultValue = null)
        {
            bool? result = defaultValue;
            if (!(source == DBNull.Value || source == null))
                result = source.ToString().IsNumber()
                        ? (source.ToString() == "1")
                        : Convert.ToBoolean(source);
            return result;
        }

        /// <summary>
        /// 將指定的System.Object強制轉換為Decimale，Exception傳回0
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Decimal</returns>
        public static decimal ToDecimal(this object source, decimal defaultValue = 0)
        {
            try
            {
                decimal result = defaultValue;
                if (!(source == DBNull.Value || source == null))
                    result = Convert.ToDecimal(source);
                return result;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 將指定的System.Object強制轉換為Double，Exception傳回0
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Double</returns>
        public static double ToDouble(this object source) => source.ToDouble(0);

        /// <summary>
		/// 將指定的System.Object強制轉換為Double，Exception傳回0
		/// </summary>
		/// <param name="source">System.Object</param>
		/// <returns>Double</returns>
		public static double ToDouble(this object source, double defaultValue)
        {
            try
            {
                double result = defaultValue;
                if (!(source == DBNull.Value || source == null))
                    result = Convert.ToDouble(source);
                return result;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 將指定的System.Object轉換為Decimal?
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Decimal?</returns>
        public static decimal? ParseDecimal(this object source, decimal? defaultValue = null)
        {
            decimal? result = defaultValue;
            if (!(source == DBNull.Value || source == null))
                result = Convert.ToDecimal(source);
            return result;
        }

        /// <summary>
        /// 將指定的System.Object轉換為Double?
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>Double?</returns>
        public static double? ParseDouble(this object source, double? defaultValue = null)
        {
            double? result = defaultValue;
            if (!(source == DBNull.Value || source == null))
                result = Convert.ToDouble(source);
            return result;
        }

        /// <summary>
        /// 將指定的System.Object轉換為DateTime?
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>DateTime?</returns>
        public static DateTime? ParseDateTime(this object source, DateTime? defalutValue = null)
        {
            DateTime? result = defalutValue;
            if (!(source == DBNull.Value || source == null))
                result = Convert.ToDateTime(source);
            return result;
        }

        /// <summary>
        /// 將指定的System.Object轉換為DateTime?
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <param name="format">DateTime format(yyyy-MM-dd)</param>
        /// <returns>DateTime?</returns>
        public static DateTime? ParseDateTime(this object source, string format)
        {
            DateTime result;

            if (!(source == DBNull.Value || source == null) &&
                DateTime.TryParseExact(source.ToString(), format, new CultureInfo("en-US"), DateTimeStyles.None, out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// 將指定的System.Object轉換為Datetime
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>DateTime</returns>
        public static DateTime ToDateTime(this object source)
        {
            DateTime result = DateTime.MinValue;
            DateTime.TryParse(source.ToString(), out result);
            return result;
        }

        /// <summary>
        /// 將指定的System.Object轉換為Datetime
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <param name="Default">預設時間</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object source, DateTime Default)
        {
            DateTime result = Default;
            DateTime.TryParse(source.ToString(), out result);
            return result;
        }

        /// <summary>
        /// 將System.Int? 轉換為數值或DBNull.Value的物件
        /// </summary>
        /// <param name="source">System.Int?</param>
        /// <returns>System.Object</returns>
        public static object ToDBValue(this int? source)
        {
            object result = DBNull.Value;
            if (source.HasValue)
                result = source.Value;
            return result;
        }

        /// <summary>
        /// 將System.Decimal? 轉換為數值或DBNull.Value的物件
        /// </summary>
        /// <param name="source">System.Decimal?</param>
        /// <returns>System.Object</returns>
        public static object ToDBValue(this decimal? source)
        {
            object result = DBNull.Value;
            if (source.HasValue)
                result = source.Value;
            return result;
        }

        /// <summary>
        /// 將System.DateTime? 轉換為DateTime或DBNull.Value的物件
        /// </summary>
        /// <param name="source">System.DateTime?</param>
        /// <returns>System.Object</returns>
        public static object ToDBValue(this DateTime? source)
        {
            object result = DBNull.Value;
            if (source.HasValue)
                result = source.Value;
            return result;
        }

        /// <summary>
        /// 將System.Guid? 轉換為Guid或DBNull.Value的物件
        /// </summary>
        /// <param name="source">System.Guid?</param>
        /// <returns>System.Object</returns>
        public static object ToDBValue(this Guid? source)
        {
            object result = DBNull.Value;
            if (source.HasValue)
                result = source.Value;
            return result;
        }

        /// <summary>
        /// 轉換為金額字串(每千分位加一個 , )
        /// </summary>
        /// <param name="source">System.Object</param>
        /// <returns>金額字串</returns>
        public static object ConvertToCurrency(this object source)
        {
            decimal? result = source.ParseDecimal();

            return result.HasValue ? ((result.Value == 0) ? "0" : result.Value.ToString("#,###.##")) : string.Empty;
        }
    }
}
