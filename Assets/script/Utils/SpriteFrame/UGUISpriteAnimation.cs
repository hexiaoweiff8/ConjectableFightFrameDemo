using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// 序列帧
/// </summary>
[RequireComponent(typeof(Image))]
public class UGUISpriteAnimation : MonoBehaviour
{
    /// <summary>
    /// Image组件
    /// </summary>
    private Image ImageSource;
    /// <summary>
    /// 当前帧位置
    /// </summary>
    private int mCurFrame = 0;
    /// <summary>
    /// 时间累积
    /// </summary>
    private float mDelta = 0;

    /// <summary>
    /// 播放速度
    /// </summary>
    public float FPS = 5;
    /// <summary>
    /// 图列表
    /// </summary>
    public List<Sprite> SpriteFrames;
    /// <summary>
    /// 播放状态
    /// </summary>
    public bool IsPlaying = false;
    /// <summary>
    /// 是否正向播放
    /// </summary>
    public bool Foward = true;
    /// <summary>
    /// 是否自动播放
    /// </summary>
    public bool AutoPlay = false;
    /// <summary>
    /// 是否循环
    /// </summary>
    public bool Loop = false;

    /// <summary>
    /// 帧数
    /// </summary>
    public int FrameCount
    {
        get
        {
            return SpriteFrames.Count;
        }
    }

    void Awake()
    {
        // 获取Image
        ImageSource = GetComponent<Image>();
    }

    void Start()
    {
        // 是否播放
        if (AutoPlay)
        {
            Play();
        }
        else
        {
            IsPlaying = false;
        }
    }

    /// <summary>
    /// 设置当前位置
    /// </summary>
    /// <param name="idx"></param>
    private void SetSprite(int idx)
    {
        ImageSource.sprite = SpriteFrames[idx];
        //该部分为设置成原始图片大小，如果只需要显示Image设定好的图片大小，注释掉该行即可。
        //ImageSource.SetNativeSize();
    }

    /// <summary>
    /// 播放
    /// </summary>
    public void Play()
    {
        IsPlaying = true;
        Foward = true;
    }

    /// <summary>
    /// 反向播放
    /// </summary>
    public void PlayReverse()
    {
        IsPlaying = true;
        Foward = false;
    }

    void Update()
    {
        // 验证状态
        if (!IsPlaying || 0 == FrameCount)
        {
            return;
        }

        mDelta += Time.deltaTime;
        // 控制速度
        if (mDelta > 1 / FPS)
        {
            mDelta = 0;
            if (Foward)
            {
                mCurFrame++;
            }
            else
            {
                mCurFrame--;
            }

            if (mCurFrame >= FrameCount)
            {
                if (Loop)
                {
                    mCurFrame = 0;
                }
                else
                {
                    IsPlaying = false;
                    return;
                }
            }
            else if (mCurFrame < 0)
            {
                if (Loop)
                {
                    mCurFrame = FrameCount - 1;
                }
                else
                {
                    IsPlaying = false;
                    return;
                }
            }

            SetSprite(mCurFrame);
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        IsPlaying = false;
    }

    /// <summary>
    /// 继续播放
    /// </summary>
    public void Resume()
    {
        if (!IsPlaying)
        {
            IsPlaying = true;
        }
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    public void Stop()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        IsPlaying = false;
    }

    /// <summary>
    /// 重新播放
    /// </summary>
    public void Rewind()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        Play();
    }
}