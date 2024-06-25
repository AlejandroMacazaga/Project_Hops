using System;
using System.Collections.Generic;

namespace Utils.Extensions
{
    public static class ListExtensions
    {
        private static Random _rng;
        
        public static IList<T> Shuffle<T>(this IList<T> list) {
            _rng ??= new Random();
            var count = list.Count;
            while (count > 1) {
                --count;
                var index = _rng.Next(count + 1);
                (list[index], list[count]) = (list[count], list[index]);
            }

            return list;
        }
    }
}