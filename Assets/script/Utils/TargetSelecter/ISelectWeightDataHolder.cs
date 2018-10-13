/// <summary>
/// ѡ��Ŀ��Ȩ�س�����
/// </summary>
public interface ISelectWeightDataHolder
{
    SelectWeightData SelectWeightData { get; set; }
}

/// <summary>
/// Ŀ��ѡ��Ȩ��
/// </summary>
public class SelectWeightData
{
    // Level 1, 2, 3����ֵ���Ǵ�-1 - ������, -1Ϊ��ȫ�����, 0Ϊ��Ӱ��Ȩ��, Ȩ��Խ��Խ��Ҫ
    // Level 4��ֵ 0 - ������ ���������ȫ���������


    // ----------------------------Ȩ��ѡ�� Level1-----------------------------
    /// <summary>
    /// ѡ����浥λȨ��
    /// </summary>
    public float SurfaceWeight { get; set; }

    /// <summary>
    /// ѡ����յ�λȨ��
    /// </summary>
    public float AirWeight { get; set; }

    /// <summary>
    /// ѡ����Ȩ��
    /// </summary>
    public float BuildWeight { get; set; }

    /// <summary>
    /// ��Ӫѡ��Ȩ��
    /// 0ȫѡ
    /// 1����
    /// 2�з�
    /// </summary>
    public int CampWeight { get; set; }

    /// <summary>
    /// ����ֵѡ��Χ����
    /// 0:�ٷֱ�
    /// 1:����ֵ
    /// </summary>
    public int HpScopeType { get; set; }

    /// <summary>
    /// ����ֵѡ��Χ���ֵ
    /// -1���ֵ��Ч
    /// </summary>
    public float HpScopeMaxValue { get; set; }

    /// <summary>
    /// ����ֵѡ��Χ��Сֵ
    /// -1���ֵ��Ч
    /// </summary>
    public float HpScopeMinValue { get; set; }

    /// <summary>
    /// ������Debuffʱ
    /// 0: ����Ϊ����
    /// 1: ���ϱ����и���buff
    /// -1:���ϱ���û�и���buff
    /// </summary>
    public int DeBuffWeight { get; set; }

    /// <summary>
    /// ����������buffʱ
    /// 0: ����Ϊ����
    /// 1: ���ϱ���������buff
    /// -1:���ϱ���û������buff
    /// </summary>
    public int BuffWeight { get; set; }


    // ----------------------------Ȩ��ѡ�� Level2-----------------------------

    /// <summary>
    /// ����Ȩ��
    /// </summary>
    public float HumanWeight { get; set; }

    /// <summary>
    /// ����Ȩ��
    /// </summary>
    public float OrcWeight { get; set; }

    /// <summary>
    /// ��еȨ��
    /// </summary>
    public float OmnicWeight { get; set; }
    ///// <summary>
    ///// ѡ��̹��Ȩ��
    ///// </summary>
    //public float TankWeight { get; set; }

    ///// <summary>
    ///// ѡ�������ؾ�Ȩ��
    ///// </summary>
    //public float LVWeight { get; set; }

    ///// <summary>
    ///// ѡ�����Ȩ��
    ///// </summary>
    //public float CannonWeight { get; set; }

    ///// <summary>
    ///// ѡ�������Ȩ��
    ///// </summary>
    //public float AirCraftWeight { get; set; }

    ///// <summary>
    ///// ѡ�񲽱�Ȩ��
    ///// </summary>
    //public float SoldierWeight { get; set; }


    // ----------------------------Ȩ��ѡ�� Level3-----------------------------
    /// <summary>
    /// ѡ�����ε�λȨ��
    /// </summary>
    public float HideWeight { get; set; }

    /// <summary>
    /// ѡ�񳰷�Ȩ��(���ֵӦ�úܴ�, �����з�����Ч���ĵ�λ)
    /// </summary>
    public float TauntWeight { get; set; }


    // ----------------------------Ȩ��ѡ�� Level4-----------------------------


    /// <summary>
    /// ������Ȩ��
    /// </summary>
    public float HealthMinWeight { get; set; }

    /// <summary>
    /// ������Ȩ��
    /// </summary>
    public float HealthMaxWeight { get; set; }


    /// <summary>
    /// ��λ��Ȩ��
    /// </summary>
    public float DistanceMinWeight { get; set; }

    /// <summary>
    /// Զλ��Ȩ��
    /// </summary>
    public float DistanceMaxWeight { get; set; }

    ///// <summary>
    ///// �Ƕ�Ȩ��
    ///// </summary>
    //public float AngleWeight { get; set; }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public SelectWeightData()
    {

    }

    ///// <summary>
    ///// ��ʼ��
    ///// </summary>
    ///// <param name="armyaim">��ʼ������</param>
    //public SelectWeightData(armyaim_cInfo armyaim)
    //{
    //    SetSelectWeightData(armyaim);
    //}


    ///// <summary>
    ///// ��������
    ///// </summary>
    ///// <param name="armyaim"></param>
    //public void SetSelectWeightData(armyaim_cInfo armyaim)
    //{
    //    SurfaceWeight = armyaim.Surface;
    //    AirWeight = armyaim.Air;
    //    BuildWeight = armyaim.Build;

    //    HumanWeight = armyaim.Human;
    //    OrcWeight = armyaim.Orc;
    //    OmnicWeight = armyaim.Omnic;

    //    HideWeight = armyaim.Hide;
    //    TauntWeight = armyaim.Taunt;

    //    HealthMinWeight = armyaim.HealthMin;
    //    HealthMaxWeight = armyaim.HealthMax;
    //    DistanceMinWeight = armyaim.RangeMin;
    //    DistanceMaxWeight = armyaim.RangeMax;

    //    HpScopeMaxValue = -1;
    //    HpScopeMinValue = -1;
    //}
}