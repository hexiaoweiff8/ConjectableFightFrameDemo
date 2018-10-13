using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// 单例对象
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleItem<T> where T : new()
{

    /// <summary>
    /// 单例
    /// </summary>
    public static T Single
    {
        get
        {
            if (single == null)
            {
                single = new T();
            }
            return single;
        }
    }

    /// <summary>
    /// 单例对象
    /// </summary>
    private static T single = default(T);

}