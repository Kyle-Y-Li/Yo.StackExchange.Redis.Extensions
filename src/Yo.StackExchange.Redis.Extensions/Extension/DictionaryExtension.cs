using System;
using System.Collections.Generic;

namespace Yo.StackExchange.Redis.Extensions.Extension;

/// <summary>
/// <see cref="Dictionary{TKey, TValue}"/> Extension
/// </summary>
public static class DictionaryExtension
{
    /// <summary>
    /// Convert IEnumerable to Dictionary, and when there are duplicate keys, take the first key
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
    /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
    ///
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
    /// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type <typeparamref name="TElement"/> selected from the input sequence.</returns>
    public static Dictionary<TKey, TElement> ToDictionaryExt<TSource, TKey, TElement>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector)
    {
        Dictionary<TKey, TElement> dict = new Dictionary<TKey, TElement>();

        if (source == null || keySelector == null || elementSelector == null)
        {
            return dict;
        }

        foreach (TSource k in source)
        {
            TKey key = keySelector(k);
            if (dict.ContainsKey(key))
            {
                continue;
            }

            dict.Add(key, elementSelector(k));
        }

        return dict;
    }

    public static TValue GetOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> valuePairs,
        TKey key,
        TValue defaultValue = default)
    {

        if (key == null) throw new ArgumentNullException(nameof(key));
        if (valuePairs == null) throw new ArgumentNullException(nameof(valuePairs));

        bool isExist = valuePairs.TryGetValue(key, out TValue value);
        if (isExist) { return value; }
        return defaultValue;
    }

    /// <summary>
    /// Try to add a new key to <paramref name="valuePairs"/>. If the same key already exists, it will not be added.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="valuePairs"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns>added successfully is true. Otherwise false</returns>
    public static bool TryAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> valuePairs,
         TKey key,
         TValue value)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (valuePairs == null) throw new ArgumentNullException(nameof(valuePairs));

        bool isExist = valuePairs.ContainsKey(key);
        if (!isExist)
        {
            valuePairs.Add(key, value);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get or add
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="valuePairs"></param>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> valuePairs,
        TKey key,
        Func<TValue> factory)

    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        if (valuePairs == null) throw new ArgumentNullException(nameof(valuePairs));

        bool isExist = valuePairs.TryGetValue(key, out TValue value);
        if (isExist) { return value; }

        TValue result = factory();
        lock (valuePairs)
        {
            valuePairs.Add(key, result);
        }
        return result;
    }
}
