using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace ListViewTask
{
    public  class ViewHolder : Java.Lang.Object
    {
     
        public Button Edit { get; set; }
        public Button Delete { get; set; }

        public TextView FullName { get; set ; }
        public TextView BillNum { get; set; }
        public TextView OldB { get; set; }
        public TextView MoneyToP { get ; set ; }

        public TextView ReportHour { get; set; }
        public TextView ReportDate { get; set; }
        public TextView EndDate { get; set; }
        public TextView Address { get; set; }

        public bool ReceiveNotifyNewInvoiceToday { get; set; }
        public bool ReceiveNotifyInvoiceOverdueToday { get; set; }
        public bool ReciveNotifyReadingToday { get; set; }

        public bool NewCharge { get; set; }
        public bool LateBil { get; set; }
        public bool Report { get; set; }

        public Color color { get; set; }

    }
}