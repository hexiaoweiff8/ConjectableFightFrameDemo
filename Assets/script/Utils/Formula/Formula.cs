using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


/// <summary>
/// 行为链单位
/// </summary>
public class Formula : IFormula
{


    // -------------------------属性---------------------------
    /// <summary>
    /// 下一个节点
    /// </summary>
    public IFormula NextFormula { get; set; }

    /// <summary>
    /// 上一个节点
    /// </summary>
    public IFormula PreviewFormula { get; set; }

    /// <summary>
    /// 是否可以继续下一个
    /// 如果该值为false则不能获取下一节点
    /// </summary>
    public bool CanMoveNext {
        get { return canMoveNext; }
        set { canMoveNext = value; } }

    /// <summary>
    /// 常量 不等待直接下一节点执行
    /// </summary>
    public const int FormulaNotWaitType = 0;

    /// <summary>
    /// 常亮 等待完成后继续执行
    /// </summary>
    public const int FormulaWaitType = 1;

    /// <summary>
    /// 行为链类型
    /// 0: 无需等待直接继续下一节点
    /// 1: 等待当前节点执行完成再执行下一节点
    /// </summary>
    public int FormulaType
    {
        get { return formulaType; }
        set { formulaType = value; }
    }

    /// <summary>
    /// 数据域
    /// </summary>
    public DataScope DataScope { get; set; }

    /// <summary>
    /// 当前节点执行的操作 
    /// 外部只读
    /// </summary>
    public Action<Action, DataScope> Do { get; protected set; }

    /// <summary>
    /// 行为链执行方式
    /// 0: 无需等待直接继续下一节点
    /// 1: 等待当前节点执行完成再执行下一节点
    /// </summary>
    protected int formulaType = FormulaNotWaitType;

    /// <summary>
    /// 是否可以继续执行下一个节点
    /// 如果该值为false则不能获取下一节点
    /// </summary>
    private bool canMoveNext = true;


    // -----------------------公用方法-----------------------


    /// <summary>
    /// 构建方法
    /// 传入执行操作
    /// </summary>
    /// <param name="doForWaitAction">当前节点的行为</param>
    /// <param name="type">执行类型, 0:不等待是否执行完毕继续下一届点, 1:等待节点执行完毕调用回调.</param>
    public Formula(Action<Action, DataScope> doForWaitAction, int type = FormulaNotWaitType)
    {
        Do = doForWaitAction;
        formulaType = type;
    }

    /// <summary>
    /// 获取行为链链头
    /// </summary>
    /// <returns>链头单位</returns>
    public IFormula GetFirst()
    {
        IFormula tmpItem = PreviewFormula;
        IFormula first = this;
        while (tmpItem != null)
        {
            first = tmpItem;
            tmpItem = tmpItem.PreviewFormula;
        }
        return first;
    }

    /// <summary>
    /// 当前节点是否能获取下一单位
    /// </summary>
    /// <returns></returns>
    public bool HasNext()
    {
        if (NextFormula != null && canMoveNext)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// 添加下一个节点
    /// </summary>
    /// <param name="nextBehavior">下一个节点</param>
    /// <returns>下一节点</returns>
    public IFormula After(IFormula nextBehavior)
    {
        if (nextBehavior != null)
        {
            // 如果后一个单位不为空则向后移
            if (NextFormula != null)
            {
                NextFormula.After(NextFormula);
            }
            NextFormula = nextBehavior;
            nextBehavior.PreviewFormula = this;

            return nextBehavior;
        }
        return this;
    }

    /// <summary>
    /// 添加前一个节点
    /// </summary>
    /// <param name="preBehavior">前一个节点</param>
    /// <returns>前一节点</returns>
    public IFormula Before(IFormula preBehavior)
    {
        if (preBehavior != null)
        {
            // 如果前一个单位不为空则向前移
            if (PreviewFormula != null)
            {
                PreviewFormula.Before(PreviewFormula);
            }
            PreviewFormula = preBehavior;
            preBehavior.NextFormula = this;

            return preBehavior;
        }
        return this;
    }
}



/// <summary>
/// 泛型数据域
/// </summary>
public class DataScope
{
    /// <summary>
    /// 数据列表
    /// </summary>
    private Dictionary<Type, Dictionary<string, object>> dataDic = new Dictionary<Type, Dictionary<string, object>>();

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">数据key</param>
    /// <param name="data">数据</param>
    public void SetData<T>([NotNull]string key, [NotNull]T data)
    {
        var type = typeof(T);
        if (!dataDic.ContainsKey(type))
        {
            dataDic.Add(type, new Dictionary<string, object>());
        }
        if (dataDic[type].ContainsKey(key))
        {
            dataDic[type].Add(key, data);
        }
    }

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <typeparam name="T">被获取数据类型</typeparam>
    /// <param name="key">被获取数据key</param>
    /// <returns></returns>
    public T GetData<T>([NotNull]string key)
    {
        T result = default(T);

        var type = typeof(T);
        if (dataDic.ContainsKey(type))
        {
            if (dataDic[type].ContainsKey(key))
            {
                result = (T)dataDic[type][key];
            }
            else
            {
                throw new Exception("数据不存在:" + key);
            }
        }
        else
        {
            throw new Exception("数据不存在:" + key);
        }

        return result;
    }

    /// <summary>
    /// 是否包含数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">数据key</param>
    /// <returns></returns>
    public bool HasData<T>([NotNull] string key)
    {
        var result = false;

        var type = typeof(T);
        if (dataDic.ContainsKey(type))
        {
            if (dataDic[type].ContainsKey(key))
            {
                result = true;
            }
        }

        return result;
    }
}