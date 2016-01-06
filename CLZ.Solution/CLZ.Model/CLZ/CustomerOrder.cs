using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLZ.Model.CLZ
{
    public class CustomerOrder
    {
        public string truename { get; set; }
        public string wechat_id { get; set; }
        public int totalOrder { get; set; }
        public int totalCount { get; set; }
        public decimal totalPrice { get; set; }
    }
}
