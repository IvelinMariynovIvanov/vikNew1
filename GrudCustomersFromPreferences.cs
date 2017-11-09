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

namespace ListViewTask
{
   public class GrudCustomersFromPreferences
    {
        public List<Customer> GetCustomersFromPreferences()
        {
            // get shared preferences
            ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var customers = pref.GetString("Customers", null);

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