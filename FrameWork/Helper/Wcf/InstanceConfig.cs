using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace Eltc.Base.FrameWork.Helper.Wcf
{

    public interface IInstanceConfig
    {
        /// <summary>
        /// 可以考虑更新为 T CreateConfigInstance<T, V>(V entity) where T : class;
        /// 只为创建单个实列类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        T CreateConfigInstance<T, V>(IList<SittConfigSection<V>> list) where T : class;

    }

    /// <summary>
    /// 读取指定文件,生成配值文件实体类
    /// </summary>
    internal partial class InstanceConfig : IInstanceConfig
    {
        #region IInstanceConfig 成员

        internal XmlDocument LoadFile(string filePath)
        {
            XmlDocument doc = null;
            string configPath = TrasnsForm(filePath);
            if (File.Exists(configPath))
            {
                try
                {
                    doc = new XmlDocument();
                    doc.Load(configPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("配置文件分析出错，请检查文件。", ex);
                }
            }
            return doc;
        }
        private string TrasnsForm(string filePath)
        {
            string configPath = filePath;
            if (filePath.Contains("~"))
                configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath.Substring(2));
            //可以指定本地文件路
            return configPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">配值文件实体类</typeparam>
        /// <typeparam name="V">列表中的类型,可以自定类型,实现具体类时指定,这个指定为string仅是一个路径</typeparam>
        /// <param name="list">从配置节点中读取的全部信息</param>
        /// <returns></returns>
        
        public T CreateConfigInstance<T, V>(IList<SittConfigSection<V>> list)
            where T : class
        {

            XmlDocument doc = new XmlDocument();
            doc.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlElement rootElem = doc.CreateElement("configuration");
            doc.AppendChild(rootElem);
            foreach (var item in list)
            {
                var doc1 = LoadFile(item.Value as string);
                {
                    foreach (var element in XmlHelper.Children(doc1.DocumentElement))
                    {
                        XmlNode node = doc.ImportNode(element, true);
                        doc.DocumentElement.AppendChild(node);
                    }
                }

            }
            return Activator.CreateInstance(typeof(T), doc) as T;
        }
        //---以上这个类更适合放在BaseSetting中
        #endregion
    }

}
