using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 技能附加-特性
/// 使用该特性的属性会被技能修改 否则不能被技能所修改
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SkillAddition : Attribute
{

}