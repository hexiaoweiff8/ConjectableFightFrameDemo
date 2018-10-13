using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



/// <summary>
/// 行为链单位接口
/// </summary>
public interface IFormula
{
    /// <summary>
    /// 下一个节点
    /// </summary>
    IFormula NextFormula { get; set; }

    /// <summary>
    /// 上一个节点
    /// </summary>
    IFormula PreviewFormula { get; set; }

    /// <summary>
    /// 是否可以继续下一个
    /// </summary>
    bool CanMoveNext { get; set; }

    /// <summary>
    /// 行为链类型
    /// 0: 无需等待直接继续下一节点
    /// 1: 等待当前节点执行完成再执行下一节点
    /// </summary>
    int FormulaType { get; set; }

    /// <summary>
    /// 具体执行lambda表达式
    /// </summary>
    Action<Action, DataScope> Do { get; }

    /// <summary>
    /// 添加下一节点
    /// </summary>
    /// <param name="nextBehavior">下一个节点</param>
    /// <returns>下一个节点</returns>
    IFormula After(IFormula nextBehavior);

    /// <summary>
    /// 添加前一节点
    /// </summary>
    /// <param name="preBehavior">前一个节点</param>
    /// <returns>前一个节点</returns>
    IFormula Before(IFormula preBehavior);

    /// <summary>
    /// 当前节点是否有下一节点
    /// </summary>
    /// <returns></returns>
    bool HasNext();

    /// <summary>
    /// 获取链头
    /// </summary>
    /// <returns>链头单位</returns>
    IFormula GetFirst();

    /// <summary>
    /// 数据域
    /// </summary>
    DataScope DataScope { get; set; }
}
