using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Xml.Serialization;

namespace Eltc.Base.FrameWork.Helper
{
    public static class XmlHelper
    {
        /// <summary>
        /// 获取节点的特定子元素
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="name">子元素名</param>
        /// <returns></returns>
        public static XmlElement Child(XmlNode elem, string name)
        {
            foreach (XmlNode node in elem.ChildNodes)
            {
                XmlElement xe = node as XmlElement;
                if (xe != null && xe.LocalName == name) return xe;
            }

            return null;
        }

        /// <summary>
        /// 获取节点的特定子元素集合
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="name">子元素名</param>
        /// <returns></returns>
        public static List<XmlElement> Children(XmlElement elem, string name)
        {
            List<XmlElement> list = new List<XmlElement>();
            foreach (XmlNode node in elem.ChildNodes)
            {
                XmlElement xe = node as XmlElement;
                if (xe != null && xe.LocalName == name) list.Add(xe);
            }

            return list;
        }

        /// <summary>
        /// 获取子元素集合
        /// </summary>
        /// <param name="elem"></param>
      
        /// <returns></returns>
        public static List<XmlElement> Children(XmlElement elem)
        {
            List<XmlElement> list = new List<XmlElement>();
            foreach (XmlNode node in elem.ChildNodes)
            {
                XmlElement xe = node as XmlElement;
                list.Add(xe);
            }

            return list;
        }

        /// <summary>
        /// 序列化对象为XPathNodeIterator
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static XPathNodeIterator Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(ms, obj);
                ms.Position = 0;

                XmlDocument doc = new XmlDocument();
                doc.Load(ms);

                return doc.CreateNavigator().Select("*");
            }
        }

    }
}
