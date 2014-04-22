using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    public abstract class BaseSetting
    {
        private InitConfig getConfigSitting;

        private InitConfig GetConfigSitting
        {
            get
            {
                if (getConfigSitting == null)
                    //构造默认Section读取配值
                    return DefaultConfigSitting();
                return getConfigSitting;
            }
        }
        /// <summary>
        /// 从config文件读取section列表可以重复
        /// </summary>
        private IList<SittConfigSection<XmlAttributeCollection>> List
        {
            get
            {
                return GetConfigSitting.List;
            }
        }

        /// <summary>
        /// 具体类根据配值不同读取类型不同,这个类中仅用Path一个节点
        /// </summary>
        protected IList<SittConfigSection<string>> SittConfiglist
        {
            get
            {
                var list = new List<SittConfigSection<string>>();
                foreach (var item in List)
                {
                    list.Add(new SittConfigSection<string>()
                    {
                        Key = item.Key,
                        Value = item.Value["Path"].Value
                    });
                }
                return list;
            }
        }

        protected virtual InitConfig DefaultConfigSitting()
        {
            return ConfigurationManager.GetSection("WpwWcfSeting") as InitConfig;
        }

        /// <summary>
        /// 创建配值实体生成类,子类可以重载
        /// </summary>
        protected virtual IInstanceConfig GetInstanceConfig
        {
            get 
            {
                return new InstanceConfig();
            }

        }

        protected virtual InitConfig SetConfigSitting(InitConfig configsitting)
        {
            return getConfigSitting = configsitting;

        }
    }
}
