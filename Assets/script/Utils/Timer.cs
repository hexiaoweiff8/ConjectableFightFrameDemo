using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    /// <summary>
    /// 定时器
    /// </summary>
    public class Timer
    {

        // ---------------------------公有属性-----------------------------

        /// <summary>
        /// 暂停
        /// </summary>
        public bool IsPause { get; private set; }

        /// <summary>
        /// 是否循环执行
        /// </summary>
        public bool IsLoop { get; private set; }

        /// <summary>
        /// 执行循环时间
        /// </summary>
        public float LoopTime { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public float OutTime { get; private set; }

        /// <summary>
        /// 启动时间
        /// </summary>
        public double StartTime { get; private set; }


        /// <summary>
        /// 时间到执行
        /// </summary>
        public Action TimesUpDo
        {
            get;
            set;
        }

        /// <summary>
        /// kill事件
        /// </summary>
        public Action KillDo { get; set; }

        /// <summary>
        /// 是否
        /// </summary>
        public bool IsStop { get; private set; }

        /// <summary>
        /// 停止时间
        /// </summary>
        public double StopTime
        {
            get { return stopTime; }
        }


        // ----------------------------私有属性--------------------------------

        /// <summary>
        /// 停止时间
        /// </summary>
        private double stopTime = 0;


        // -------------------------公有方法----------------------------

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="liveTime">存活时间(单位 秒) 如果isLoop为true则该值为循环间隔</param>
        /// <param name="isLoop">是否循环执行, 如果为true则每隔"存活时间"执行一次直至Kill掉(可能内存泄漏), 否则只执行一次</param>
        public Timer(float liveTime, bool isLoop = false)
        {
            LoopTime = liveTime;
            IsLoop = isLoop;
            OutTime = -1f;
        }

        /// <summary>
        /// 初始化
        /// 默认循环
        /// </summary>
        /// <param name="liveTime">存活时间(单位 秒) 如果isLoop为true则该值为循环间隔</param>
        public Timer(float liveTime, float outTime)
        {
            LoopTime = liveTime;
            IsLoop = true;
            OutTime = outTime;
        }


        public Timer Start()
        {
            //计时器复用
            if (IsStop)
                IsStop = false;
            // 加入TimeManager中
            stopTime = TimerManager.Single.StartTimerTick + LoopTime;
            StartTime = TimerManager.Single.StartTimerTick;
            TimerManager.Single.AddTimer(this);
            return this;
        }

        /// <summary>
        /// 计时回调
        /// 时间到调用
        /// </summary>
        /// <param name="completeCallback">调用Callback Action</param>
        public Timer OnCompleteCallback(Action completeCallback)
        {
            TimesUpDo += completeCallback;
            return this;
        }

        /// <summary>
        /// kill时执行时间
        /// </summary>
        /// <param name="onKill"></param>
        /// <returns></returns>
        public Timer OnKill(Action onKill)
        {
            KillDo = onKill;
            return this;
        }

        /// <summary>
        /// 干掉计时器
        /// </summary>
        public void Kill()
        {
            if (KillDo != null)
            {
                KillDo();
            }
            IsStop = true;
        }

        /// <summary>
        /// 时间移至下一次
        /// </summary>
        public void NextTick()
        {
            if (IsLoop)
            {
                stopTime = TimerManager.Single.StartTimerTick + LoopTime;
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            IsPause = true;
        }

        /// <summary>
        /// 继续
        /// </summary>
        public void GoOn()
        {
            IsPause = false;
        }
       

    }


    /// <summary>
    /// 定时循环器
    /// </summary>
    public class TimerManager : ILoopItem
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static TimerManager Single
        {
            get
            {
                if (single == null)
                {
                    single = new TimerManager();
                }
                return single;
            }
        }

        /// <summary>
        /// 单例对象
        /// </summary>
        private static TimerManager single = null;

        /// <summary>
        /// 计时器Tick
        /// </summary>
        public double StartTimerTick {
            get
            {
                TimeSpan sp = DateTime.Now - startTime;
                return sp.TotalSeconds;
            }
        }

        /// <summary>
        /// 活动中计时器列表
        /// </summary>
        private SortedDictionary<double, List<Timer>> activityTimerList = new SortedDictionary<double, List<Timer>>();

        /// <summary>
        /// 计时器启动时间
        /// </summary>
        private DateTime startTime = DateTime.Now;


        public TimerManager()
        {
            // 将计时器管理器加入Loop中开始运行
            LooperManager.Single.Add(this);
        }

        /// <summary>
        /// 添加定时器
        /// </summary>
        /// <param name="timer">定时器对象</param>
        public void AddTimer(Timer timer)
        {
            if (timer == null)
            {
                return;
            }

            var stopTime = timer.StopTime;
            List<Timer> keyMapTimerList = null;
            if (!activityTimerList.ContainsKey(stopTime))
            {
                keyMapTimerList = new List<Timer>();
                activityTimerList.Add(stopTime, keyMapTimerList);
            }
            else
            {
                keyMapTimerList = activityTimerList[stopTime];
            }

            keyMapTimerList.Add(timer);
        }

        /// <summary>
        /// 删除计时器
        /// </summary>
        /// <param name="timer">被删除timer</param>
        public void RemoveTimer(Timer timer)
        {
            if (timer == null || activityTimerList.Count == 0)
            {
                return;
            }

            if (activityTimerList.ContainsKey(timer.StopTime))
            {
                var itemForTimerList = activityTimerList[timer.StopTime];
                itemForTimerList.Remove(timer);
            }
        }

        /// <summary>
        /// 单次循环
        /// </summary>
        public void Do()
        {
            var time = StartTimerTick;
            while (DoOnce(time)) ;
        }


        public bool DoOnce(double time)
        {
            // 获取最小值
            if (activityTimerList.Count == 0)
            {
                return false;
            }
            // 获取排序过的列表头
            var itemForTimerList = activityTimerList.GetEnumerator();
            itemForTimerList.MoveNext();
            var current = itemForTimerList.Current;
            var key = current.Key;
            if (key > time)
            {
                // 未到时间
                return false;
            }

            // 删除列表
            activityTimerList.Remove(key);

            var timerList = current.Value;
            foreach (var timer in timerList)
            {
                if (timer.TimesUpDo != null && !timer.IsStop)
                {
                    if (!timer.IsPause)
                    {
                        timer.TimesUpDo();
                    }

                    // 如果是循环执行并且未超时则将其移至下一次执行
                    if ((timer.IsLoop && (timer.OutTime < 0 || timer.OutTime > time - timer.StartTime)) 
                        || timer.IsPause)
                    {
                        timer.NextTick();
                        AddTimer(timer);
                    }
                    else
                    {
                        // 执行OnKill
                        if (timer.KillDo != null)
                        {
                            timer.KillDo();
                        }
                    }
                }
            }
            timerList = null;
            return true;
        }

        /// <summary>
        /// 是否执行完毕
        /// </summary>
        /// <returns>是否执行完毕</returns>
        public bool IsEnd()
        {
            // 不会结束, 贯穿全程
            return false;
        }

        /// <summary>
        /// 被销毁时执行
        /// </summary>
        public void OnDestroy()
        {
            // 清空列表
            foreach (var kv in activityTimerList)
            {
                foreach (var timer in kv.Value)
                {
                    timer.Kill();
                }
            }
        }

    }
}