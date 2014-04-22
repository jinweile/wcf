using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;

namespace Eltc.Base.FrameWork.Helper.Wcf.Monitor
{
    public class MonitorControl : CommonWcfBll, IMonitorControl
    {
        /// <summary>
        /// 系统内存大小
        /// </summary>
        private static double _memcount = 1;
        private static volatile object lockhelper = new object();

        /// <summary>
        /// 获取wcf监控信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, LinkModel> GetMonitorInfo(out PCData pcdata, out double memCount)
        {
            Process CurrentProcess = Process.GetCurrentProcess();
            pcdata = new PCData();
            try
            {
                pcdata.ProcessId = CurrentProcess.Id;
            }
            catch { }
            try
            {
                //创建性能计数器
                using (var p_cpu = new PerformanceCounter("Process", "% Processor Time", CurrentProcess.ProcessName))
                {
                    //注意除以CPU数量
                    pcdata.Cpu = p_cpu.NextValue() / (float)Environment.ProcessorCount;
                }
            }
            catch { }
            try
            {
                using (var p_mem = new PerformanceCounter("Process", "Working Set - Private", CurrentProcess.ProcessName))
                {
                    pcdata.Mem = p_mem.NextValue() / (1024 * 1024);
                }
            }
            catch { }
            try
            {
                pcdata.ThreadCount = CurrentProcess.Threads.Count;
            }
            catch { }
            try
            {
                #region 获取系统总内存
                if (_memcount == 1)
                {
                    lock (lockhelper)
                    {
                        if (_memcount == 1)
                        {
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher();   //用于查询一些如系统信息的管理对象 
                            searcher.Query = new SelectQuery("Win32_PhysicalMemory ", "", new string[] { "Capacity" });//设置查询条件 
                            ManagementObjectCollection collection = searcher.Get();   //获取内存容量 
                            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

                            long capacity = 0;
                            while (em.MoveNext())
                            {
                                ManagementBaseObject baseObj = em.Current;
                                if (baseObj.Properties["Capacity"].Value != null)
                                {
                                    try
                                    {
                                        capacity += long.Parse(baseObj.Properties["Capacity"].Value.ToString());
                                    }
                                    catch { }
                                }
                            }
                            _memcount = (double)capacity / (double)1024 / (double)1024;
                        }
                    }
                }
                #endregion
            }
            catch { }

            memCount = _memcount;
            return MonitorData.Instance.getMonitorInfo();
        }
    }
}
