using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using IBatisNet.DataMapper.SessionStore;
using System.Configuration;

namespace WcfStatistics.Dao
{
    public class SqlMapperManager
    {
        #region Configure
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="obj"></param>
        protected static void Configure(object obj)
        {
            _Mapper = null;
        }
        #endregion

        /// <summary>
        /// 数据库映射对象下标
        /// </summary>
        private static volatile ISqlMapper _Mapper;

        /// <summary>
        /// 数据库印射对象下标
        /// </summary>
        public static ISqlMapper Instance
        {
            get
            {
                if (_Mapper == null)
                {
                    lock (typeof(SqlMapper))
                    {
                        if (_Mapper == null) // double-check
                        {
                            _Mapper = CastleContext.Instance.Resolve<ISqlMapper>("wcfstatistics");
                            if (ConfigurationManager.AppSettings["EnableIbatisWebSessionStore"] == "True")
                            {
                                _Mapper.SessionStore = new HybridWebThreadSessionStore(_Mapper.Id);
                            }
                        }
                    }
                }
                return _Mapper;
            }
        }
    }
}
