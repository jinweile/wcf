using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Collections;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    public class InitConfig : IConfigurationSectionHandler
    {
        private static IList<SittConfigSection<XmlAttributeCollection>> mlist = null;
        /// <summary>
        /// 重载IConfigurationSectionHandler读取Section配值
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new InitConfig();

            XmlNodeList list = section.ChildNodes;
            mlist = new List<SittConfigSection<XmlAttributeCollection>>();
            //Section的列表存入list列表中
            foreach (XmlNode node in list)
            {
                if (node.NodeType != XmlNodeType.Comment)
                {
                    mlist.Add(new SittConfigSection<XmlAttributeCollection>()
                    {
                        Key = node.Name,
                        Value = node.Attributes
                    });
                }
            }


            return config;
        }

        /// <summary>
        /// 全部SittConfig下列表的项目
        /// </summary>
        public IList<SittConfigSection<XmlAttributeCollection>> List
        {
            get
            {
                return mlist;
            }
        }
    }

    public class SittConfigSection<T>
    {
        public string Key
        {
            get;
            set;
        }
        public T Value
        {
            get;
            set;
        }
    }
}
