using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core.Resource;

namespace WcfStatistics.Dao
{
    public class CastleContext
    {
        /// <summary>
        /// 容器实例对象
        /// </summary>
        private static volatile WindsorContainer instance = null;

        /// <summary>
        /// 锁
        /// </summary>
        private static volatile object olock = new object();

        /// <summary>
        /// 获取Castle容器的实例
        /// </summary>
        public static WindsorContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (olock)
                    {
                        if (instance == null)
                        {
                            instance = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// 重新加载容器的实例
        /// </summary>
        public static void ReloadWinsorContainer()
        {
            lock (olock)
            {
                if (instance != null)
                {
                    instance.Dispose();
                }
                instance = null;
            }
        }
    }
}
