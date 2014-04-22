using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Eltc.Base.FrameWork.Helper;
using Com.Dianping.Cat;

namespace WPW.Demo.Wcf.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory + "config\\catclient.xml";
            Cat.Initialize(filename);

            WcfServer server = WcfServer.GetInstance();
            server.WcfFaultedEvent += new WcfFaulted(server_WcfFaultedEvent);
            server.WcfClosedEvent += new WcfClosed(server_WcfClosedEvent);
            server.WcfBeforeCallEvent += new WcfBeforeCall(server_WcfBeforeCallEvent);
            server.WcfAfterCallEvent += new WcfAfterCall(server_WcfAfterCallEvent);
            server.Start();
            Console.WriteLine("服务启动...");

            Console.ReadKey();

            server.Stop();
        }

        static void server_WcfAfterCallEvent(string operationName, object[] outputs, object returnValue, object correlationState, string AbsolutePath)
        {
            //Console.WriteLine("返回操作结束：" + AbsolutePath + "/" + operationName);
            Console.WriteLine("返回操作结束：" + correlationState.ToString());
        }

        static void server_WcfBeforeCallEvent(string operationName, object[] inputs, string AbsolutePath, object correlationState)
        {
            //Console.WriteLine("返回操作开始：" + AbsolutePath + "/" + operationName);
            Console.WriteLine("返回操作开始：" + correlationState.ToString());
        }

        static void server_WcfClosedEvent(object sender, EventArgs e)
        {
            //Console.WriteLine(e.ToString());
        }

        static void server_WcfFaultedEvent(object sender, EventArgs e)
        {
            //Console.WriteLine(e.ToString());
        }
    }
}
