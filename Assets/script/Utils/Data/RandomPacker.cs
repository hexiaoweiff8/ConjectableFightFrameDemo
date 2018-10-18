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
    private LCGRandom random = new LCGRandom(99);

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
        random = new LCGRandom((ulong)seed);
    }

    /// <summary>
    /// 获取随机值
    /// </summary>
    /// <param name="min">范围最小值</param>
    /// <param name="max">范围最大值</param>
    /// <returns></returns>
    public int GetRangeI(int min, int max)
    {
        return (int)random.Rand((ulong)min, (ulong)max);
    }
}



/// <summary>
/// 线性同余
/// </summary>
public class LCGRandom
{
    /// <summary>
    /// 实例化
    /// </summary>
    /// <param name="seed"></param>
    public LCGRandom(ulong seed = 0)
    {
        rand_seed = seed;
    }

    /// <summary>
    /// 生成随机数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public ulong Rand(ulong min, ulong max)
    {
        //System.Diagnostics.Debug.Assert(max > min && min >= 0);

        ulong A = 0x5DEECE66D;
        ulong C = 0xB;
        ulong M = ((ulong)1 << 48);
        rand_seed = (rand_seed * A + C) % M;

        ulong bb = max - min;
        ulong value = rand_seed % bb + min;
        return value;
    }
    private ulong rand_seed;
}