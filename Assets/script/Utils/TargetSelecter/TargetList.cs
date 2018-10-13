using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ŀ���б�
/// </summary>
/// <typeparam name="T"></typeparam>
public class TargetList<T> where T : IGraphicsHolder//IGraphical<Rectangle>
{
    /// <summary>
    /// ����ȫ�����б�
    /// </summary>
    public IList<T> List { get { return list; } } 

    /// <summary>
    /// �����Ĳ����б�
    /// </summary>
    public QuadTree<T> QuadTree { get { return quadTree; } }

    /// <summary>
    /// Ŀ�����б�
    /// </summary>
    private IList<T> list = null;

    /// <summary>
    /// �Ĳ���
    /// </summary>
    private QuadTree<T> quadTree = null;

    /// <summary>
    /// ��ͼ��Ϣ
    /// </summary>
    private MapBase mapBase = null;


    /// <summary>
    /// ����Ŀ���б�
    /// </summary>
    /// <param name="x">��ͼλ��x</param>
    /// <param name="y">��ͼλ��y</param>
    /// <param name="width">��ͼ���</param>
    /// <param name="height">��ͼ�߶�</param>
    /// <param name="unitWidht"></param>
    public TargetList(float x, float y, int width, int height, int unitWidht)
    {
        var mapRect = new RectGraphics(new Vector2(x, y), width * unitWidht, height * unitWidht, 0);
        quadTree = new QuadTree<T>(0, mapRect);
        list = new List<T>();
    }

    /// <summary>
    /// ��ӵ�Ԫ
    /// </summary>
    /// <param name="t">��Ԫ����, ����T</param>
    public void Add(T t)
    {
        // �ն��󲻼������
        if (t == null)
        {
            return;
        }
        if (list.Contains(t))
        {
            Debug.LogError("��λ���б����Ѵ���");
            return;
        }
        // ����ȫ���б�
        list.Add(t);
        // �����Ĳ���
        quadTree.Insert(t);
    }

    /// <summary>
    /// ɾ����λ
    /// </summary>
    /// <param name="t"></param>
    public void Remove(T t)
    {
        // �ն��󲻼������
        if (t == null)
        {
            return;
        }
        list.Remove(t);
        // TODO Ŀǰʹ���ع�����ʽ����Ĳ���ɾ����λ, �и��÷������滻
        RebuildQuadTree();
    }


    /// <summary>
    /// ���ݷ�Χ��ȡ����
    /// </summary>
    /// <param name="graphics">��Χ����, �����ж���ײ</param>
    /// <returns></returns>
    public IList<T> GetListWithRectangle(ICollisionGraphics graphics)
    {
        // ���ط�Χ�ڵĶ����б�
        return quadTree.Retrieve(graphics);
    }

    /// <summary>
    /// ���¹����Ĳ���
    /// ʹ�����: �б��ж���λ���ѱ��ʱ
    /// </summary>
    public void RebuildQuadTree()
    {
        quadTree.Clear();
        quadTree.Insert(list);
    }


    public void Refresh()
    {
        quadTree.Refresh();
    }


    //public void RebulidMapInfo()
    //{
    //    if (mapinfo != null)
    //    {
    //        mapinfo.RebuildMapInfo(list);
    //    }
    //}

    /// <summary>
    /// ��������
    /// </summary>
    public void Clear()
    {
        if (list != null)
        {
            list.Clear();
        }
        if (quadTree != null)
        {
            quadTree.Clear();
        }
    }

}