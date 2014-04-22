using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    internal class WcfServiceSetting : BaseSetting
    {
        private IList<SittConfigSection<string>> WcfSererPath
        {
            get
            {
                return 
                    base.SittConfiglist.Where(e => e.Key == "WcfServer")
                                .ToList<SittConfigSection<string>>();
            }
        }
        
        private IList<SittConfigSection<string>> WcfSettingList
        {
            get
            {
                return
                    base.SittConfiglist.Where(e => e.Key == "WcfConstant")
                                .ToList<SittConfigSection<string>>();
            }
        }

        /// <summary>
        /// 服务读取配值
        /// </summary>
        internal WcfSettingConfig WcfSetting
        {
            get
            {
                return GetInstanceConfig.CreateConfigInstance<WcfSettingConfig, string>(this.WcfSererPath);
            }
        }

        /// <summary>
        /// 常量读取配值
        /// </summary>
        internal WcfConstantSettingConfig ConstantSetting
        {
            get
            {
                return GetInstanceConfig.CreateConfigInstance<WcfConstantSettingConfig, string>(this.WcfSettingList);
            }
        }
    }
}
