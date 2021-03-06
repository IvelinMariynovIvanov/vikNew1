﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.App.Job;
using Android.Util;

using JobSchedulerType = Android.App.Job.JobScheduler;
using Export = Java.Interop.ExportAttribute;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Java.Lang;
using Android.Nfc;
using System.Globalization;


namespace ListViewTask
{
    [Service(Exported = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    public class MyJobService : Android.App.Job.JobService
    {
        private List<Customer> mGetCustomersFromDbToNotify;  
        private List<Customer> mCustomers;

        private List<Customer> mTempFetchCollection = new List<Customer>();

        private List<Customer> mCustomerFromApiToNotifyToday =  new List<Customer>();
        private List<Customer> mCountНotifyReadingustomers = new List<Customer>();
        private List<Customer> mCountНotifyInvoiceOverdueCustomers = new List<Customer>();
        private List<Customer> mCountNewНotifyNewInvoiceCustomers = new List<Customer>();
        private List<Customer> mAllUpdateCustomerFromApi = new List<Customer>();

        private string updateHour = string.Empty;
        private string updateDate = string.Empty;

        private bool isAlredyBeenUpdated = false;
        private bool isNeedUpdate = false;
        string updateHourAndDate;
        private bool isThereAnewCustomer = false;

        private int fetchFromPressButton = 0;

        public override bool OnStopJob(JobParameters args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("bg-BG");

            return false;
        }

        public override bool OnStartJob(JobParameters args) 
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("bg-BG");

            mCustomers = GetCustomersFromPreferences();

            Thread thread = new Thread(AllJobsDoneInService);

            thread.Start();

            return false;
        }


        private static bool GetIsThereAneCustomer()
        {
            // get shared preferences
            ISharedPreferences pref1 = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var getIsUpdated = pref1.GetString("isAddedAnewCustomer", string.Empty); //, null);

            // if preferences return null, initialize listOfCustomers
            if (getIsUpdated == string.Empty)
            {

                return false;
            }

            bool lastIsUpdated = Convert.ToBoolean(getIsUpdated);

            if (lastIsUpdated == false) //|| lastIsUpdated == string.Empty)
            {

                return false;
            }

            return lastIsUpdated;
        }

        private static bool GetIsAlreadyBeenUpdated()
        {
            // get shared preferences
            ISharedPreferences pref1 = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var getIsUpdated = pref1.GetString("isAlredyBeenUpdated", string.Empty); //, null);

            // if preferences return null, initialize listOfCustomers
            if (getIsUpdated == string.Empty)
            {

                return false;
            }

            bool lastIsUpdated = Convert.ToBoolean(getIsUpdated);

            if (lastIsUpdated == false) //|| lastIsUpdated == string.Empty)
            {

                return false;
            }

            return lastIsUpdated;
        }

        private static bool GetIsUpdated()
        {
            // get shared preferences
            ISharedPreferences pref1 = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var getIsUpdated = pref1.GetString("isUpdated", string.Empty); //, null);

            // if preferences return null, initialize listOfCustomers
            if (getIsUpdated == string.Empty)
            {

                return false;
            }

            bool lastIsUpdated = Convert.ToBoolean(getIsUpdated) ;

            if (lastIsUpdated == false ) //|| lastIsUpdated == string.Empty)
            {

                return false;
            }

            return lastIsUpdated;
        }

        private void AllJobsDoneInService()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("bg-BG");


            mCountНotifyReadingustomers = new List<Customer>();
            mCountНotifyInvoiceOverdueCustomers = new List<Customer>();
            mCountNewНotifyNewInvoiceCustomers = new List<Customer>();

            mCustomerFromApiToNotifyToday = new List<Customer>();

            // get customers
            mCustomers = GetCustomersFromPreferences();

            ConnectToApi connectToApi = new ConnectToApi();

            bool connection = connectToApi.CheckConnectionOfVikSite();

           
            if (connection == true)
            {
                CheckIfThereisAnewMessageFromApi(connectToApi);

                foreach (var item in mCustomers)
                {
                    mTempFetchCollection.Add(item);
                }

                foreach (var customer in mCustomers)
                {

                    bool isReceiveNotifyNewInvoiceCheck = false;
                    bool isReceiveNotifyInvoiceOverdueCheck = false;
                    bool isReciveNotifyReadingCheck = false;

                    isReceiveNotifyNewInvoiceCheck = customer.NotifyNewInvoice;
                    isReceiveNotifyInvoiceOverdueCheck = customer.NotifyInvoiceOverdue;
                    isReciveNotifyReadingCheck = customer.NotifyReading;


                    EncrypConnection encryp = new EncrypConnection();


                    string crypFinalPass = encryp.Encrypt();



                    string billNumber = customer.Nomer;
                    string egn = customer.EGN;


                    string realUrl = ConnectToApi.urlAPI + "api/abonats/" + crypFinalPass + "/" + billNumber + "/" + egn + "/"
                                   + ConnectToApi.updateByAutoService + "/"
                                   + isReceiveNotifyNewInvoiceCheck + "/" + isReceiveNotifyInvoiceOverdueCheck + "/" + isReciveNotifyReadingCheck + "/";

                    //string realUrl = "http://192.168.2.222/VIKWebApi/" + "api/abonats/"
                    //   + crypFinalPass + "/" + billNumber + "/" + egn + "/" + ConnectToApi.updateByButtonRefresh + "/"
                    //   + isReceiveNotifyNewInvoiceCheck + "/" + isReceiveNotifyInvoiceOverdueCheck + "/" + isReciveNotifyReadingCheck + "/";

                    var jsonResponse = connectToApi.FetchApiDataAsync(realUrl); //FetchApiDataAsync(realUrl);

                    mTempFetchCollection.Remove(customer);


                    //check the api
                    if (jsonResponse == null)
                    {
                        mAllUpdateCustomerFromApi.Add(customer);

                    }
                    // check in vikSite is there a customer with this billNumber (is billNumber correct)
                    else if (jsonResponse == "[]")
                    {
                        mAllUpdateCustomerFromApi.Add(customer);

                    }

                    // check if billNumber is correct and get and save customer in phone
                    else if (jsonResponse != null)
                    {
                        Customer updateCutomerButNoNotify = connectToApi.GetCustomerFromApi(jsonResponse);

                        if (updateCutomerButNoNotify != null && updateCutomerButNoNotify.IsExisting == true)
                        {

                            updateCutomerButNoNotify.NotifyNewInvoice = customer.NotifyNewInvoice;
                            updateCutomerButNoNotify.NotifyInvoiceOverdue = customer.NotifyInvoiceOverdue;
                            updateCutomerButNoNotify.NotifyReading = customer.NotifyReading;

                            mAllUpdateCustomerFromApi.Add(updateCutomerButNoNotify);
                        }

                        else
                        {
                            mAllUpdateCustomerFromApi.Add(customer);
                           
                        }
                    }

                }

                SelectWhichCustomersTobeNotified(mCountНotifyReadingustomers, mCountНotifyInvoiceOverdueCustomers, mCountNewНotifyNewInvoiceCustomers, mAllUpdateCustomerFromApi); // mCustomerFromApiToNotifyToday

                SaveCustomersFromApiInPhone();

                SentNotificationForOverdue(mCountНotifyInvoiceOverdueCustomers);

                SentNoficationForNewInovoice(mCountNewНotifyNewInvoiceCustomers);

                SentNotificationForReading(mCountНotifyReadingustomers);

            
            }
        }

        private void CheckIfThereisAnewMessageFromApi(ConnectToApi connectToApi)
        {
            EncrypConnection encryp = new EncrypConnection();

          
            string crypFinalPass = encryp.Encrypt();

            //// get message from preferences
            GrudMessageFromPreferemces grudMessage = new GrudMessageFromPreferemces();

            int lastMessageId = grudMessage.GetMessageFromPreferencesInPhone().MessageID;

            //real 
            string messageUrl = ConnectToApi.urlAPI + "api/msg/";

            ///test
            //string messageUrl = ConnectToApi.wtf + "api/msg/";

            string finalUrl = messageUrl + crypFinalPass + "/" + lastMessageId;

            var messageFromApiAsJsonString = connectToApi.FetchApiDataAsync(finalUrl);

            // check api response
            if (messageFromApiAsJsonString != null)
            {
                Message newMessage = new Message();

                newMessage = connectToApi.GetMessageFromApi(messageFromApiAsJsonString);

                if (newMessage.MessageID > lastMessageId)
                {
                    grudMessage.SaveMessageInPhone(newMessage);

                    int messagesCount = newMessage.Messages.Count;

                    if (messagesCount > 0)
                    {
                        SentNotificationWithoutSubscribe(newMessage);
                    }
                }
            }
        }



        private void SentNotificationWithoutSubscribe(Message newMessage)
        {
            // string countНotifyInvoiceOverdueCustomersAsString = JsonConvert.SerializeObject(countНotifyInvoiceOverdueCustomers);

            // Set up an intent so that tapping the notifications returns to this app:
            Intent intent = new Intent(this, typeof(MainActivity));

            // Create a PendingIntent; 
            const int pendingIntentId = 3;
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.CancelCurrent);

            // Instantiate the Inbox style:
            Notification.InboxStyle inboxStyle = new Notification.InboxStyle();

            //  Instantiate the builder and set notification elements:
            Notification.Builder bulideer = new Notification.Builder(this)
                .SetContentIntent(pendingIntent)
                .SetSmallIcon(Resource.Drawable.vik);

            // Set the title and text of the notification:
            bulideer.SetContentTitle("Съобщение от ВиК Русе");

            foreach (var item in newMessage.Messages)
            {
                //  Generate a message summary for the body of the notification:
                inboxStyle.AddLine($"{item.ToString()}");
                bulideer.SetContentText($"{item.ToString()}");
            }
            // Plug this style into the builder:
            bulideer.SetStyle(inboxStyle);

            // Build the notification:
            Notification notification11 = bulideer.Build();

            // Get the notification manager:
            NotificationManager notificationManager1 =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationIdd = 3;
            notificationManager1.Notify(notificationIdd, notification11);
        }

    

        private void MakeToastWhenNoInternetAccses()
        {
            
            string StatusString = "Проверете интернет връзката";
            Toast.MakeText(this, $"{StatusString}", ToastLength.Long).Show();
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



        public static void SelectWhichCustomersTobeNotified(List<Customer> countНotifyReadingustomers, List<Customer> countНotifyInvoiceOverdueCustomers, List<Customer> countNewНotifyNewInvoiceCustomers, List<Customer> mAllUpdateCustomerFromApi) // mCustomerFromApiToTotifyToday
        {
            foreach (var customer in mAllUpdateCustomerFromApi) // mCustomerFromApiToTotifyToday
            {   
                ///
                /// Need this to refresh notifications
                /// 

                //customer.ReceiveNotifyInvoiceOverdueToday = true;
                //customer.ReceiveNotifyNewInvoiceToday = true;
                //customer.ReciveNotifyReadingToday = true;

                bool isAnyNotifycationCheck =
                    (customer.NotifyInvoiceOverdue == true ||
                    customer.NotifyNewInvoice == true ||
                    customer.NotifyReading == true);

                if (isAnyNotifycationCheck == true)
                {

                    if (customer.NotifyNewInvoice == true && customer.ReceiveNotifyNewInvoiceToday) 
                    {

                       countNewНotifyNewInvoiceCustomers.Add(customer);
                    }
                    if (customer.NotifyInvoiceOverdue == true && customer.ReceiveNotifyInvoiceOverdueToday)       
                    {
                        countНotifyInvoiceOverdueCustomers.Add(customer);
                    }
                    if (customer.NotifyReading == true && customer.ReciveNotifyReadingToday)           
                    {
                        countНotifyReadingustomers.Add(customer);
                    }
                }
            }
        }

        public void SaveCustomersFromApiInPhone()
        {
            DateTime updateHourAndDate = DateTime.Now;

            string hourFormat = "HH:mm";
            string shortReportDatetHour = updateHourAndDate.ToShortTimeString();

            string dateFormat = "dd.MM.yyyy";

            updateHour = updateHourAndDate.ToString(hourFormat);
            updateDate = updateHourAndDate.ToString(dateFormat);

           

            ISharedPreferences pref =
               Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // convert the list to json
            var listOfCustomersAsJson = JsonConvert.SerializeObject(mAllUpdateCustomerFromApi); // mCustomers

            ISharedPreferencesEditor editor = pref.Edit();

            // set the value to Customers key
            editor.PutString("Customers", listOfCustomersAsJson);

            editor.PutString("Hour", updateHour);
            editor.PutString("Date", updateDate);

            // commit the changes
            editor.Commit();



            ///////////////////////////
            //if (mCustomers.Count == 0)
            //{
            //    //updateDate = string.Empty;
            //    //updateHour = string.Empty;

            //    var intent = new Intent(this, typeof(MainActivity));

            //    StartActivity(intent);
            //}
        }

  
        public void SentNotificationForOverdue(List<Customer> countНotifyInvoiceOverdueCustomers)
        {
            if (countНotifyInvoiceOverdueCustomers.Count > 0)
            {
   
                // Set up an intent so that tapping the notifications returns to this app:
                Intent intent = new Intent(this, typeof(MainActivity));

                // Create a PendingIntent; 
                const int pendingIntentId = 0;
                PendingIntent pendingIntent =
                    PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.CancelCurrent);

                // Instantiate the Inbox style:
                Notification.InboxStyle inboxStyle = new Notification.InboxStyle();

                //  Instantiate the builder and set notification elements:
                Notification.Builder bulideer = new Notification.Builder(this);

                bulideer.SetContentIntent(pendingIntent);
                bulideer.SetSmallIcon(Resource.Drawable.vik);

                // Set the title and text of the notification:
                bulideer.SetContentTitle("Просрочване");

                foreach (var cus in countНotifyInvoiceOverdueCustomers)
                {

                        string format = "dd.MM.yyyy";
                        string date = cus.EndPayDate.ToString(format);

                        inboxStyle.AddLine($"Аб. номер: {cus.Nomer.ToString()}, {date}");

                        bulideer.SetContentText($"Аб. номер: {cus.Nomer.ToString()}, {date}");
                      
                }

                // Plug this style into the builder:
                bulideer.SetStyle(inboxStyle);

                // Build the notification:
                Notification notification11 = bulideer.Build();

                // Get the notification manager:
                NotificationManager notificationManager1 =
                    GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationIdd = 0;
                notificationManager1.Notify(notificationIdd, notification11);


            }
        }

        public void SentNoficationForNewInovoice(List<Customer> countNewНotifyNewInvoiceCustomers)
        {
            if (countNewНotifyNewInvoiceCustomers.Count > 0)
            {
                // Set up an intent so that tapping the notifications returns to this app:
                Intent intent = new Intent(this, typeof(MainActivity));

                // Create a PendingIntent; 
                const int pendingIntentId = 1;
                PendingIntent pendingIntent =
                    PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.CancelCurrent);


                // Instantiate the Inbox style:
                Notification.InboxStyle inboxStyle = new Notification.InboxStyle();

                //  Instantiate the builder and set notification elements:
                Notification.Builder bulideer = new Notification.Builder(this);

                bulideer.SetContentIntent(pendingIntent);
                bulideer.SetSmallIcon(Resource.Drawable.vik);

                // Set the title and text of the notification:
                bulideer.SetContentTitle("Нова фактура");
                //  bulideer.SetContentText("chimchim@xamarin.com");

                foreach (var item in countNewНotifyNewInvoiceCustomers)
                {                   
                        string money = item.MoneyToPay.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("bg-BG"));

                        inboxStyle.AddLine($"Аб. номер: {item.Nomer.ToString()}, {money}");

                        bulideer.SetContentText($"Аб. номер: {item.Nomer.ToString()}, {money}");
                
                }
                // Plug this style into the builder:
                bulideer.SetStyle(inboxStyle);

                // Build the notification:
                Notification notification11 = bulideer.Build();

                // Get the notification manager:
                NotificationManager notificationManager1 =
                    GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationIdd = 1;
                notificationManager1.Notify(notificationIdd, notification11);
            }
        }

        public void SentNotificationForReading(List<Customer> countНotifyReadingustomers)
        {
            if (countНotifyReadingustomers.Count > 0)
            {
                // Set up an intent so that tapping the notifications returns to this app:
                Intent intent = new Intent(this, typeof(MainActivity));

                // Create a PendingIntent; 
                const int pendingIntentId = 2;
                PendingIntent pendingIntent =
                    PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.CancelCurrent);


                // Instantiate the Inbox style:
                Notification.InboxStyle inboxStyle = new Notification.InboxStyle();

                //  Instantiate the builder and set notification elements:
                Notification.Builder bulideer = new Notification.Builder(this);

                bulideer.SetSmallIcon(Resource.Drawable.vik);
                bulideer.SetContentIntent(pendingIntent);

                // Set the title and text of the notification:
                bulideer.SetContentTitle("Ден на отчитане");

                foreach (var cus in countНotifyReadingustomers)
                {
                
                        // Generate a message summary for the body of the notification:
                        string format = "dd.MM.yyyy";
                        string date = cus.StartReportDate.ToString(format);

                        inboxStyle.AddLine($"Аб. номер: {cus.Nomer.ToString()}, {date}");

                        bulideer.SetContentText($"Аб. номер: {cus.Nomer.ToString()}, {date}");

                }

                // Plug this style into the builder:
                bulideer.SetStyle(inboxStyle);

                // Build the notification:
                Notification notification11 = bulideer.Build();

                // Get the notification manager:
                NotificationManager notificationManager1 =
                    GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationIdd = 2;
                notificationManager1.Notify(notificationIdd, notification11);

            }
        }

       

        private List<Customer> GetCustomersFromPreferences()
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