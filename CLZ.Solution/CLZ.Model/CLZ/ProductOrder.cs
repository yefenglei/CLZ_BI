using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLZ.Model.CLZ
{
    public class ProductOrder
    {
        public DateTime time { get; set; }
        public int address_id { get; set; }
        public string address_name { get; set; }
        public int count { get; set; }
        public decimal price { get; set; }
    }
}
