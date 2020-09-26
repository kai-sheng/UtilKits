using System;
using System.Collections.Generic;
using System.Globalization;

namespace UtilKits.Extensions
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 將目前System.DateTime轉換成Unix TimeSpan
        /// </summary>
        /// <param name="source">System.DateTime</param>
        /// <returns>UnixTimeSpan</returns>
        public static TimeSpan ToUnixTimeSpan(this DateTime source)
        {
            return (source.ToUniversalTime() - new DateTime(1970, 1, 1));
        }

        /// <summary>
        /// 將目前System.DateTime轉換成Unix Time Stamp
        /// </summary>
        /// <param name="source">System.DateTime</param>
        /// <returns>UnixTimeStamp</returns>
        public static long ToUnixTimestamp(this DateTime source)
        {
            return Math.Floor(source.ToUnixTimeSpan().TotalSeconds).ToLong();
        }

        /// <summary>
        /// 將目前System.DateTime轉換成台北標準時間
        /// </summary>
        /// <param name="source">System.DateTime</param>
        /// <returns>台北標準時間</returns>
        public static DateTime ToTaipeiStandardTime(this DateTime source)
        {
            TimeZoneInfo taipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");

            return !TimeZoneInfo.Local.Equals(taipeiTimeZone) ?
                TimeZoneInfo.ConvertTime(source, taipeiTimeZone) :
                source;
        }

        /// <summary>
        /// 取得結束日期前的所有日期
        /// </summary>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns>結束日期前的所有日期</returns>
        public static IEnumerable<DateTime> GetEachDays(this DateTime startDate, DateTime endDate)
        {
            return startDate.Date.GetEachDateTime(endDate.Date, day => day.AddDays(1));
        }

        /// <summary>
        /// 取得結束日期前的所有小時
        /// </summary>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns>結束日期前的所有小時</returns>
        public static IEnumerable<DateTime> GetEachHours(this DateTime startDate, DateTime endDate)
        {
            return startDate.GetEachDateTime(endDate, day => day.AddHours(1));
        }

        /// <summary>
        /// 取得結束日期前的所有日期(UTC計算使用)
        /// </summary>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <param name="increaseDay">遞增天數</param>
        /// <returns>結束日期前的所有日期</returns>
        public static IEnumerable<DateTime> GetEachDateTime(this DateTime startDate, DateTime endDate, Func<DateTime, DateTime> callback)
        {
            for (DateTime day = startDate; day <= endDate; day = callback(day))
            {
                yield return day;
            }
        }

        /// <summary>
        /// 取得今天的第一天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        /// <summary>
        /// 取得指定日期的月份的第一天
        /// </summary>
        /// <param name="date">System.DateTime</param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// 取得指定日期的月份的最後一天
        /// </summary>
        /// <param name="date">System.DateTime</param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        /// <summary>
        /// 取得指定時間的小時整點或指定分鐘
        /// </summary>
        /// <param name="date">System.DateTime</param>
        /// <param name="minutes">指定分鐘數(0~59)</param>
        /// <returns></returns>
        public static DateTime HourTime(this DateTime date, int minutes = 0)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, minutes, 0);
        }

        /// <summary>
        /// 取得指定時間是否在此區間
        /// </summary>
        /// <param name="date">System.DateTime</param>
        /// <param name="start">起始時間</param>
        /// <param name="end">結束時間</param>
        /// <returns></returns>
        public static bool Between(this DateTime date, DateTime start, DateTime end)
        {
            return date >= start && date <= end;
        }

        /// <summary>
        /// 將UNIX時間格式轉為DateTime(Seconds)
        /// Unix timestamp is seconds past epoch
        /// </summary>
        public static DateTime ToDateTimeBySec(long unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// 將UNIX時間格式轉為DateTime(Milliseconds)
        /// Unix timestamp is seconds past epoch
        /// </summary>
        public static DateTime ToDateTimeByMilliSec(long unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// 依字串格式及轉換為地區樣式的時間格式
        /// </summary>
        /// <param name="s">日期字串</param>
        /// <param name="format">字串格式</param>
        /// <param name="cultureString">地區樣式</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string s,
            string format, string cultureString = "zh-TW")
        {
            try
            {
                var r = DateTime.ParseExact(
                    s: s,
                    format: format,
                    provider: CultureInfo.GetCultureInfo(cultureString));
                return r;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (CultureNotFoundException)
            {
                throw;
            }
        }

        /// <summary>
        /// 取得下個整點小時的差距時間
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetNextFullHour(this DateTime date)
        {
            var timeOfDay = date.TimeOfDay;
            var nextFullHour = TimeSpan.FromHours(Math.Ceiling(timeOfDay.TotalHours));

            return nextFullHour - timeOfDay;
        }
    }
}
