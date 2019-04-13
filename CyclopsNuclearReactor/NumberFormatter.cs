namespace CyclopsNuclearReactor
{
    using System.Collections.Generic;
    using UnityEngine;

    internal static class NumberFormatter
    {
        private static readonly IDictionary<int, string> _stringCache = new Dictionary<int, string>();

        internal static string FormatNumber(int number)
        {
            if (_stringCache.TryGetValue(number, out string knownValue))
            {
                return knownValue;
            }
            else
            {
                string newValue = HandleLargeNumbers(number);
                _stringCache.Add(number, newValue);
                return newValue;
            }
        }

        private static string HandleLargeNumbers(int possiblyLargeValue)
        {
            if (possiblyLargeValue > 9999999)
            {
                return $"{possiblyLargeValue / 1000000f:F1}M";
            }

            if (possiblyLargeValue > 9999)
            {
                return $"{possiblyLargeValue / 1000f:F1}K";
            }

            return $"{Mathf.CeilToInt(possiblyLargeValue)}";
        }
    }
}
