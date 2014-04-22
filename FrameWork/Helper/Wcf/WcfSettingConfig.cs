using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    internal class WcfSettingConfig
    {
        /// <summary>
        /// wcf服务配值类
        /// </summary>
        private IList<ServicePoint> mList;
        public WcfSettingConfig(XmlDocument doc)
        {
            mList = new List<ServicePoint>(1000);
            foreach (XmlElement elem in XmlHelper.Children(doc.DocumentElement, "Services"))
            {
                foreach (XmlElement xe in XmlHelper.Children(elem, "service"))
                {
                    if (xe.NodeType != XmlNodeType.Comment)
                    {
                        mList.Add(new ServicePoint()
                             {
                                 Name = Type.GetType(xe.GetAttribute("name")),
                                 Contract = Type.GetType(xe.GetAttribute("contract")),
                                 Address = xe.GetAttribute("address"),
                                 MaxItemsInObjectGraph = xe.Attributes["maxItemsInObjectGraph"] == null ? null : (int?)int.Parse(xe.GetAttribute("maxItemsInObjectGraph"))
                             });
                    }
                }
            }
        }

        public IList<ServicePoint> List
        {
            get
            {
                return mList;
            }
        }
    }

    /// <summary>
    /// 服务包括三个属性Name Contract Address
    /// </summary>
    internal class ServicePoint
    {
        public Type Name
        {
            get;
            set;
        }

        public Type Contract
        {
            get;
            set;
        }
        public string Address
        {
            get;
            set;
        }
        public int? MaxItemsInObjectGraph
        {
            get;
            set;
        }
    }

}
