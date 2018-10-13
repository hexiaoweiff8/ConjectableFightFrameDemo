using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// 随机包装类
/// 用来随机产生同步伪随机值
/// </summary>
public class RandomPacker
{
    /// <summary>
    /// 单例
    /// </summary>
    public static RandomPacker Single
    {
        get
        {
            if (single == null)
            {
                single = new RandomPacker();
            }
            return single;
        }
    }


    /// <summary>
    /// 单例对象
    /// </summary>
    private static RandomPacker single = null;

    /// <summary>
    /// 随机类
    /// </summary>
    private Random random = new Random(99);

    /// <summary>
    /// 本地随机种子
    /// </summary>
    private int seed = -1;

    /// <summary>
    /// 设置本地随机种子
    /// </summary>
    /// <param name="newSeed"></param>
    public void SetSeed(int newSeed)
    {
        seed = newSeed;
        random = new Random(seed);
    }

    /// <summary>
    /// 获取随机值
    /// </summary>
    /// <param name="min">范围最小值</param>
    /// <param name="max">范围最大值</param>
    /// <returns></returns>
    public int GetRangeI(int min, int max)
    {
        return random.Next(min, max);
    }
}