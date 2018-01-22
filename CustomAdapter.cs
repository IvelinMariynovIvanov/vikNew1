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
using Newtonsoft.Json;
using Android.Graphics;
using System.Threading;
using System.Globalization;

namespace ListViewTask
{
    public class CustomAdapter :BaseAdapter<Customer>
    {
        private readonly List<Customer> _customers;
        private Context _mContex;
        private List<Customer> _customerFromNotification = new List<Customer>();


        private List<Customer> countНotifyReadingustomers = new List<Customer>();
        private List<Customer> countНotifyInvoiceOverdueCustomers = new List<Customer>();
        private List<Customer> countNewНotifyNewInvoiceCustomers = new List<Customer>();
        private List<Customer> mCustomerFromApi = new List<Customer>();

       



        public CustomAdapter(Context mContex, List<Customer> mCustomers) 
        {
            

            this._customers = mCustomers;
            this._mContex = mContex;
  
        }

        public override Customer this[int position]
        {
            get
            {
                return _customers[position];
            }
        }

        public override int Count
        {
            get
            {
                return _customers.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Customer customer = _customers[position];  // just for checking the position

            View row = convertView;  /// parametar from method
            ViewHolder holder = null;

            if (row != null)
            {
                holder = row.Tag as ViewHolder;
            }
        
            if (holder == null)
            {
                holder = new ViewHolder();

                // if row is empty create row
                row = InflateControls(position, holder);

                if (holder.ReceiveNotifyInvoiceOverdueToday == true ) 
                {
                    holder.MoneyToP.SetTextColor(Color.Red);
                    holder.EndDate.SetTextColor(Color.Red);
                }
                else if(holder.ReceiveNotifyInvoiceOverdueToday == false)
                {
                    holder.MoneyToP.SetTextColor(Color.ForestGreen);
                }

                if (_customers[position].MoneyToPay == 0)
                {
                    holder.MoneyToP.SetTextColor(Color.Black);
                }
                
                if (holder.ReciveNotifyReadingToday == true) 
                {
                    holder.ReportDate.SetTextColor(Color.Red);
                }
                if(holder.ReceiveNotifyNewInvoiceToday == true)
                {
                    holder.MoneyToP.SetTextColor(Color.ForestGreen);
                }

  
                    holder.Delete.Click += (object sender, EventArgs e) =>
                {
                    DeleteCurrentCustomer(position, holder);
          
                };

                holder.Edit.Click += (object sender, EventArgs e) =>
                {
                    EditCurrentCustomer(position, holder);
                };
                //  row.Tag = holder;   
            }
            return row;
        }


        private View InflateControls(int position, ViewHolder holder)
        {
            View row = LayoutInflater.From(_mContex).Inflate(Resource.Layout.RowView4, null, false);

          //  mAbonati = row.FindViewById<TextView>(Resource.Id.Customers);

            holder.BillNum = row.FindViewById<TextView>(Resource.Id.BillNumber);  
            holder.FullName = row.FindViewById<TextView>(Resource.Id.FullName);
            holder.MoneyToP = row.FindViewById<TextView>(Resource.Id.txtMonetToPayInDecimal);
            holder.OldB = row.FindViewById<TextView>(Resource.Id.tOldBillInDecimal);
            holder.Address = row.FindViewById<TextView>(Resource.Id.Address);
            holder.EndDate = row.FindViewById<TextView>(Resource.Id.endDateValue);
            holder.ReportDate = row.FindViewById<TextView>(Resource.Id.repoprtDate);

            holder.Edit = row.FindViewById<Button>(Resource.Id.EditImg);
            holder.Delete = row.FindViewById<Button>(Resource.Id.DeleteImg);

            holder.Edit.Text = "Настройки";
            holder.Delete.Text = "Изтрий";

            holder.BillNum.Text = _customers[position].Nomer;
            holder.FullName.Text = _customers[position].FullName;
            holder.MoneyToP.Text = Convert.ToDouble(_customers[position].MoneyToPay).ToString("N2")  + " лв";
            holder.OldB.Text = Convert.ToDouble(_customers[position].OldBill).ToString("N2") + " лв";
            holder.Address.Text = _customers[position].Address;

    
            holder.EndDate.Text = _customers[position].EndPayDate.ToShortDateString();
           

            if (_customers[position].StartReportDate == DateTime.MinValue)
            {
                holder.ReportDate.Text = "Не е зададен график."; 
            }
            else
            {
                holder.ReportDate.Text =
                             _customers[position].StartReportDate.ToShortDateString() + " " +
                             _customers[position].StartReportDate.ToShortTimeString() + "-" +
                             _customers[position].EndReportDate.ToShortTimeString();
            }  

            holder.ReceiveNotifyNewInvoiceToday = _customers[position].ReceiveNotifyNewInvoiceToday;
            holder.ReceiveNotifyInvoiceOverdueToday = _customers[position].ReceiveNotifyInvoiceOverdueToday;
            holder.ReciveNotifyReadingToday = _customers[position].ReciveNotifyReadingToday;

            holder.NewCharge = _customers[position].NotifyNewInvoice;
            holder.LateBil = _customers[position].NotifyInvoiceOverdue;
            holder.Report = _customers[position].NotifyReading;

            return row;
        }   

        private void EditCurrentCustomer(int position, ViewHolder holder)
        {
            string rowName = holder.FullName.Text;
            //// 
            //Need this for possitive button event
            //

            //AlertDialog.Builder alertDialog = new AlertDialog.Builder(_mContex);
            //alertDialog.SetTitle("Потвърждавате ли редакцията ? ");
            //alertDialog.SetMessage($"Редактирай клиент                                       {holder.FullName.Text}");
            //alertDialog.SetCancelable(false); // may click outside the dialog

            FragmentTransaction trans = ((Activity)_mContex).FragmentManager.BeginTransaction();

            EditFragment editFragmentDialog =
            new EditFragment(position, _customers.Count, holder.NewCharge, holder.LateBil, holder.Report);
            //new EditFragment(position, _customers.Count, holder.ReceiveNotifyNewInvoiceToday, holder.ReceiveNotifyInvoiceOverdueToday, holder.ReciveNotifyReadingToday);
            editFragmentDialog.Show(trans, "edit fragment");

            editFragmentDialog.OnEditCustomerComplete += (object sender1, OnEditCustomerEventArgs e1) =>
            {
                             
                holder.NewCharge = e1.IsThereANewCharge;
                holder.LateBil = e1.IsThereALateBill;
                holder.Report = e1.IsThereAReport;

                // save to json
                ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

                ISharedPreferencesEditor editor = pref.Edit();
                //editor.Clear();
                editor.Remove("Customers");
                editor.Commit();

                Customer updateCustomer = _customers[position];

                updateCustomer.NotifyNewInvoice = e1.IsThereANewCharge;
                updateCustomer.NotifyInvoiceOverdue = e1.IsThereALateBill;
                updateCustomer.NotifyReading = e1.IsThereAReport;

                #region CheckPossition

                _customers.RemoveAt(position); //new count -1
                _customers.Insert(e1.CurrentPossition, updateCustomer); // put in the same posstion

                NotifyDataSetChanged();

                #endregion

                string edtListOfCustomers = JsonConvert.SerializeObject(_customers);
                editor.PutString("Customers", edtListOfCustomers);
                editor.Commit();

                editor.Commit();
            };

            //// 
            //Need this for possitive button event
            //

            //alertDialog.SetPositiveButton("Да", delegate
            //{
            //    Android.Widget.Toast.MakeText(_mContex, "Редактиране  " + rowName, Android.Widget.ToastLength.Long).Show();

            //});

            //alertDialog.SetNeutralButton("Не", delegate
            //{
            //    alertDialog.Dispose();
            //});

            // alertDialog.Show();
        }

        private void DeleteCurrentCustomer(int position, ViewHolder holder)
        {
            var rowName = holder.FullName.Text;

            AlertDialog.Builder alertDialog = new AlertDialog.Builder(_mContex);
            alertDialog.SetTitle("Потвърждавате ли изтриването ? ");
            alertDialog.SetMessage($"Изтрий клиент                                       {holder.FullName.Text}");
            alertDialog.SetCancelable(false); // may click outside the dialog

            alertDialog.SetPositiveButton("Да", delegate
            {
                ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

                // clear all data in PREFERENCE_NAME
                ISharedPreferencesEditor editor = pref.Edit();
                editor.Remove("Customers");
                editor.Commit();

                Android.Widget.Toast.MakeText(_mContex, "Изтриване  " + rowName, Android.Widget.ToastLength.Long).Show();

                _customers.RemoveAt(position);

                NotifyDataSetChanged();

                // save new list of customers without deleted one
                var listOfCustomersAsJson = JsonConvert.SerializeObject(_customers);

                editor.PutString("Customers", listOfCustomersAsJson);

                editor.Commit();


            });

            alertDialog.SetNeutralButton("Не", delegate
            {
                 alertDialog.Dispose();
            });
            alertDialog.Show();
        }

        private List<Customer> GetCountNewНotifyNewInvoiceCustomers()
        {
            // get shared preferences
            ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var customers = pref.GetString("countNewНotifyNewInvoiceCustomers", null);

            // if preferences return null, initialize listOfCustomers
            if (customers == null)
                return new List<Customer>();

            var listOfCustomers = JsonConvert.DeserializeObject<List<Customer>>(customers);

            if (listOfCustomers == null)
                return new List<Customer>();

            return listOfCustomers;
        }
        private List<Customer> GetCountНotifyInvoiceOverdueCustomers()
        {

            // get shared preferences
            ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var customers = pref.GetString("countНotifyInvoiceOverdueCustomers", null);

            // if preferences return null, initialize listOfCustomers
            if (customers == null)
                return new List<Customer>();

            var listOfCustomers = JsonConvert.DeserializeObject<List<Customer>>(customers);

            if (listOfCustomers == null)
                return new List<Customer>();

            return listOfCustomers;
        }
        private List<Customer> GetCustomersFromApi()
        {
            // get shared preferences
            ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var customers = pref.GetString("customersFromApi", null);

            // if preferences return null, initialize listOfCustomers
            if (customers == null)
                return new List<Customer>();

            var listOfCustomers = JsonConvert.DeserializeObject<List<Customer>>(customers);

            if (listOfCustomers == null)
                return new List<Customer>();

            return listOfCustomers;
        }
        private List<Customer> GetNotifyReadingCustomers()
        {
            // get shared preferences
            ISharedPreferences pref1 = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var customers = pref1.GetString("countНotifyReadingustomers", null);

            // if preferences return null, initialize listOfCustomers
            if (customers == null)
                return new List<Customer>();

            var listOfCustomers = JsonConvert.DeserializeObject<List<Customer>>(customers);

            if (listOfCustomers == null)
                return new List<Customer>();

            return listOfCustomers;
        }
    }
}