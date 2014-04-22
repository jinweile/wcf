using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

using WcfStatistics.Model;
using WcfStatistics.Dao;
using WcfStatistics.Dao.Int;

using Eltc.Base.FrameWork.Helper.Wcf.Monitor;

namespace WcfStatisticsServiceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //首先获取wcf监控服务器的信息
            IWcfServerDao serverdao = CastleContext.Instance.GetService<IWcfServerDao>();
            IList<WcfServer> serverlist = serverdao.FindAll();

            //首先建立批次信息
            Batch batch = new Batch();
            batch.CreateTime = DateTime.Now;
            IBatchDao batchdao = CastleContext.Instance.GetService<IBatchDao>();
            int batchid = batchdao.Insert(batch);

            IWcfServerPerformanceDao perfdao = CastleContext.Instance.GetService<IWcfServerPerformanceDao>();
            IClientConnInfoDao clientdao = CastleContext.Instance.GetService<IClientConnInfoDao>();
            IOperationInfoDao operatedao = CastleContext.Instance.GetService<IOperationInfoDao>();

            foreach (WcfServer server in serverlist)
            {
                string ip = server.IP + ":" + server.Point;
                PCData pcdata;
                double memCount;
                Dictionary<string, LinkModel> modellist = getLinkModel(ip, out pcdata, out memCount);
                int all_connnums = 0;

                foreach (string key in modellist.Keys)
                {
                    LinkModel model = modellist[key];
                    Dictionary<string, UrlInfo> infolist = model.UrlInfoList;

                    foreach (string url in infolist.Keys)
                    {
                        UrlInfo info = infolist[url];
                        all_connnums += info.ConnNums;

                        foreach (string opereateName in info.OperateNums.Keys)
                        {
                            //加入操作信息表
                            OperationInfo oinfo = new OperationInfo();
                            oinfo.Adress = url;
                            oinfo.BatchID = batchid;
                            oinfo.IP = model.ClientIP.Split('_')[0];
                            oinfo.OperationName = opereateName;
                            oinfo.OperationNums = info.OperateNums[opereateName];
                            oinfo.ServerID = server.ID;
                            oinfo.AppName = model.ClientIP.Split('_')[1];

                            operatedao.Insert(oinfo);
                        }

                        //加入客户端连接信息表
                        ClientConnInfo clientinfo = new ClientConnInfo();
                        clientinfo.Adress = url;
                        clientinfo.BatchID = batchid;
                        clientinfo.IP = model.ClientIP.Split('_')[0];
                        clientinfo.LinkNums = info.ConnNums;
                        clientinfo.ServerID = server.ID;
                        clientinfo.AppName = model.ClientIP.Split('_')[1];

                        clientdao.Insert(clientinfo);
                    }
                    
                }

                //存入服务器性能表
                WcfServerPerformance sperf = new WcfServerPerformance();
                sperf.BatchID = batchid;
                sperf.Cpu = (decimal)pcdata.Cpu;
                sperf.AllMem = (decimal)memCount;
                sperf.Mem = (decimal)pcdata.Mem;
                sperf.ProcessId = pcdata.ProcessId;
                sperf.ServerID = server.ID;
                sperf.ThreadCount = pcdata.ThreadCount;
                sperf.CurrentConnNums = all_connnums;

                perfdao.Insert(sperf);
            }
        }

        private static Dictionary<string, LinkModel> getLinkModel(string ip, out PCData pcdata, out double memCount)
        {
            NetTcpBinding binging = new NetTcpBinding();
            binging.MaxBufferPoolSize = 524288;
            binging.MaxBufferSize = 2147483647;
            binging.MaxReceivedMessageSize = 2147483647;
            binging.MaxConnections = 1000;
            binging.ListenBacklog = 200;
            binging.OpenTimeout = TimeSpan.Parse("00:10:00");
            binging.ReceiveTimeout = TimeSpan.Parse("00:10:00");
            binging.TransferMode = TransferMode.Buffered;
            binging.Security.Mode = SecurityMode.None;

            binging.SendTimeout = TimeSpan.Parse("00:10:00");
            binging.ReaderQuotas.MaxDepth = 64;
            binging.ReaderQuotas.MaxStringContentLength = 2147483647;
            binging.ReaderQuotas.MaxArrayLength = 16384;
            binging.ReaderQuotas.MaxBytesPerRead = 4096;
            binging.ReaderQuotas.MaxNameTableCharCount = 16384;
            binging.ReliableSession.Ordered = true;
            binging.ReliableSession.Enabled = false;
            binging.ReliableSession.InactivityTimeout = TimeSpan.Parse("00:10:00");

            Dictionary<string, LinkModel> modellist = new Dictionary<string, LinkModel>();
            using (MonitorClient client = MonitorClient.Instance)
            {
                IMonitorControl proxy = client.getProxy(ip, binging);
                modellist = proxy.GetMonitorInfo(out pcdata, out memCount);
            }

            return modellist;
        }
    }
}
