using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EditSubscriptionViewModel
    {
        public int Id { get; set; }
        public string NameOfSubscription { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public byte DurationInMonths { get; set; }
    }
}
