using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;

namespace Com.Bing.UI
{
    public class RibbonControlEx : RibbonControl
    {
        private RibbonViewInfoEx viewInfo;
        public override RibbonViewInfo ViewInfo
        {
            get
            {
                if (viewInfo == null) viewInfo = new RibbonViewInfoEx(this);
                return viewInfo;
            }
        }
    }
    public class RibbonViewInfoEx : RibbonViewInfo
    {
        public RibbonViewInfoEx(RibbonControl ribbon)
            : base(ribbon)
        {
        }
        public override bool IsAllowDisplayRibbon
        {
            get
            {
                return false;
            }
        }
    }

    
}
