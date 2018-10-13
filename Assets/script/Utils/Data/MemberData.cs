using UnityEngine;
using System.Collections;

public class MemberData
{
    private float _totalHP;
    private float _currentHP;
    private int _entityID;
    private string _name;
    private int _objID;

    private int _armyID;
    private short _armyType;
    private float _armor;
    private float _antiArmor;
    private float _antiCrit;
    private float _crit;
    private float _dodge;
    private float _hit;
    private float _attackRange;
    private float _skillRange;
    private float _moveSpeed;
    private int _behaviorType;
    private int _camp;
    private string _modelPack;
    private string _modelName;
    private string _modelTexture;


    public int UniqueID;
    /// <summary>
    /// 兵种等级
    /// </summary>
    public short ArmyLevel;
    /// <summary>
    /// 目标空地类型
    /// </summary>
    public short AimGeneralType;
    /// <summary>
    /// 部署时间
    /// </summary>
    public float DeployTime;
    /// <summary>
    /// 自身空地建筑属性总类型
    /// </summary>
    public short GeneralType;
    /// <summary>
    /// 火力
    /// </summary>
    [SkillAddition]
    public float Attack1;
  
    /// <summary>
    /// 弹容量
    /// </summary>
    [SkillAddition]
    public short Clipsize;

    /// <summary>
    /// 攻击速度
    /// </summary>
    [SkillAddition]
    public float AttackRate;
  
    /// <summary>
    /// 装填时间
    /// </summary>
    [SkillAddition]
    public float ReloadTime1;
   
    /// <summary>
    /// 精准度
    /// </summary>
    [SkillAddition]
    public float Accuracy;
    /// <summary>
    /// 体积因子(受击半径)
    /// </summary>
    [SkillAddition]
    public float SpaceSet;
    /// <summary>
    /// 散布半径
    /// </summary>
    public float SpreadRange;
    /// <summary>
    /// 防御
    /// </summary>
    [SkillAddition]
    public float Defence;
    /// <summary>
    /// 弹道抛射角度值
    /// </summary>
    public float BallisticsAngle;
    /// <summary>
    /// 爆击伤害
    /// </summary>
    [SkillAddition]
    public float CritDamage;
    /// <summary>
    /// 修正暴击伤害
    /// </summary>
    [SkillAddition]
    public float FixCritDemage;
    /// <summary>
    /// 子弹类型
    /// </summary>
    [SkillAddition]
    public short BulletType;
    /// <summary>
    /// 子弹速度
    /// </summary>
    [SkillAddition]
    public float BulletSpeed;
    /// <summary>
    /// 攻击类型（单体/AOE）
    /// </summary>
    [SkillAddition]
    public short AttackType;
    ///// <summary>
    ///// 多目标攻击上限
    ///// </summary>
    //[SkillAddition]
    //public short MultiAimMax;
    /// <summary>
    ///视野范围
    /// </summary>
    [SkillAddition]
    public float SightRange;
    /// <summary>
    /// 是否隐形
    /// </summary>
    [SkillAddition]
    public bool IsHide;
    /// <summary>
    /// 是否嘲讽
    /// </summary>
    [SkillAddition]
    public bool IsTaunt;
    /// <summary>
    /// 是否为机械单位
    /// </summary>
    [SkillAddition]
    public bool IsMechanic;
    /// <summary>
    /// 是否近战
    /// </summary>
    [SkillAddition]
    public bool IsMelee;
    /// <summary>
    /// 是否霸体
    /// </summary>
    [SkillAddition]
    public bool IsDambody;
    /// <summary>
    /// 是否为召唤生物
    /// </summary>
    [SkillAddition]
    public bool IsSummon;
    /// <summary>
    /// 是否可移动
    /// </summary>
    [SkillAddition]
    public bool CouldMove = true;
    /// <summary>
    /// 是否可普通攻击
    /// </summary>
    [SkillAddition]
    public bool CouldNormalAttack = true;
    /// <summary>
    /// 是否可释放技能
    /// </summary>
    [SkillAddition]
    public bool CouldReleaseSkill = true;
    /// <summary>
    /// 是否抵抗Buff
    /// </summary>
    [SkillAddition]
    public bool IsAntiBuff;
    /// <summary>
    /// 是否反隐形
    /// </summary>
    [SkillAddition]
    public bool IsAntiHide;
    /// <summary>
    /// 生存时间
    /// </summary>
    [SkillAddition]
    public float LifeTime; 
    /// <summary>
    /// 技能1
    /// </summary>
    [SkillAddition]
    public int Skill1;
    /// <summary>
    /// 技能2
    /// </summary>
    [SkillAddition]
    public int Skill2;
    /// <summary>
    /// 技能3
    /// </summary>
    [SkillAddition]
    public int Skill3;
    /// <summary>
    /// 技能4
    /// </summary>
    [SkillAddition]
    public int Skill4;
    /// <summary>
    /// 技能5
    /// </summary>
    [SkillAddition]
    public int Skill5;


    /// <summary>
    /// 穿甲属性
    /// </summary>
    [SkillAddition]
    public float AntiArmor
    {
        get { return _antiArmor; }
        set { _antiArmor = value; }
    }

    /// <summary>
    /// 对应的模型资源包名
    /// </summary>
    public string ModelPack
    {
        get { return _modelPack; }
        set { _modelPack = value; }
    }
    /// <summary>
    /// 对应模型的预设名字
    /// </summary>
    public string ModelName
    {
        get { return _modelName; }
        set { _modelName = value; }
    }

    public string ModelTexture
    {
        get { return _modelTexture;}
        set { _modelTexture = value; }
    }

    /// <summary>
    /// 战斗中的移动速度
    /// </summary>
    [SkillAddition]
    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }
    /// <summary>
    /// 行为类型
    /// </summary>
    public int BehaviorType
    {
        get { return _behaviorType; }
        set { _behaviorType = value; }
    }
    /// <summary>
    /// 攻击范围
    /// </summary>
    [SkillAddition]
    public float AttackRange
    {
        get { return _attackRange; }
        set { _attackRange = value; }
    }
    /// <summary>
    /// 技能释放范围
    /// </summary>
    [SkillAddition]
    public float SkillRange
    {
        get { return _skillRange; }
        set { _skillRange = value; }
    }
    /// <summary>
    /// 命中
    /// </summary>
    [SkillAddition]
    public float Hit
    {
        get { return _hit; }
        set { _hit = value; }
    }
    /// <summary>
    /// 闪避
    /// </summary>
    [SkillAddition]
    public float Dodge
    {
        get { return _dodge; }
        set { _dodge = value; }
    }
    /// <summary>
    /// 暴击概率
    /// </summary>
    [SkillAddition]
    public float Crit
    {
        get { return _crit; }
        set { _crit = value; }
    }
    /// <summary>
    /// 暴击修正(二重暴击)概率
    /// </summary>
    [SkillAddition]
    public float FixCrit { get; set; }

    /// <summary>
    /// 防爆率
    /// </summary>
    [SkillAddition]
    public float AntiCrit
    {
        get { return _antiCrit; }
        set { _antiCrit = value; }
    }
    /// <summary>
    /// 护甲
    /// </summary>
    [SkillAddition]
    public float Armor
    {
        get { return _armor; }
        set { _armor = value; }
    }


    /// <summary>
    /// 自身大兵种类型
    /// </summary>
    public short ArmyType
    {
        get { return _armyType; }
        set { _armyType = value; }
    }
    /// <summary>
    /// 
    /// </summary>
    public int ArmyID
    {
        get { return _armyID; }
        set { _armyID = value; }
    }
    /// <summary>
    /// 存储在vo类身上的ObjectID 通过它 把数据和显示对象关联
    /// </summary>
    public int ObjID
    {
        get { return _objID; }
        set
        {
            _objID = value;
        }
    }
    
    /// <summary>
    /// 模版中存储的名字 
    /// </summary>
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    /// <summary>
    /// 总血量所有可被攻击的对象都有总血量和血量属性
    /// </summary>
    [SkillAddition]
    public float TotalHp
    {
        get { return _totalHP; }
        set
        {
            if (value > _totalHP)
            {
                _currentHP += value - _totalHP;
            }
            if (value < _currentHP)
            {
                _currentHP = value;
            }
            _totalHP = value; 
            
        }
    }
    /// <summary>
    /// 当前实际血量[0,TotalHp]
    /// </summary>
    [SkillAddition]
    public float CurrentHP
    {
        get { return _currentHP; }
        set
        {
            _currentHP = value;
            //if (_currentHP >= _totalHP)
            //{
            //    _currentHP = _totalHP;
            //}
        }
    }

    /// <summary>
    /// 对象的实体ID 唯一id 
    /// </summary>
    public int EntityID
    {
        get { return _entityID; }
        set { _entityID = value; }
    }

    /// <summary>
    /// 定义玩家的阵营1，我方 2 敌方
    /// </summary>
    [SkillAddition]
    public int Camp
    {
        get { return _camp; }
        set { _camp = value; }
    }

    //----------------------------字段声明都在上半部分---------------------------------

    /// <summary>
    /// 设置血量
    /// </summary>
    /// <param name="hp"></param>
    public void SetCurrentHP(float hp)
    {
        if (hp > _totalHP)
        {
            hp = _totalHP;
        }
        _currentHP = hp;
    }
    ///// <summary>
    ///// 装填单位属性
    ///// </summary>
    //public void SetSoldierData(armybase_cInfo data)
    //{
    //    Name = data.Name;
    //    ArmyID = data.ArmyID;
    //    ArmyType = data.ArmyType;
    //    TotalHp = data.HP;
    //    CurrentHP = TotalHp;
    //    Armor = data.Armor;
    //    AntiArmor = data.AntiArmor;
    //    AntiCrit = data.AntiCrit;
    //    Crit = data.Crit;
    //    Dodge = data.Dodge;
    //    Hit = data.Hit;
    //    AttackRange = data.AttackRange;
    //    SkillRange = data.SkillRange;
    //    MoveSpeed = data.MoveSpeed;
    //    BehaviorType = data.BehaviorType;
    //    UniqueID = data.UniqueID;
    //    ArmyLevel = data.ArmyLevel;
    //    AimGeneralType = data.AimGeneralType;
    //    DeployTime = data.DeployTime;
    //    GeneralType = data.GeneralType;
    //    Attack1 = data.Attack1;
    //    Clipsize1 = data.Clipsize1;
    //    AttackRate1 = data.AttackRate1;
    //    ReloadTime1 = data.ReloadTime1;
    //    Accuracy = data.Accuracy;
    //    SpaceSet = data.SpaceSet;
    //    SpreadRange = data.SpreadRange;
    //    Defence = data.Defence;
    //    CritDamage = data.CritDamage;
    //    BulletType = data.BulletType;
    //    BulletSpeed = data.BulletSpeed;
    //    AttackType = data.AttackType;
    //    SightRange = data.SightRange;
    //    IsHide = data.IsHide == 1;
    //    IsSummon = data.IsSummoned == 1;
    //    IsMechanic = data.IsCreature == 2;
    //    IsMelee = data.RangeType == 1;
    //    IsAntiHide = data.IsAntiHide == 1;
    //    LifeTime = data.LifeTime;
    //    Skill1 = data.Skill1;
    //    Skill2 = data.Skill2;
    //    Skill3 = data.Skill3;
    //    Skill4 = data.Skill4;
    //    Skill5 = data.Skill5;
    //    ModelPack = data.Pack;
    //    ModelName = data.Prefab;
    //    ModelTexture = data.Texture;
    //}


    public override string ToString()
    {
        return "HP:" + _currentHP + ", TotalHP" + _totalHP;
    }
}
