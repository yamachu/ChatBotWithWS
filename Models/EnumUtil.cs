using System;
using System.Collections;
using System.Collections.Generic;

namespace ChatBotWithWS.Models
{
    // http://qiita.com/hugo-sb/items/38fe86a09e8e0865d471
    // ジェネリックを利用した汎用ヘルパクラス
    static class EnumUtil<T>
    {
        // 整数値が enum で定義済みかどうか？
        public static bool IsDefined(int n)
        {
            return Enum.IsDefined(typeof(T), n);
        }

        // Foreach用のGetEnumeratorを持つヘルパクラス
        public class EnumerateEnum: IEnumerable<T>
        {
            public IEnumerator<T> GetEnumerator()
            {
                foreach (T e in Enum.GetValues(typeof(T)))
                    yield return e;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        // enum定義をforeachに渡すためのヘルパクラスを返す
        public static EnumerateEnum Enumerate()
        {
            return new EnumerateEnum();
        }
    }
}