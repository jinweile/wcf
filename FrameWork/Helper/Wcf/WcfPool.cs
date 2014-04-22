using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Threading;
using System.Collections;
using System.Diagnostics;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    internal class WcfPool
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="WcfMaxPoolSize">wcf池子最大数</param>
        /// <param name="WcfTimeOut">wcf获取连接超时时间，以秒为单位</param>
        public WcfPool(int WcfMaxPoolSize, long WcfOutTime, long WcfFailureTime, int WcfPoolMonitorReapTime)
        {
            this.wcfMaxPoolSize = WcfMaxPoolSize;
            this.wcfOutTime = new TimeSpan(WcfOutTime * 1000 * 10000);
            this.wcfFailureTime = new TimeSpan(WcfFailureTime * 1000 * 10000);
            this.monitorTimeSpan = WcfPoolMonitorReapTime;
            poollist = new List<WcfCommunicationObj>(this.wcfMaxPoolSize);
            usedNumsDic = new Dictionary<string, int>();
            countNumsDic = new Dictionary<string, int>();
        }

        ~WcfPool()
        {
            ClearPool();
        }

        /// <summary>
        /// Wcf连接池最大值，默认为100
        /// </summary>
        int wcfMaxPoolSize = 100;

        /// <summary>
        /// Wcf获取连接过期时间(默认一分钟)
        /// </summary>
        TimeSpan wcfOutTime = new TimeSpan((long)60 * 1000 * 10000);

        /// <summary>
        /// Wcf连接失效时间(默认一分钟)
        /// </summary>
        TimeSpan wcfFailureTime = new TimeSpan((long)60 * 1000 * 10000);

        /// <summary>
        /// 监控时间间隔（单位：秒）
        /// </summary>
        int monitorTimeSpan = 30;

        /// <summary>
        /// 监控逻辑
        /// </summary>
        public void MonitorExec()
        {
            while (true)
            {
                //write("F:\\1.txt", "monitor");
                Thread.Sleep(monitorTimeSpan * 1000);
                try
                {
                    ReapPool();
                }
                catch { }
            }
        }

        /// <summary>
        /// 清空连接池
        /// </summary>
        public void ClearPool()
        {
            lock (lockhelper)
            {
                foreach (WcfCommunicationObj obj in this.poollist)
                {
                    try 
                    {
                        //obj.Scope.Dispose();
                        obj.CommucationObject.Close(); 
                    }
                    catch { obj.CommucationObject.Abort(); }
                }
                poollist.Clear();
                poollist = null;
                countNumsDic.Clear();
                countNumsDic = null;
                usedNumsDic.Clear();
                usedNumsDic = null;
                index = 0;
            }
        }

        /// <summary>
        /// 当前正在使用的池子数量
        /// </summary>
        public int GetUsedPoolNums(string contract)
        {
            if (usedNumsDic.ContainsKey(contract))
            {
                return usedNumsDic[contract];
            }
            return 0;
        }

        /// <summary>
        /// 当前空闲池子数量
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public int GetFreePoolNums(string contract)
        {
            return GetCountPoolNums(contract) - GetUsedPoolNums(contract);
        }

        /// <summary>
        /// 判断非当前契约是否有空闲池子
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public bool GetFreePoolNumsNotCurrent(string contract)
        {
            bool flag = false;
            foreach (WcfCommunicationObj obj in poollist)
            {
                if (!obj.IsUsed && obj.Contract != contract)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        /// <summary>
        /// 获取池子总数
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public int GetCountPoolNums(string contract)
        {
            if (countNumsDic.ContainsKey(contract))
            {
                return countNumsDic[contract];
            }
            return 0;
        }

        /// <summary>
        /// 当前池子连接总数
        /// </summary>
        public int CurrentPoolNums
        {
            get
            {
                return poollist.Count;
            }
        }

        /// <summary>
        /// 判断池子是否满了
        /// </summary>
        /// <returns></returns>
        public bool IsPoolFull
        {
            get
            {
                return poollist.Count >= this.wcfMaxPoolSize;
            }
        }

        /// <summary>
        /// 通讯实体列表
        /// </summary>
        private List<WcfCommunicationObj> poollist = null;
        private object lockhelper = new object();
        /// <summary>
        /// 索引
        /// </summary>
        private int index = 0;
        /// <summary>
        /// 已经使用的数量
        /// </summary>
        private IDictionary<string, int> usedNumsDic = null;
        /// <summary>
        /// 池子里个类型连接总数
        /// </summary>
        private IDictionary<string, int> countNumsDic = null;

        /// <summary>
        /// 处理连接池
        /// </summary>
        private void ReapPool()
        {
            lock (lockhelper)
            {
                //string content = "";
                for (int i = 0; i < poollist.Count; i++)
                {
                    WcfCommunicationObj obj = poollist[i];
                    if ((!obj.IsUsed && DateTime.Now - obj.CreatedTime > this.wcfFailureTime) || (obj.CommucationObject.State != CommunicationState.Opened))
                    {
                        try
                        {
                            //obj.Scope.Dispose();
                            obj.CommucationObject.Close(); 
                        }
                        catch { obj.CommucationObject.Abort(); }
                        poollist.Remove(obj);
                        if (countNumsDic.ContainsKey(obj.Contract))
                            countNumsDic[obj.Contract] = countNumsDic[obj.Contract] == 0 ? 0 : countNumsDic[obj.Contract] - 1;
                        if (usedNumsDic.ContainsKey(obj.Contract))
                            usedNumsDic[obj.Contract] = usedNumsDic[obj.Contract] == 0 ? 0 : usedNumsDic[obj.Contract] - 1;

                        i--;
                    }
                }
                //write("F:\\2.txt", content);
            }
        }

        /// <summary>
        /// 加入连接池
        /// </summary>
        /// <typeparam name="T">连接契约类型</typeparam>
        /// <param name="channel">连接</param>
        /// <param name="Index">返回的连接池索引</param>
        /// <returns></returns>
        public bool AddPool<T>(ChannelFactory<T> factory, out T channel, out int? Index, bool isReap)
        {
            //做一次清理
            //if (isReap)
            //    ReapPool();

            bool flag = false;
            Index = null;
            channel = default(T);
            
            if (poollist.Count < this.wcfMaxPoolSize)
            {
                lock (lockhelper)
                {
                    if (poollist.Count < this.wcfMaxPoolSize)
                    {
                        channel = factory.CreateChannel();
                        ICommunicationObject communicationobj = channel as ICommunicationObject;
                        communicationobj.Open();
                        WcfCommunicationObj obj = new WcfCommunicationObj();
                        index = index >= Int32.MaxValue ? 1 : index + 1;
                        Index = index;
                        obj.Index = index;
                        obj.UsedNums = 1;
                        obj.CommucationObject = communicationobj;
                        obj.Contract = typeof(T).FullName;
                        obj.CreatedTime = DateTime.Now;
                        obj.LastUsedTime = DateTime.Now;
                        obj.IsUsed = true;
                        //obj.Scope = new OperationContextScope(((IClientChannel)channel));
                        poollist.Add(obj);
                        countNumsDic[obj.Contract] = countNumsDic.ContainsKey(obj.Contract) ? countNumsDic[obj.Contract] + 1 : 1;
                        usedNumsDic[obj.Contract] = usedNumsDic.ContainsKey(obj.Contract) ? usedNumsDic[obj.Contract] + 1 : 1;
                        flag = true;
                    }
                }
            }
            return flag;
        }

       /// <summary>
        /// 从连接池中获取一个连接
       /// </summary>
       /// <typeparam name="T">获取的契约</typeparam>
       /// <returns></returns>
        public WcfCommunicationObj GetChannel<T>()
        {
            //先做一次清理
            //ReapPool();

            string t = typeof(T).FullName;
            WcfCommunicationObj channel = null;
            if (GetFreePoolNums(t) > 0)
            {
                lock (lockhelper)
                {
                    if (GetFreePoolNums(t) > 0)
                    {
                        for(int i=0;i<poollist.Count;i++)
                        {
                            WcfCommunicationObj obj = poollist[i];
                            if (!obj.IsUsed && DateTime.Now - obj.CreatedTime < this.wcfFailureTime && t == obj.Contract)
                            {
                                if (obj.CommucationObject.State == CommunicationState.Opened)
                                {
                                    obj.IsUsed = true;
                                    obj.UsedNums++;
                                    obj.LastUsedTime = DateTime.Now;
                                    usedNumsDic[obj.Contract] = usedNumsDic.ContainsKey(obj.Contract) ? usedNumsDic[obj.Contract] + 1 : 1;

                                    channel = obj;
                                    break;
                                }
                                else//如果当前连接无效，则清理出连接池
                                {
                                    try
                                    {
                                        //obj.Scope.Dispose();
                                        obj.CommucationObject.Close();
                                    }
                                    catch { obj.CommucationObject.Abort(); }
                                    poollist.Remove(obj);
                                    if (countNumsDic.ContainsKey(obj.Contract))
                                        countNumsDic[obj.Contract] = countNumsDic[obj.Contract] == 0 ? 0 : countNumsDic[obj.Contract] - 1;
                                    if (usedNumsDic.ContainsKey(obj.Contract))
                                        usedNumsDic[obj.Contract] = usedNumsDic[obj.Contract] == 0 ? 0 : usedNumsDic[obj.Contract] - 1;
                                    i--;
                                }
                            }
                        }
                    }
                }
            }

            return channel;
        }

        /// <summary>
        /// 把连接放回池子
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ReturnPool<T>(int Index)
        {
            string t = typeof(T).FullName;
            lock (lockhelper)
            {
                foreach (WcfCommunicationObj obj in poollist)
                {
                    if (Index == obj.Index && t == obj.Contract)
                    {
                        obj.IsUsed = false;
                        obj.LastUsedTime = DateTime.Now;
                        if (usedNumsDic.ContainsKey(obj.Contract))
                            usedNumsDic[obj.Contract] = usedNumsDic[obj.Contract] == 0 ? 0 : usedNumsDic[obj.Contract] - 1;
                        break;
                    }
                }
            }

            //做一次清理
            //ReapPool();
        }

        /// <summary>
        /// 移除索引的连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Index"></param>
        /// <returns></returns>
        public bool RemovePoolAt<T>(int Index)
        {
            bool flag = false;
            string t = typeof(T).FullName;
            lock (lockhelper)
            {
                int len = poollist.Count;
                for (int i = 0; i < poollist.Count; i++)
                {
                    WcfCommunicationObj obj = poollist[i];
                    if (Index == obj.Index && t == obj.Contract)
                    {
                        try 
                        {
                            //obj.Scope.Dispose();
                            obj.CommucationObject.Close(); 
                        }
                        catch { obj.CommucationObject.Abort(); }
                        poollist.Remove(obj);
                        if (countNumsDic.ContainsKey(obj.Contract))
                            countNumsDic[obj.Contract] = countNumsDic[obj.Contract] == 0 ? 0 : countNumsDic[obj.Contract] - 1;
                        if (usedNumsDic.ContainsKey(obj.Contract))
                            usedNumsDic[obj.Contract] = usedNumsDic[obj.Contract] == 0 ? 0 : usedNumsDic[obj.Contract] - 1;

                        flag = true;
                        i--;
                        break;
                    }
                }
            }

            return flag;
        }

        /// <summary>
        /// 踢掉一个非当前契约的空闲连接
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public bool RemovePoolOneNotAt<T>(ChannelFactory<T> factory, out T channel, out int? Index)
        {
            bool flag = false;
            Index = null;
            channel = default(T);

            string contract = typeof(T).FullName;
            lock (lockhelper)
            {
                int len = poollist.Count;
                //如果池子满了，先踢出一个非当前创建契约的连接
                if (poollist.Count >= this.wcfMaxPoolSize)
                {
                    for (int i = 0; i < poollist.Count; i++)
                    {
                        WcfCommunicationObj obj = poollist[i];
                        if (!obj.IsUsed && obj.Contract != contract)
                        {
                            try 
                            {
                                //obj.Scope.Dispose();
                                obj.CommucationObject.Close(); 
                            }
                            catch { obj.CommucationObject.Abort(); }
                            poollist.Remove(obj);
                            if (countNumsDic.ContainsKey(obj.Contract))
                                countNumsDic[obj.Contract] = countNumsDic[obj.Contract] == 0 ? 0 : countNumsDic[obj.Contract] - 1;
                            if (usedNumsDic.ContainsKey(obj.Contract))
                                usedNumsDic[obj.Contract] = usedNumsDic[obj.Contract] == 0 ? 0 : usedNumsDic[obj.Contract] - 1;

                            flag = true;
                            i--;
                            break;
                        }
                    }
                }
                //增加一个连接到池子
                if (poollist.Count < this.wcfMaxPoolSize)
                {
                    channel = factory.CreateChannel();
                    ICommunicationObject communicationobj = channel as ICommunicationObject;
                    communicationobj.Open();
                    WcfCommunicationObj obj = new WcfCommunicationObj();
                    index = index >= Int32.MaxValue ? 1 : index + 1;
                    Index = index;
                    obj.Index = index;
                    obj.UsedNums = 1;
                    obj.CommucationObject = communicationobj;
                    obj.Contract = contract;
                    obj.CreatedTime = DateTime.Now;
                    obj.LastUsedTime = DateTime.Now;
                    obj.IsUsed = true;
                    //obj.Scope = new OperationContextScope(((IClientChannel)channel));
                    poollist.Add(obj);
                    countNumsDic[obj.Contract] = countNumsDic.ContainsKey(obj.Contract) ? countNumsDic[obj.Contract] + 1 : 1;
                    usedNumsDic[obj.Contract] = usedNumsDic.ContainsKey(obj.Contract) ? usedNumsDic[obj.Contract] + 1 : 1;
                    flag = true;
                }
            }

            return flag;
        }

    }

    /// <summary>
    /// Wcf通讯实体
    /// </summary>
    internal class WcfCommunicationObj
    {
        private int index;
        /// <summary>
        /// 索引
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        string contract;
        /// <summary>
        /// 契约类型
        /// </summary>
        public string Contract
        {
            get { return contract; }
            set { contract = value; }
        }

        //private OperationContextScope scope;
        ///// <summary>
        ///// wcf上下文对象
        ///// </summary>
        //public OperationContextScope Scope
        //{
        //    get { return scope; }
        //    set { scope = value; }
        //}

        ICommunicationObject commucationObject;
        /// <summary>
        /// 通讯对象
        /// </summary>
        public ICommunicationObject CommucationObject
        {
            get { return commucationObject; }
            set { commucationObject = value; }
        }

        bool isUsed = false;
        /// <summary>
        /// 是否在使用
        /// </summary>
        public bool IsUsed
        {
            get { return isUsed; }
            set { isUsed = value; }
        }

        DateTime createdTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime
        {
            get { return createdTime; }
            set { createdTime = value; }
        }

        DateTime lastUsedTime;
        /// <summary>
        /// 最后使用时间
        /// </summary>
        public DateTime LastUsedTime
        {
            get { return lastUsedTime; }
            set { lastUsedTime = value; }
        }

        int usedNums;
        /// <summary>
        /// 使用次数
        /// </summary>
        public int UsedNums
        {
            get { return usedNums; }
            set { usedNums = value; }
        }
    }

}
