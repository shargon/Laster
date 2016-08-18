using System;
using System.Collections.Generic;

namespace Laster.Core.Classes
{
    public class ShareableClass<T, TKey>
    {
        /// <summary>
        /// Valor
        /// </summary>
        public T Value { get; private set; }
        /// <summary>
        /// Clave
        /// </summary>
        public TKey Key { get; private set; }

        static Dictionary<ShareableClass<T, TKey>, List<object>> Dic = new Dictionary<ShareableClass<T, TKey>, List<object>>();
        static Dictionary<TKey, ShareableClass<T, TKey>> Dic2 = new Dictionary<TKey, ShareableClass<T, TKey>>();

        public bool Free(object obj, Action<T> releaseAction)
        {
            List<object> r2;
            lock (Dic)
            {
                if (Dic.TryGetValue(this, out r2))
                {
                    r2.Remove(obj);
                    if (r2.Count != 0) return false;

                    Dic.Remove(this);
                    Dic2.Remove(Key);
                    releaseAction(Value);
                    return true;
                }
            }

            return false;
        }
        public static ShareableClass<T, TKey> GetOrCreate(object obj, TKey key, Func<TKey, T> item)
        {
            lock (Dic)
            {
                ShareableClass<T, TKey> r2;
                if (Dic2.TryGetValue(key, out r2))
                {
                    List<object> l = Dic[r2];
                    if (!l.Contains(obj)) l.Add(obj);
                    return r2;
                }

                // Nuevo
                r2 = new ShareableClass<T, TKey>()
                {
                    Key = key,
                    Value = item(key)
                };
                if (r2.Value == null) return null;

                Dic.Add(r2, new List<object>(new object[] { obj }));
                Dic2.Add(key, r2);
                return r2;
            }
        }
    }
}