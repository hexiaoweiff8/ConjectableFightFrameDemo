/// <summary>
/// ѡ��Ŀ��Ȩ�س�����
/// TODO �ĳɽӿ�, �����ó�����
/// </summary>
public interface ISelectWeightData
{
    // Level 1, 2, 3����ֵ���Ǵ�-1 - ������, -1Ϊ��ȫ�����, 0Ϊ��Ӱ��Ȩ��, Ȩ��Խ��Խ��Ҫ
    // Level 4��ֵ 0 - ������ ���������ȫ���������


    // ----------------------------Ȩ��ѡ�� Level1-----------------------------
    /// <summary>
    /// ѡ����浥λȨ��
    /// </summary>
    float SurfaceWeight { get; set; }

    /// <summary>
    /// ѡ����յ�λȨ��
    /// </summary>
    float AirWeight { get; set; }

    /// <summary>
    /// ѡ����Ȩ��
    /// </summary>
    float BuildWeight { get; set; }

    
    // ----------------------------Ȩ��ѡ�� Level1-----------------------------

    /// <summary>
    /// ѡ��̹��Ȩ��
    /// </summary>
    float TankWeight { get; set; }

    /// <summary>
    /// ѡ�������ؾ�Ȩ��
    /// </summary>
    float LVWeight { get; set; }

    /// <summary>
    /// ѡ�����Ȩ��
    /// </summary>
    float CannonWeight { get; set; }

    /// <summary>
    /// ѡ�������Ȩ��
    /// </summary>
    float AirCraftWeight { get; set; }

    /// <summary>
    /// ѡ�񲽱�Ȩ��
    /// </summary>
    float SoldierWeight { get; set; }


    // ----------------------------Ȩ��ѡ�� Level3-----------------------------
    /// <summary>
    /// ѡ�����ε�λȨ��
    /// </summary>
    float HideWeight { get; set; }

    /// <summary>
    /// ѡ�񳰷�Ȩ��(���ֵӦ�úܴ�, �����з�����Ч���ĵ�λ)
    /// </summary>
    float TauntWeight { get; set; }


    // ----------------------------Ȩ��ѡ�� Level4-----------------------------

    
    /// <summary>
    /// ������Ȩ��
    /// </summary>
    float HealthMinWeight { get; set; }

    /// <summary>
    /// ������Ȩ��
    /// </summary>
    float HealthMaxWeight { get; set; }


    /// <summary>
    /// ��λ��Ȩ��
    /// </summary>
    float DistanceMinWeight { get; set; }

    /// <summary>
    /// Զλ��Ȩ��
    /// </summary>
    float DistanceMaxWeight { get; set; }

    /// <summary>
    /// �Ƕ�Ȩ��
    /// </summary>
    float AngleWeight { get; set; }


}