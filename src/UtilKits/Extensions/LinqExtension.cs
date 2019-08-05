using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilKits.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                var elementValue = keySelector(element);
                if (seenKeys.Add(elementValue))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TSource> Last<TSource>(this IEnumerable<TSource> source, int takeSize)
        {
            return source.Skip(Math.Max(0, source.Count() - takeSize));
        }

        public static IEnumerable<IEnumerable<TSource>> TakePaging<TSource>(this IEnumerable<TSource> source, int takeSize)
        {
            int skipSize = 0;
            IEnumerable<TSource> pageSource;

            do
            {
                pageSource = source.Skip(skipSize).Take(takeSize);
                skipSize += takeSize;

                if (pageSource.Count() > 0)
                    yield return pageSource;

            } while (pageSource.Count() > 0);
        }

        public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source, int takeSize = 0)
        {
            var rand = new Random(Environment.TickCount);
            var list = source.ToList();
            int maxValue = list.Count;
            int takeCount = (takeSize == 0 || takeSize > maxValue) ? maxValue : takeSize;

            do
            {
                int index = rand.Next(0, maxValue);
                yield return list.ElementAt(index);

                list.RemoveAt(index);
                maxValue--;
                takeCount--;
            } while (takeCount > 0);
        }
    }
}
