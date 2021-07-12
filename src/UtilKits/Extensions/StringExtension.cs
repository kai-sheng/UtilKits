using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilKits.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// 檢查字串是否為數字
        /// </summary>
        /// <param name="source">string extension</param>
        /// <returns>True: 為數字字串, False: 非數字字串</returns>
        public static bool IsNumber(this string source)
        {
            if (String.IsNullOrEmpty(source))
                return false;

            bool isNum = false;

            isNum = Double.TryParse(source, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double num);
            return isNum;
        }

        /// <summary>
        /// 檢查字串是否為Guid
        /// </summary>
        /// <param name="source">string extension</param>
        /// <returns>True: 為數字字串, False: 非數字字串</returns>
        public static bool IsGuid(this string source)
        {
            if (String.IsNullOrEmpty(source))
                return false;

            string patternStrict = @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$";

            return Regex.IsMatch(source, patternStrict, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// 檢查字串是否為數字
        /// </summary>
        /// <param name="source">string extension</param>
        /// <returns>True: 為日期字串, False: 非日期字串</returns>
        public static bool IsDateTime(this string source)
        {
            if (String.IsNullOrEmpty(source))
                return false;

            bool isDate = false;
            DateTime dt;

            isDate = DateTime.TryParse(source, out dt);
            return isDate;
        }

        /// <summary>
        /// 檢查字串是否為電子郵件格式
        /// </summary>
        /// <param name="source">string extension</param>
        /// <returns>True: 為電子郵件格式, False: 非電子郵件格式</returns>
        public static bool IsEmail(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                                 + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                 + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                 + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                 + @"[a-zA-Z]{2,}))$";
            return Regex.IsMatch(source, patternStrict, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// 檢查字串是否為手機格式(純10碼數字)
        /// </summary>
        /// <param name="source">string extension</param>
        /// <returns>True: 為手機格式, False: 非手機格式</returns>
        public static bool IsMobile(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return Regex.IsMatch(source, @"^09[0-9]{8}$");
        }

        /// <summary>
        /// 檢查字串是否有 Html Tag
        /// </summary>
        /// <param name="source">string extension</param>
        /// <returns></returns>
        public static bool IsHtml(this string source)
        {
            Regex closureTagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>", RegexOptions.Compiled);
            Regex singleTagRegex = new Regex(@"<[^>]+>", RegexOptions.Compiled);

            return closureTagRegex.IsMatch(source) || singleTagRegex.IsMatch(source);
        }

        /// <summary>
        /// 是否為中文字元
        ///// / </summary>
        /// <param name="source">string extension</param>
        /// <returns>True: 中文字串, False: 非中文字串</returns>
        public static bool IsChineseWords(this char c)
        {
            return c >= 0x4E00 && c <= 0x9FA5;
        }

        /// <summary>
        /// 是否為中文字串
        ///// / </summary>
        /// <param name="source">string extension</param>
        /// <returns>True: 中文字串, False: 非中文字串</returns>
        public static bool IsChineseWords(this string source)
        {
            if (string.IsNullOrEmpty(source)) return false;

            char[] ch = source.ToCharArray();

            for (int i = 0; i < ch.Length; i++)
            {
                if (ch[i].IsChineseWords())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 依輸入的長度，擷取字串長度(不切割英文單字)
        /// 後置文字預設為(...)
        /// </summary>
        /// <param name="source">字串來源</param>
        /// <param name="length">擷取長度</param>
        /// <returns>擷取字串及後置文字</returns>
        public static string SubstringByWord(this string source, int length)
        {
            return SubstringByWord(source, length, "...");
        }

        /// <summary>
        /// 依輸入的長度，擷取字串長度(不切割英文單字)
        /// </summary>
        /// <param name="source">字串來源</param>
        /// <param name="length">擷取長度</param>
        /// <param name="suffix">後置文字</param>
        /// <returns>擷取字串及後置文字</returns>
        public static string SubstringByWord(this string source, int length, string suffix)
        {
            if (source.Length <= length)
                return source;

            //設定擷取長度
            Regex reg = new Regex(String.Concat(@".{", length, @"}.*?\b"), RegexOptions.Singleline);
            //比對字串
            MatchCollection match = reg.Matches(source);
            //回傳擷取字串及後置文字
            if (match.Count == 0)
                return String.Empty;

            if ((match[0].Value.Length - length) < 20)
                return String.Concat(match[0].Value, suffix);
            else
                return String.Concat(source.Substring(1, length), suffix);

        }

        /// <summary>
        /// 依輸入的字元遮蔽處理字串中不保留的字元
        /// </summary>
        /// <param name="source">字串來源</param>
        /// <param name="front">前保留字元數</param>
        /// <param name="back">後保留字元數</param>
        /// <param name="maskChar">遮蔽字元</param>
        /// <returns>遮蔽處理後的字串</returns>
        public static string Maskstring(this string source, int front, int back, char maskChar)
        {
            int length = source.Length;
            if (length <= front + back) return source;

            string frontStr = source.Substring(0, front);
            string backStr = source.Substring(length - back);
            return string.Concat(frontStr.PadRight(length - back, maskChar), backStr);
        }


        /// <summary>
        /// 清除StringBuilder字串
        /// </summary>
        /// <param name="source">StringBuilder物件</param>
        public static void Clear(this StringBuilder source)
        {
            source.Remove(0, source.Length);
        }

        /// <summary>
        /// 將字串切割成數字陣列
        /// </summary>
        /// <param name="source">字串來源</param>
        /// <param name="separator">切割字元</param>
        /// <returns>The to int.</returns>
        public static int[] SplitToInt(this string source, params char[] separator)
        {
            return source.Split(separator).Select(a => a.ToInt()).ToArray();
        }
    }
}
