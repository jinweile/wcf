using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

namespace Eltc.Base.FrameWork.Helper.Wcf.LoadBalance
{
    /// <summary>
    /// 一致性哈希
    /// </summary>
    internal class ConsistentHashing : BalanceAlgorithmBase
    {
        /// <summary>
        /// 一致性哈希圆环列表
        /// </summary>
        private SortedList<long, string> ketamaNodes = null;
        /// <summary>
        /// 虚拟节点数量(默认虚拟160个)
        /// </summary>
        private int numReps = 160;
        /// <summary>
        /// 获取服务器键超时时间
        /// </summary>
        private TimeSpan getkeyTimeout = new TimeSpan(0, 0, 10);

        AutoResetEvent autoEvents = null;
        /// <summary>
        /// 是否正在初始化
        /// </summary>
        bool isInit = false;

        /// <summary>
        /// 初始化算法圆环(第一次或服务器列表发生改变时使用)
        /// </summary>
        /// <param name="nodes">负载节点列表</param>
        protected override void Init(ICollection<string> _urilist)
        {
            isInit = true;
            autoEvents = new AutoResetEvent(false);
            loadRound(_urilist);
            autoEvents.Set();
            isInit = false;
        }

        /// <summary>
        /// 产生圆环
        /// </summary>
        /// <param name="server_keys"></param>
        private void loadRound(ICollection<string> server_keys)
        {
            ketamaNodes = new SortedList<long, string>();

            //对所有节点，生成nCopies个虚拟结点
            foreach (string node in server_keys)
            {
                //每四个虚拟结点为一组
                for (int i = 0; i < numReps / 4; i++)
                {
                    //getKeyForNode方法为这组虚拟结点得到惟一名称 
                    byte[] digest = HashAlgorithm.computeMd5(node + i);
                    //Md5是一个16字节长度的数组，将16字节的数组每四个字节一组，分别对应一个虚拟结点
                    //这就是为什么上面把虚拟结点四个划分一组的原因
                    for (int h = 0; h < 4; h++)
                    {
                        long m = HashAlgorithm.hash(digest, h);
                        ketamaNodes[m] = node;
                    }
                }
            }
        }

        /// <summary>
        /// 获取服务器key
        /// </summary>
        /// <returns>key</returns>
        internal override string GetServerKey()
        {
            //5秒内没获取到Init释放，则认为超时
            if (autoEvents != null && isInit && !autoEvents.WaitOne(TimeSpan.Parse("00:00:05")))
            {
                throw new Exception("获取负载的服务器超时");
            }

            if (ketamaNodes.Count == 0)
            {
                throw new Exception("警告:所有负载的服务器都挂了");
            }

            Guid newid = Guid.NewGuid();
            byte[] digest = HashAlgorithm.computeMd5(newid.ToString());
            string rv = GetNodeForKey(HashAlgorithm.hash(digest, 0));
            return rv;
        }

        string GetNodeForKey(long hash)
        {
            string rv;
            long key = hash;
            //如果找到这个节点，直接取节点，返回   
            if (!ketamaNodes.ContainsKey(key))
            {
                //得到大于当前key的那个子Map，然后从中取出第一个key，就是大于且离它最近的那个key
                //说明详见: http://www.javaeye.com/topic/684087
                var tailMap = from coll in ketamaNodes
                              where coll.Key > hash
                              select new { coll.Key };
                if (tailMap == null || tailMap.Count() == 0)
                    key = ketamaNodes.FirstOrDefault().Key;
                else
                    key = tailMap.FirstOrDefault().Key;
            }
            rv = ketamaNodes[key];
            return rv;
        }
    }

    /// <summary>
    /// 计算散列MD5
    /// </summary>
    internal class HashAlgorithm
    {
        /// <summary>
        /// hash
        /// </summary>
        /// <param name="digest"></param>
        /// <param name="nTime"></param>
        /// <returns></returns>
        public static long hash(byte[] digest, int nTime)
        {
            long rv = ((long)(digest[3 + nTime * 4] & 0xFF) << 24)
                                    | ((long)(digest[2 + nTime * 4] & 0xFF) << 16)
                                    | ((long)(digest[1 + nTime * 4] & 0xFF) << 8)
                                    | ((long)digest[0 + nTime * 4] & 0xFF);

            /* Truncate to 32-bits */
            return rv & 0xffffffffL;
        }

        /// <summary>
        /// Get the md5 of the given key.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static byte[] computeMd5(string k)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] keyBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(k));
            md5.Clear();
            return keyBytes;
        }
    }
}
