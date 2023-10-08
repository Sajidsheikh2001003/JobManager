using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CreateOrderViewModel
    {
        public string OrderId { get; set; }
        public double Amount { get; set; }
        public string Key { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int SubscriptionId { get; set; }
    }
}
