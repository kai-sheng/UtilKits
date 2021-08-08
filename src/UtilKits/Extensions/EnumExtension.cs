using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace UtilKits.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// 由指定的列舉描述取得指定型別的列舉
        /// </summary>
        /// <typeparam name="T">列舉型別</typeparam>
        /// <param name="description">描述</param>
        /// <returns></returns>
        public static T GetEnum<T>(this string description)
        {
            return GetEnumValue<T>(description);
        }

        /// <summary>
        /// 取得指定列舉的描述
        /// </summary>
        /// <param name="value">要取得描述的列舉</param>
        /// <returns>傳回描述，或是依照指定的強制處理狀況，回應其列舉名稱或擲出例外</returns>
        public static string GetDescription(this Enum value)
        {
            return GetEnumDescription(value);
        }

        /// <summary>取得指定列舉的描述</summary>
        /// <param name="value">要取得描述的列舉</param>
        /// <returns>傳回描述，或是依照指定的強制處理狀況，回應其列舉名稱或擲出例外</returns>
        public static string GetDescription<T>(object value)
        {
            return GetEnumDescription(value);
        }

        /// <summary>
        /// 由指定的Description取得對應的列舉值
        /// </summary>
        /// <typeparam name="T">列舉型別</typeparam>
        /// <param name="description">描述</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">當參數description為空字串時擲出</exception>
        /// <exception cref="ArgumentException">當型別參數T為不為列舉型別時擲出</exception>
        /// <exception cref="ArgumentException">當無法對應到任何列舉時擲出</exception>
        private static T GetEnumValue<T>(string description)
        {
            if (String.IsNullOrWhiteSpace(description))
                throw new ArgumentException(String.Format("參數description必須有值"));

            Type type = typeof(T);

            //Make sure the object is an enum.
            if (!type.GetTypeInfo().IsEnum)
                throw new ArgumentException("T必須為列舉型別");

            return Enum.GetValues(type).Cast<T>().SingleOrDefault(t =>
            {
                FieldInfo fieldInfo = type.GetField(t.ToString());
                var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptionAttributes == null || descriptionAttributes.Count() == 0)
                {
                    return false;
                }
                else if (descriptionAttributes.Count() > 1)
                {
                    throw new Exception($"列舉類型「{type.Name}」有過多的Description屬性，相對應的列舉為「{t.ToString()}」");
                }

                return String.Compare((descriptionAttributes.First() as DescriptionAttribute).Description, description, true) == 0;
            });
        }

        /// <summary>取得指定列舉的描述</summary>
        /// <param name="value">要取得描述的列舉</param>
        /// <returns>傳回描述，或是依照指定的強制處理狀況，回應其列舉名稱或擲出例外</returns>
        public static string GetEnumDescription<T>(this T value)
        {
            Type type = value.GetType();

            //Make sure the object is an enum.
            if (!type.GetTypeInfo().IsEnum)
            {
                throw new ArgumentException("其值必須為列舉");
            }
            FieldInfo fieldInfo = type.GetField(value.ToString());
            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptionAttributes == null || descriptionAttributes.Count() == 0)
            {
                return value.ToString();
            }
            else if (descriptionAttributes.Count() > 1)
            {
                throw new Exception($"列舉類型「{type.Name}」有過多的Description屬性，相對應的列舉為「{value.ToString()}」");
            }

            //Return the value of the DescriptionAttribute.
            return (descriptionAttributes.First() as DescriptionAttribute).Description;
        }

        /// <summary>
        /// 將多重列舉[Flags]數值轉換為清單型態
        /// </summary>
        /// <typeparam name="T">需轉換的列舉型別</typeparam>
        /// <param name="value">要轉換的列舉</param>
        /// <returns></returns>
        public static IEnumerable<T> ToFlagList<T>(this Enum value)
        {
            if (!typeof(T).GetTypeInfo().IsEnum)
                throw new ArgumentException("其值必須為列舉");

            return Enum.GetValues(typeof(T)).Cast<Enum>().Where(m => value.HasFlag(m)).Cast<T>();
        }
        /// <summary>
        /// 依據 int:value 取得enum的值
        /// </summary>
        /// <typeparam name="T">Enum型別</typeparam>
        /// <param name="value">int 值</param>
        /// <returns></returns>
        public static T GetEnumValue<T>(this int value) where T : Enum
        {
            if (!typeof(T).GetTypeInfo().IsEnum)
                throw new ArgumentException("其值必須為列舉");
            var result = Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault(o => o.ToInt() == value);
            return result;
        }
        /// <summary>
        /// 取得Enum清單
        /// </summary>
        /// <typeparam name="T">Enum型別</typeparam>
        /// <returns></returns>
        public static List<T> GetEnumList<T>() where T : Enum
        {
            return ((T[])Enum.GetValues(typeof(T))).ToList();
        }
    }
}
