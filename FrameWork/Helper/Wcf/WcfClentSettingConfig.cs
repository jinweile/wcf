using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    internal class WcfClentSettingConfig
    {
        private IDictionary<string,IDictionary<string,WcfClent>> mlist;
        private IDictionary<string, string> mlist1;
        /// <summary>
        /// 客户端服务读取配值文件
        /// </summary>
        /// <param name="doc"></param>
        public WcfClentSettingConfig(XmlDocument doc)
        {
            mlist = new Dictionary<string, IDictionary<string, WcfClent>>();
            mlist1 = new Dictionary<string, string>();
            IDictionary<string, WcfClent> list2;
            foreach (XmlElement elem in XmlHelper.Children(doc.DocumentElement, "Clients"))
            {
                foreach (XmlElement xe in XmlHelper.Children(elem, "Server"))
                {
                    var name = xe.GetAttribute("name");
                    list2 = new Dictionary<string, WcfClent>();
                    foreach (XmlElement xe1 in XmlHelper.Children(xe, "client"))
                    {
                        if (xe1.NodeType != XmlNodeType.Comment)
                        {
                            string contract = xe1.Attributes["contract"].Value;
                            string address = xe1.Attributes["address"].Value;
                            int? maxItemsInObjectGraph = xe1.Attributes["maxItemsInObjectGraph"] == null ?
                                             null : (int?)int.Parse(xe1.Attributes["maxItemsInObjectGraph"].Value);
                            list2.Add(contract, new WcfClent()
                                        {
                                            Contract = contract,
                                            Address = address,
                                            MaxItemsInObjectGraph = maxItemsInObjectGraph
                                        });
                            mlist1.Add(contract, name);
                        }
                    }
                    //相同建做合并
                    if (mlist.ContainsKey(name))
                    {
                        mlist[name] = mlist[name].Union(list2).ToDictionary(k => k.Key, e => e.Value);
                    }
                    else
                    {
                        mlist.Add(name, list2);
                    }
                }
            }
        }

        /// <summary>
        /// 读取以name为key 其它值为wcfclent为值的列表
        /// </summary>
        public IDictionary<string, IDictionary<string, WcfClent>> ClientsServiceList
        {
            get
            {
                return mlist;
            }
        }

        /// <summary>
        /// 读取以name为contract 其它值为name为值的列表
        /// </summary>
        public IDictionary<string, string> ClientsServiceReverseList
        {
            get
            {
                return mlist1;
            }
        }
    }

    /// <summary>
    /// wcf 客户端有三个属性 Contract Address MaxItemsInObjectGraph
    /// </summary>
    internal class WcfClent
    {
        public string Contract
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
