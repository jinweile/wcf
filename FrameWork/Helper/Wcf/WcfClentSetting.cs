using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    internal class WcfClentSetting : BaseSetting
    {
        #region 默认在config中配置
        private IList<SittConfigSection<string>> WcfClentPath
        {
            get
            {
                return
                    base.SittConfiglist.Where(e => e.Key == "WcfClent")
                                .ToList<SittConfigSection<string>>();
            }
        }
        

        private IList<SittConfigSection<string>> WcfClentConstantPath
        {
            get
            {
                return
                     base.SittConfiglist.Where(e => e.Key == "WcfClentConstantPath")
                                 .ToList<SittConfigSection<string>>();
            }
            
        }

        /// <summary>
        /// 客户端服务配置
        /// </summary>
        internal WcfClentSettingConfig WcfClent
        {
            get
            {
                return GetInstanceConfig.CreateConfigInstance<WcfClentSettingConfig, string>(this.WcfClentPath);
            }
        }

        /// <summary>
        /// 客户端常量
        /// </summary>
        internal WcfClentConstantSettingConfig WcfClentConstant
        {
            get
            {
                return GetInstanceConfig.CreateConfigInstance<WcfClentConstantSettingConfig, string>(this.WcfClentConstantPath);
            }
        }
        #endregion
    }
}
