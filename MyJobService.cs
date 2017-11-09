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
        private List<Customer> mGetCustomersFromDbToNotify;  //only with 5 properties which are needed for sending to api
        private List<Customer> mCustomers;

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

        public override bool OnStartJob(JobParameters args) // JobParameters @params
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("bg-BG");

            mCustomers = GetCustomersFromPreferences();

            Thread thread = new Thread(AllJobsDoneInService);

            thread.Start();

            //isNeedUpdate = GetIsUpdated();

            //isAlredyBeenUpdated = GetIsAlreadyBeenUpdated();

            //isThereAnewCustomer = GetIsThereAneCustomer();

            //string format = "HH";
            //string currentTime = DateTime.Now.ToString(format);
            //int currentTimeAsInt = Convert.ToInt32(currentTime);

            //string dateFormat = "ddMMyyyy";
            //string currentDate = DateTime.Now.ToString(dateFormat);
            //int currentDateAsInt = Convert.ToInt32(currentDate);

            //ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);
            //string lastUpdateDate = pref.GetString("Date", null);

            ////if(lastUpdateDate == null)
            ////{
            ////    lastUpdateDate = DateTime.Now.ToShortDateString();
            ////}

            //var lastUpdateAsDate = Convert.ToDateTime(lastUpdateDate);
            //string formatLastUpdate = lastUpdateAsDate.ToString(dateFormat);
            //int lastUpdateAsInt = Convert.ToInt32(formatLastUpdate);

            //if(currentTimeAsInt <= 10)  /// 9                   //currentDateAsInt
            //{
            //    isAlredyBeenUpdated = false;

            //    ISharedPreferencesEditor editor = pref.Edit();

            //    // convert the list to json
            //    // var isUpdatedAsString = JsonConvert.SerializeObject(isNeedUpdate);

            //    var isUpdatedAsString = JsonConvert.SerializeObject(isAlredyBeenUpdated);

            //    // set the value to Customers key

            //    editor.PutString("isAlredyBeenUpdated", isUpdatedAsString);

            //    // commit the changes
            //    editor.Commit();
            //}

            //if (currentTimeAsInt >= 10)     //10          // && isUpdated == false) // && currentTimeAsInt >= 23)   // do not receive at morning
            //{
            //    isNeedUpdate = false;

            //    ISharedPreferencesEditor editor = pref.Edit();

            //    // convert the list to json
            //    var isUpdatedAsString = JsonConvert.SerializeObject(isNeedUpdate);

            //    // set the value to Customers key

            //    editor.PutString("isUpdated", isUpdatedAsString);

            //    // commit the changes
            //    editor.Commit();
            //}

            //if(isThereAnewCustomer == true)
            //{
            //    isNeedUpdate = false;
            //    isAlredyBeenUpdated = false;

            //}


            ////if (isTHereANewCustomerToNotifyToday == true)
            ////{
            ////    isUpdated = false;
            ////}

            //bool doesStartAService = //(currentTimeAsInt >= 16 && currentDateAsInt >= lastUpdateAsInt);
            //    (isNeedUpdate == false && currentTimeAsInt >= 10 && currentDateAsInt >= lastUpdateAsInt && isAlredyBeenUpdated == false); //  &&  && isThereAnewCustomer == false) ;//isTHereANewCustomerToNotifyToday == true);

            //   if (doesStartAService == true) ///currentDateAsInt == )
            //{
            //    Thread thread = new Thread(AllJobsDoneInService);

            //    thread.Start();

            //    /// all true
            //    //   isNeedUpdate = true;
            //    isNeedUpdate = false;
            //    isThereAnewCustomer = false;

            //    isAlredyBeenUpdated = true;

            //    foreach (var cust in mCustomers)
            //    {
            //        cust.DidGetAnyNotificationToday = true;
            //    }


            //    // save
            //    ISharedPreferencesEditor editor = pref.Edit();

            //    // convert the list to json
            //    // var isUpdatedAsString = JsonConvert.SerializeObject(isNeedUpdate);
            //    var isUpdatedAsString = JsonConvert.SerializeObject(isAlredyBeenUpdated);
            //    string isThereAneCustomerAsString = JsonConvert.SerializeObject(isThereAnewCustomer);
            //    // set the value to Customers key


            //    editor.PutString("isUpdated", isUpdatedAsString);
            //    editor.PutString("isAddedAnewCustomer", isThereAneCustomerAsString);
            //    editor.PutString("isAlredyBeenUpdated", isNeedUpdate.ToString());
            //    // commit the changes
            //    editor.Commit();

            //   return false;
            //}

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

            // if (mCustomers.Count > 0)
            //  {
            if (connection == true)
            {
                CheckIfThereisAnewMessageFromApi(connectToApi);

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


                    // check if connection is ok
                    //  if (isAnyNotifycationCheck == true)
                    //   {
                    string billNumber = customer.Nomer;
                    string egn = customer.EGN;

                    //CREATE URL
                    // string url = "http://192.168.2.222/VIKWebApi/";


                    /// !!!!!!!!!!!!!!!!!!!!! here
                    //string realUrl = ConnectToApi.urlAPI + "api/abonats/" + crypFinalPass + "/" + billNumber + "/" + egn;

                    //string realUrl = ConnectToApi.urlAPI + "api/abonats/" + crypFinalPass + "/" + billNumber + "/" + egn + "/"
                    //               + ConnectToApi.updateByAutoService + "/"
                    //               + isReceiveNotifyNewInvoiceCheck + "/" + isReceiveNotifyInvoiceOverdueCheck + "/" + isReciveNotifyReadingCheck + "/"; 

                    string realUrl = "http://192.168.2.222/VIKWebApi/" + "api/abonats/"
                       + crypFinalPass + "/" + billNumber + "/" + egn + "/" + ConnectToApi.updateByButtonRefresh + "/"
                       + isReceiveNotifyNewInvoiceCheck + "/" + isReceiveNotifyInvoiceOverdueCheck + "/" + isReciveNotifyReadingCheck + "/";

                    var jsonResponse = connectToApi.FetchApiDataAsync(realUrl); //FetchApiDataAsync(realUrl);

                    //check the api
                    if (jsonResponse == null)
                    {
                        return;
                    }
                    // check in vikSite is there a customer with this billNumber (is billNumber correct)
                    else if (jsonResponse == "[]")
                    {
                        return;  ////

                    }

                    // check if billNumber is correct and get and save customer in phone
                    else if (jsonResponse != null)
                    {
                        Customer updateCutomerButNoNotify = connectToApi.GetCustomerFromApi(jsonResponse);

                        if (updateCutomerButNoNotify != null && updateCutomerButNoNotify.IsExisting == true)
                        //  (newCustomer != null && newCustomer.IsExisting == true)
                        {

                            updateCutomerButNoNotify.NotifyNewInvoice = customer.NotifyNewInvoice;
                            updateCutomerButNoNotify.NotifyInvoiceOverdue = customer.NotifyInvoiceOverdue;
                            updateCutomerButNoNotify.NotifyReading = customer.NotifyReading;

                            mAllUpdateCustomerFromApi.Add(updateCutomerButNoNotify);
                        }

                        else
                        {
                            return;
                        }
                    }

                    //string format = "HH";
                    //string currentTime = DateTime.Now.ToString(format);
                    //int currentTimeAsInt = Convert.ToInt32(currentTime);

                    //   if(currentTimeAsInt == 17)
                    //  {
                    SelectWhichCustomersTobeNotified(mCountНotifyReadingustomers, mCountНotifyInvoiceOverdueCustomers, mCountNewНotifyNewInvoiceCustomers, mAllUpdateCustomerFromApi); // mCustomerFromApiToNotifyToday

                    SaveCustomersFromApiInPhone();



                    //Looper.Prepare();

                    //MyNotification myNotification = new MyNotification(this);

                    //myNotification.SentNotificationForOverdue(mCountНotifyInvoiceOverdueCustomers);

                    //if()
                    SentNotificationForOverdue(mCountНotifyInvoiceOverdueCustomers);

                    SentNoficationForNewInovoice(mCountNewНotifyNewInvoiceCustomers);

                    SentNotificationForReading(mCountНotifyReadingustomers);

                    // SaveCustomersFromApiInPhone();
                    //   }


                    //else
                    //{
                    //    SelectWhichCustomersTobeNotified(mCountНotifyReadingustomers, mCountНotifyInvoiceOverdueCustomers, mCountNewНotifyNewInvoiceCustomers, mCustomerFromApiToNotifyToday);

                    //  //  SaveCustomersFromApiInPhone();

                    //    foreach (var customer in mCustomerFromApiToNotifyToday)
                    //    {
                    //        if(customer.NotifyNewInvoice == true && customer.ReceiveNotifyNewInvoiceToday && customer.DidGetNewInoviceToday == false)
                    //        {
                    //            SentNoficationForNewInovoice(mCountNewНotifyNewInvoiceCustomers);
                    //        }
                    //        else if(customer.NotifyInvoiceOverdue == true && customer.ReceiveNotifyInvoiceOverdueToday == true && customer.DidGetOverdueToday == false)
                    //        {
                    //            SentNotificationForOverdue(mCountНotifyInvoiceOverdueCustomers);
                    //        }
                    //        else if(customer.NotifyReading == true && customer.ReciveNotifyReadingToday == true && customer.DidGetReadingToday == false)
                    //        {
                    //            SentNotificationForReading(mCountНotifyReadingustomers);
                    //        }
                    //    }

                    //   // SaveCustomersFromApiInPhone();
                    //}

                }
                //else
                //{
                //    return;  ////
                //}
                //  }
            }
        }

        private void CheckIfThereisAnewMessageFromApi(ConnectToApi connectToApi)
        {
            EncrypConnection encryp = new EncrypConnection();

          
            string crypFinalPass = encryp.Encrypt();

            //// get message from preferences
            GrudMessageFromPreferemces grudMessage = new GrudMessageFromPreferemces();

            int lastMessageId = grudMessage.GetMessageFromPreferencesInPhone().MessageID;

            //realno !!!!!!!!!!!!!
            string messageUrl = ConnectToApi.urlAPI + "api/msg/";

            ///testovo
        //    string messageUrl = ConnectToApi.wtf + "api/msg/";

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

                    //   SaveMessageInPhone(newMessage);

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
                PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

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
                //customer.ReceiveNotifyInvoiceOverdueToday = true;
                //customer.ReceiveNotifyNewInvoiceToday = true;
                //customer.ReciveNotifyReadingToday = true;

                bool isAnyNotifycationCheck =
                    (customer.NotifyInvoiceOverdue == true ||
                    customer.NotifyNewInvoice == true ||
                    customer.NotifyReading == true);

                if (isAnyNotifycationCheck == true)
                {

                    if (customer.NotifyNewInvoice == true && customer.ReceiveNotifyNewInvoiceToday) // && customer.DidGetNewInoviceToday == false)              /// bez poslednata proverka
                    {

                       countNewНotifyNewInvoiceCustomers.Add(customer);
                    }
                    if (customer.NotifyInvoiceOverdue == true && customer.ReceiveNotifyInvoiceOverdueToday)        /// bez poslednata proverka
                    {
                        countНotifyInvoiceOverdueCustomers.Add(customer);
                    }
                    if (customer.NotifyReading == true && customer.ReciveNotifyReadingToday)           /// bez poslednata proverka
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
                //string countНotifyInvoiceOverdueCustomersAsString = JsonConvert.SerializeObject(countНotifyInvoiceOverdueCustomers);

                // Set up an intent so that tapping the notifications returns to this app:
                Intent intent = new Intent(this, typeof(MainActivity));

                // Create a PendingIntent; 
                const int pendingIntentId = 0;
                PendingIntent pendingIntent =
                    PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

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
                    // Generate a message summary for the body of the notification:

                  //  if(cus.DidGetOverdueToday == false)
                  //  {
                        string format = "dd.MM.yyyy";
                        string date = cus.EndPayDate.ToString(format);

                        inboxStyle.AddLine($"Аб. номер: {cus.Nomer.ToString()}, {date}");

                        bulideer.SetContentText($"Аб. номер: {cus.Nomer.ToString()}, {date}");

                        //cus.DidGetOverdueToday = true;

                        //Customer notifiedCustomer = mAllUpdateCustomerFromApi.FirstOrDefault(c => c.EGN == cus.EGN);
                        //mAllUpdateCustomerFromApi.Remove(notifiedCustomer);

                        //mAllUpdateCustomerFromApi.Add(cus);
                  //  }
                }

                // save customers
                

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
                    PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);


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
                    // Generate a message summary for the body of the notification:
                
                  //  if(item.DidGetNewInoviceToday == false)
                  //  {
                        string money = item.MoneyToPay.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("bg-BG"));

                        inboxStyle.AddLine($"Аб. номер: {item.Nomer.ToString()}, {money}");

                        bulideer.SetContentText($"Аб. номер: {item.Nomer.ToString()}, {money}");

                       // item.DidGetNewInoviceToday = true;
                  //  }
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
                    PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);


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
                  // if(cus.DidGetReadingToday == false)
                 //   {
                        // Generate a message summary for the body of the notification:
                        string format = "dd.MM.yyyy";
                        string date = cus.StartReportDate.ToString(format);

                        inboxStyle.AddLine($"Аб. номер: {cus.Nomer.ToString()}, {date}");

                        bulideer.SetContentText($"Аб. номер: {cus.Nomer.ToString()}, {date}");

                        //cus.DidGetReadingToday = true;
                        //cus.DidGetAnyNotificationToday = true;

                        //Customer notifiedCustomer = mAllUpdateCustomerFromApi.FirstOrDefault(c => c.EGN == cus.EGN);
                        //mAllUpdateCustomerFromApi.Remove(notifiedCustomer);

                        //mAllUpdateCustomerFromApi.Add(cus);
                   // }

                }

                //SaveCustomersFromApiInPhone(mAllUpdateCustomerFromApi);

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

             //   ///// saveeeee
             //   ISharedPreferences pref =
             //Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

             //   // convert the list to json
             //   var listOfCustomersAsJson = JsonConvert.SerializeObject(mAllUpdateCustomerFromApi); // mCustomers

             //   ISharedPreferencesEditor editor = pref.Edit();

             //   // set the value to Customers key
             //   editor.PutString("Customers", listOfCustomersAsJson);


             //   // commit the changes
             //   editor.Commit();

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

        //private Message GetmessageFromPreferencesInPhone()
        //{
        //    // get shared preferences
        //    ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

        //    // read exisiting value
        //    var messageAsString = pref.GetString("MessageFromApi", null);

        //    // if preferences return null, initialize listOfCustomers
        //    if (messageAsString == null)
        //        return new Message();

        //    var message = JsonConvert.DeserializeObject<Message>(messageAsString);

        //    if (message == null)
        //        return new Message();

        //    return message;
        //}

        public List<Customer> CheckOverdue(List<Customer> abonatiList)
        {
            HttpClientHandler handler = new HttpClientHandler();

            var client = new System.Net.Http.HttpClient(handler, false);

            string json = JsonConvert.SerializeObject(abonatiList);
            StringContent data = new StringContent("=" + json, Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = client.PostAsync("http://192.168.2.222/VIKWebApi/api/CheckOverdue/", data).Result;

            string result = response.Content.ReadAsStringAsync().Result;

            List<Customer> abonatiListReturn = new List<Customer>(); // long ???

            //   JsonConvert.PopulateObject(result, abonatiListReturn);

            string res = result.ToString();  // ??? result  // ?? res

            // mGetCheckedOverdueCustomrsFromApiAsString = result;
            //mCustomersShort = abonatiListReturn;

            //   return abonatiListReturn;

            //     abonatiListReturn = JsonConvert.DeserializeObject<List<Customer>>(res);

            return abonatiListReturn;
        }

    }
}