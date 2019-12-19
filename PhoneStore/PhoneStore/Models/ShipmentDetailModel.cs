using Syncfusion.XForms.ProgressBar;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneStore.Models
{
    public class ShipmentDetailModel
    {
        public string Title { get; set; }
        //public string Description { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public StepStatus Status { get; set; }
        public int ProgressValue { get; set; }
    }
}
