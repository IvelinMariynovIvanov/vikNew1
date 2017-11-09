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

namespace ListViewTask
{
   public class Customer
    {


        public string EGN { get; set; } // not showing
        public string Nomer { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }

        public bool NotifyNewInvoice { get; set; }
        public bool NotifyInvoiceOverdue { get; set; }
        public bool NotifyReading { get; set; }

        public bool ReceiveNotifyNewInvoiceToday { get; set; }
        public bool ReceiveNotifyInvoiceOverdueToday { get; set; }
        public bool ReciveNotifyReadingToday { get; set; }

        public bool DidGetNewInoviceToday { get; set; }
        public bool DidGetOverdueToday { get; set; }
        public bool DidGetReadingToday { get; set; }
        public bool DidGetAnyNotificationToday { get; set; }

        public double MoneyToPay { get; set; }
        public double OldBill { get; set; }

        public DateTime EndPayDate { get; set; }
        public DateTime StartReportDate { get; set; }
        public DateTime EndReportDate { get; set; }

        public bool IsExisting { get; set; }


        public Customer()
        {
        }

        public Customer(string eGN, string billNumber)
        {
            EGN = eGN;
            Nomer = billNumber;
        }

        public Customer(string eGN, string billNumber, string fullName, double moneyToPay, double oldBill, bool newCharge, bool lateBill, bool report, DateTime endReportDate, DateTime startReportDate, string address, DateTime endDate)
        {
            EGN = eGN;
            Nomer = billNumber;
            FullName = fullName;
            MoneyToPay = moneyToPay;
            OldBill = oldBill;
            NotifyNewInvoice = newCharge;
            NotifyInvoiceOverdue = lateBill;
            NotifyReading = report;
            EndReportDate = endReportDate;
            StartReportDate = startReportDate;
            Address = address;
            EndPayDate = endDate;
        }

    }
}