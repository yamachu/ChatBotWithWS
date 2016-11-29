using System;
using System.Collections.Generic;

namespace ChatBotWithWS.Models
{
    // http://stackoverflow.com/questions/8002455/how-to-easily-initialize-a-list-of-tuples
    public static class TupleListExtensions
    {
        public static void Add<T1, T2>(this IList<Tuple<T1, T2>> list,
                T1 item1, T2 item2)
        {
            list.Add(Tuple.Create(item1, item2));
        }
    }
}