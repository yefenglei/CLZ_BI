//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CLZ.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class ZH_DAILY_GOODS_DEAL
    {
        public string PRODUCT_SN { get; set; }
        public string PRODUCT_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal COUNT { get; set; }
        public Nullable<decimal> FEE { get; set; }
        public System.DateTime DEAL_DATE { get; set; }
        public int TRADE_MODE { get; set; }
        public decimal AVG_AMOUNT { get; set; }
        public System.Guid ID { get; set; }
    }
}
