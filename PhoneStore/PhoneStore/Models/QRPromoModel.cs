using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneStore.Models
{
    public class QRPromoModel
    {
        public string UserEmail { get; set; }
        public string QREmail { get; set; }
        public decimal Discount { get; set; }
        public bool IsUsed { get; set; }
    }
}
