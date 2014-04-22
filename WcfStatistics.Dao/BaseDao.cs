using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IBatisNet.DataMapper;

namespace WcfStatistics.Dao
{
    public class BaseDao
    {
        protected virtual ISqlMapper Instance
        {
            get
            {
                return SqlMapperManager.Instance;
            }
        }
    }
}
