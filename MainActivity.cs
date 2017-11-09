using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Views;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System;
using Android.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Export = Java.Interop.ExportAttribute;
using Android.Graphics;
using Android.App.Job;
using Android.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using Android.Content.PM;

namespace ListViewTask
{
    [Activity(Label = "ВиК", 
        MainLauncher = true, 
        Icon = "@drawable/vik",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        public List<Customer> mCustomers = new List<Customer>();
        public List<Customer> mCutomersFromNotification = new List<Customer>();
        public string mCustomersFromNotificationAsString = string.Empty;


        private List<Customer> countНotifyReadingustomers = new List<Customer>();
        private List<Customer> countНotifyInvoiceOverdueCustomers = new List<Customer>();
        private List<Customer> countNewНotifyNewInvoiceCustomers = new List<Customer>();
        private List<Customer> mCustomerFromApiToNotifyToday = new List<Customer>();
        private List<Customer> mAllUpdateCustomerFromApi = new List<Customer>();
        public string mInfoFromNotification = string.Empty;

        private Android.Support.V7.Widget.Toolbar mToolBar;
        private Android.Telephony.TelephonyManager mTelephonyMgr;
        private string mPhoneNumber;
      //  private CustomerListWrapper customerWrapper = new CustomerListWrapper();
        private ListView mListView;
        private TextView mHour;
        private TextView mDate;
        private TextView mAbonati;
        private TextView mObnoveniKum;

        private Android.App.ProgressDialog progress;


        private string mInfoFromNewInoviceNotification;
        private string mInfoFromOverdueNotification;
        private string mInfoFromReadingNotification;

        private int fetchFromPressButton = 1;


        #region Job Service fields
        public int kJobId;
        public JobScheduler mJobScheduler ;

        private bool isServiceAlreadyRunning = false;
        private ComponentName JobScheduler;
        private MyJobService testService;
        #endregion

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //  Thread.CurrentThread.CurrentCulture = new CultureInfo("bg-BG");

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("bg-BG");


            mHour = FindViewById<TextView>(Resource.Id.Hour);
            mDate = FindViewById<TextView>(Resource.Id.Date);

            mAbonati = FindViewById<TextView>(Resource.Id.Customers);
            mObnoveniKum = FindViewById<TextView>(Resource.Id.Updated);

            mListView = FindViewById<ListView>(Resource.Id.myListView);
            mToolBar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolBar);
            SupportActionBar.Title = "Начало ";

            mJobScheduler =  (JobScheduler)GetSystemService(Context.JobSchedulerService);
            //  mJobScheduler = (JobScheduler)(android.app.JobSchedulerImpl@8652ee9);
            // JobInfo job = new JobInfo();
            // mJobScheduler.Schedule(job);

           //  mJobScheduler.CancelAll();
         
            // !=
            if (mJobScheduler == null || mJobScheduler.AllPendingJobs.Count == 0)
            {
                StartService();
            }
        }
        protected override void OnStop()
        {
            base.OnStop();
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
        }

        private  void StartService() 
        {

            if(mJobScheduler != null)
            {
                // Thread.CurrentThread.CurrentCulture = new CultureInfo("bg-BG");
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("bg-BG");

                mJobScheduler = (JobScheduler)GetSystemService(Context.JobSchedulerService);
                JobScheduler = new ComponentName(this, Java.Lang.Class.FromType(typeof(MyJobService)));

                JobInfo.Builder builder = new JobInfo.Builder(kJobId++, JobScheduler);

                builder.SetRequiredNetworkType(NetworkType.Any);
                builder.SetPeriodic(40000); // 2400000  40 min, 7200000,  ////////////3600000
                                       // builder.SetPeriodic(900000); // 2400000  40 min,                  // 1500    20 000 - 2s  // 25 min
                builder.SetPersisted(true);

                mJobScheduler.Schedule(builder.Build());
            }

        }

        protected override void OnResume()
        {
            base.OnResume();

            GrudCustomersFromPreferences grudCustomers = new GrudCustomersFromPreferences();

            mCustomers = grudCustomers.GetCustomersFromPreferences();
          //  mCustomers = GetCustomersFromPreferences();

            CustomAdapter adapter = new CustomAdapter(this, mCustomers); //, mInfoFromNewInoviceNotification, mInfoFromReadingNotification, mInfoFromOverdueNotification, mCutomersFromNotification);

            mListView.Adapter = adapter;

            //mHour.Text = GetUpdateHour();
            //mDate.Text = GetUpdateDate();

            if(mCustomers.Count != 0)
            {
                mHour.Text = GetUpdateHour();
                mDate.Text = GetUpdateDate();
            }
            else
            {
                mHour.Visibility = ViewStates.Gone;
                mDate.Visibility = ViewStates.Gone;
                mObnoveniKum.Visibility = ViewStates.Gone;

                mAbonati.Text = "Моля добавете абонати";
            }
        }

        public static string GetUpdateDate()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            var date = pref.GetString("Date", null);
            
            string format = "dd.MM.yyyy"; 

            if (date == null)
            {
                return DateTime.Now.ToString(format);
            }

            var newDate = date;

            if(newDate == null || newDate == "(null)")
            {
                return DateTime.Now.ToString(format);
            }
            return newDate;
        }

        public static string GetUpdateHour()    /////bez static
        {
            // get shared preferences
            ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var hour = pref.GetString("Hour", null);

            // if preferences return null, initialize listOfCustomers
            if (hour == null)
            {
                string DateFormatt = "HH:mm";
                return DateTime.Now.ToString(DateFormatt) + " часа, ";
            }

            var newHour = (hour);

            if (newHour == null || newHour =="(null)")
            {
                string DateFormatt = "HH:mm";
                return DateTime.Now.ToString(DateFormatt);
            }

            return newHour + " часа, " ;
        }

       

        protected override void OnSaveInstanceState(Bundle outState)
        {

            base.OnSaveInstanceState(outState);
        }

        

        private void MListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var row = mCustomers[e.Position];

            Android.Widget.Toast.MakeText(this, row.FullName, Android.Widget.ToastLength.Short).Show();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menu, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            int id = item.ItemId;

            if (id == Resource.Id.menu_addPicture)
            {

                Toast.MakeText(this, "Изпращане на сигнал", ToastLength.Long).Show();
                var intent = new Intent(this, typeof(AddPictureActivityFromGallary));
                StartActivity(intent);

                //View view = FindViewById(Resource.Id.menu_addPicture);  // This is the Id of menu item in Toolbar
                //PopupMenu popUpMenu = new PopupMenu(this, view);

                //popUpMenu.Inflate(Resource.Menu.pop_up_menu);
                //popUpMenu.Show();

                return true;
            }

            else if (id == Resource.Id.menu_redresh)
            {
                RunOnUiThread(() => { ShowProgressDialog(); });

                Thread thread = new Thread(RefreshCustomers);

                thread.Start();
               
                return true;

            }

            else if (id == Resource.Id.menu_add)
            {
                if(mCustomers.Count < 5)
                {
                    Toast.MakeText(this, "Добавяне на абонат", ToastLength.Long).Show();

                    var intent = new Intent(this, typeof(AddCustomer));
                    StartActivity(intent);
                }
                else
                {
                    Toast.MakeText(this, "Не можете да добавяте повече от пет абоната", ToastLength.Long).Show();
                }

                return true;
            }

            //else if (id == Resource.Id.menu_moreOptions)
            //{
            //    string s = string.Empty;
            //    foreach (JobInfo j in mJobScheduler.AllPendingJobs)
            //    {
            //        int jId = j.Id;
            //        //jobScheduler.Cancel(jId);
            //        // s += $"{j.Id}";
            //        s += "job(" + jId + " )";

            //    }
            //    Toast.MakeText(this, $"{s}", ToastLength.Long).Show();
            //    return true;
            //}

            else if (id == Resource.Id.menu_FromGallery)
            {
                var intent = new Intent(this, typeof(AddPictureActivityFromGallary));

                StartActivity(intent);

                return true;
            }
 
            //else if (id == Resource.Id.menu_GetPhoneNumber)
            //{
            //    mTelephonyMgr =
            // (Android.Telephony.TelephonyManager)this.GetSystemService(Android.Content.Context.TelephonyService);
            //    mPhoneNumber = mTelephonyMgr.Line1Number;

            //    Toast.MakeText(this, $"The phone number is {mPhoneNumber}", ToastLength.Long).Show();

            //    return true;
            //}

            return base.OnOptionsItemSelected(item);
        }

        private void RefreshCustomers()
        {
            List<Customer> customers;

            ISharedPreferences pref;

            customers = new List<Customer>();

            // get shared preferences
            pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var customersJsonString = pref.GetString("Customers", null);

            // if preferences return null, initialize listOfCustomers
            if (customersJsonString == null)
            {
                customers = new List<Customer>();
            }

            else
            {
                customers = JsonConvert.DeserializeObject<List<Customer>>(customersJsonString);
            }

            // if deserialization return null, initialize listOfCustomers
            if (customers == null)
                customers = new List<Customer>();

            //chek if there is no customers to refrsh
            if (customers.Count == 0)  
            {
                RunOnUiThread(() => RefreshProgressDialogAndToatWhenThereIsNoCustomers());

                ///////

                

                return;
            }
            else if (customers.Count != 0) 
            {
                UpdateCustomers(customers, pref);
            }
        }

        private void RefreshProgressDialogAndToatWhenThereIsNoCustomers()
        {
            ////////////////////
            mHour.Visibility = ViewStates.Gone;
            mDate.Visibility = ViewStates.Gone;
            mObnoveniKum.Visibility = ViewStates.Gone;

            mAbonati.Text = "Моля добавете абонати";

            Toast.MakeText(this, "Няма абонати за обновяване", ToastLength.Long).Show();
            progress.Dismiss();
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


        private void UpdateCustomers(List<Customer> mCustomers, ISharedPreferences pref)
        {
                ConnectToApi connectToApi = new ConnectToApi();

                bool connection = connectToApi.CheckConnectionOfVikSite();

                if (connection == true)
                {

                EncrypConnection encryp = new EncrypConnection();

               

                string crypFinalPass = encryp.Encrypt();

                //// get from preferences
                GrudMessageFromPreferemces grudMessage = new GrudMessageFromPreferemces();

                int lastMessageId = grudMessage.GetMessageFromPreferencesInPhone().MessageID;


                //realno !!!!!!!!!!!!!!
                string messageUrl = ConnectToApi.urlAPI +"api/msg/";

                ///teest
              //  string messageUrl = "http://192.168.2.222/VIKWebApi/api/msg/";

                string finalUrl = messageUrl + crypFinalPass + "/" + lastMessageId;

                var messageFromApiAsJsonString = connectToApi.FetchApiDataAsync(finalUrl);

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
                          //  myNotification.SentNotificationWithoutSubscribe(newMessage);
                        }
                    }
                }

                foreach (var customer in mCustomers)
                    {

                    bool isReceiveNotifyNewInvoiceCheck = false;
                    bool isReceiveNotifyInvoiceOverdueCheck = false;
                    bool isReciveNotifyReadingCheck = false;

                    isReceiveNotifyNewInvoiceCheck = customer.NotifyNewInvoice;
                    isReceiveNotifyInvoiceOverdueCheck = customer.NotifyInvoiceOverdue;
                    isReciveNotifyReadingCheck = customer.NotifyReading;

                    // if(isAnyNotifycationCheck == true)
                    //   {
                    string billNumber = customer.Nomer.ToString();
                        string egn = customer.EGN.ToString();

                    //CREATE test URL
                     string url = "http://192.168.2.222/VIKWebApi/";

                    /// !!!!!!!!!!!!!!!!!!!!!!!!
                    //string realUrl = ConnectToApi.urlAPI + "api/abonats/" + crypFinalPass + "/"
                    //                 + billNumber + "/" + egn + "/" + ConnectToApi.updateByButtonRefresh + "/";
                    //               // + isReceiveNotifyNewInvoiceToday + "/" + isReceiveNotifyInvoiceOverdueToday + "/" + isReciveNotifyReadingToday + "/";

                    string realUrl = "http://192.168.2.222/VIKWebApi/" + "api/abonats/"
                        + crypFinalPass + "/" + billNumber + "/" + egn + "/" + ConnectToApi.updateByButtonRefresh + "/"
                        + isReceiveNotifyNewInvoiceCheck + "/" + isReceiveNotifyInvoiceOverdueCheck + "/" + isReciveNotifyReadingCheck + "/"; 

                    var jsonResponse = connectToApi.FetchApiDataAsync(realUrl);

                        //check the api
                        if (jsonResponse == null)
                        {
                            RunOnUiThread(() => RefreshProgressDialogAndToastWhenNoConnectioToApi());

                            return;
                        }

                        // check in vikSite is there a customer with this billNumber (is billNumber correct)
                        else if (jsonResponse == "[]")
                        {
                            RefreshProgressDialogAndToastWhenInputIsNotValid();

                            return;
                        }

                        // check is billNumber correct and get and save customer in phone
                        else if (jsonResponse != null)
                        {
                        // var jsonArray = JArray.Parse(jsonResponse);

                        // Customer newCustomer = connectToApi.GetCustomerFromApi(jsonResponse);
                        Customer updateCutomerButNoNotify = connectToApi.GetCustomerFromApi(jsonResponse);

                        if (updateCutomerButNoNotify != null && updateCutomerButNoNotify.IsExisting == true)
                          //  (newCustomer != null && newCustomer.IsExisting == true)
                            {
                           
                            updateCutomerButNoNotify.NotifyNewInvoice = customer.NotifyNewInvoice;
                            updateCutomerButNoNotify.NotifyInvoiceOverdue = customer.NotifyInvoiceOverdue;
                            updateCutomerButNoNotify.NotifyReading = customer.NotifyReading;


                            ///////////////// be6e customer.ReceiveNotifyInvoiceOverdueToday
                            //bool isAnyNotifycationCheck =
                            //         (newCustomer.ReceiveNotifyInvoiceOverdueToday == true ||
                            //         newCustomer.ReceiveNotifyNewInvoiceToday == true ||
                            //         newCustomer.ReciveNotifyReadingToday == true);


                            //    if (isAnyNotifycationCheck == true)
                            //    {
                            //        mCustomerFromApiToNotifyToday.Add(newCustomer);

                            //    }




                          //  Customer updateCutomerButNoNotify = connectToApi.GetCustomerFromApi(jsonResponse);

                            //updateCutomerButNoNotify.NotifyInvoiceOverdue = customer.NotifyInvoiceOverdue;
                            //updateCutomerButNoNotify.NotifyNewInvoice = customer.NotifyNewInvoice;
                            //updateCutomerButNoNotify.NotifyReading = customer.NotifyReading;

                            /////////////////////////////////////////////////////////////////////////////////////////////////////
                            //if(updateCutomerButNoNotify.ReceiveNotifyInvoiceOverdueToday == true)
                            //{
                            //    updateCutomerButNoNotify.ReceiveNotifyInvoiceOverdueToday = true;
                            //}
                            //else
                            //{
                            //    updateCutomerButNoNotify.ReceiveNotifyInvoiceOverdueToday = customer.ReceiveNotifyInvoiceOverdueToday;
                            //}

                            //if (updateCutomerButNoNotify.ReceiveNotifyNewInvoiceToday == true)
                            //{
                            //    updateCutomerButNoNotify.ReceiveNotifyNewInvoiceToday = true;
                            //}
                            //else
                            //{
                            //    updateCutomerButNoNotify.ReceiveNotifyNewInvoiceToday = customer.ReceiveNotifyNewInvoiceToday;
                            //}

                            //if (updateCutomerButNoNotify.ReciveNotifyReadingToday == true)
                            //{
                            //    updateCutomerButNoNotify.ReciveNotifyReadingToday = true;
                            //}
                            //else
                            //{
                            //    updateCutomerButNoNotify.ReciveNotifyReadingToday = customer.ReciveNotifyReadingToday;
                            //}
                            //////////////////////////////////////////////////////////////////////////////////////////////////////


                            //updateCutomerButNoNotify.ReceiveNotifyInvoiceOverdueToday = customer.ReceiveNotifyInvoiceOverdueToday;
                            //updateCutomerButNoNotify.ReceiveNotifyNewInvoiceToday = customer.ReceiveNotifyNewInvoiceToday;
                            //updateCutomerButNoNotify.ReciveNotifyReadingToday = customer.ReciveNotifyReadingToday;


                            //Customer updateCutomerButNoNotify = new Customer();
                            //updateCutomerButNoNotify = newCustomer;

                            //updateCutomerButNoNotify.ReceiveNotifyInvoiceOverdueToday = false;
                            //updateCutomerButNoNotify.ReceiveNotifyNewInvoiceToday = false;
                            //updateCutomerButNoNotify.ReciveNotifyReadingToday = false;

                            mAllUpdateCustomerFromApi.Add(updateCutomerButNoNotify);     ////////////updateCutomerButNoNotify

                        }
                            else
                            {
                                RunOnUiThread(() =>
                                {
                                    RefreshProgressDialogAndToastWhenNoConnectioToApi();
                                });

                                 return;
                            }
                        }
                        // }
                    }

                    #region setting the updating date

                    string updateHour;
                    string updateDate;

                    GetUpdateDateAndHour(out updateHour, out updateDate);

                    #endregion

                    RunOnUiThread(() =>
                    {
                        GetFinalUpdateDateHour();

                    });

                    SelectWhichCustomersTobeNotified(countНotifyReadingustomers, countНotifyInvoiceOverdueCustomers, countNewНotifyNewInvoiceCustomers, mAllUpdateCustomerFromApi); //mCustomerFromApiToNotifyToday

                    SaveUpdatesInPhone(pref, mDate.Text.ToString(), mHour.Text.ToString());

                    //MyNotification myNotification = new MyNotification(this);

                    //myNotification.SentNotificationForOverdue(mCountНotifyInvoiceOverdueCustomers);

                    SentNoficationForNewInovoice(countNewНotifyNewInvoiceCustomers);

                    SentNotificationForOverdue(countНotifyInvoiceOverdueCustomers);

                    SentNotificationForReading(countНotifyReadingustomers);

                    var intent = new Intent(this, typeof(MainActivity));

                    StartActivity(intent);
                }
                else
                {
                    Looper.Prepare();
                    RunOnUiThread(() => RefreshProgresDialogAndToastWhenThereIsNoConnection());

                    return;
                }
            }

        private void SentNotificationForOverdue(List<Customer> countНotifyInvoiceOverdueCustomers)
        {
            if (countНotifyInvoiceOverdueCustomers.Count > 0)
            {
                string countНotifyInvoiceOverdueCustomersAsString = JsonConvert.SerializeObject(countНotifyInvoiceOverdueCustomers);

                // Set up an intent so that tapping the notifications returns to this app:
                Intent intent = new Intent(this, typeof(MainActivity));

                // Create a PendingIntent; 
                const int pendingIntentId = 0;
                PendingIntent pendingIntent =
                    PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

                // Instantiate the Inbox style:
                Notification.InboxStyle inboxStyle = new Notification.InboxStyle();

                //  Instantiate the builder and set notification elements:
                Notification.Builder bulideer = new Notification.Builder(this)
                     .SetContentIntent(pendingIntent)
                     .SetSmallIcon(Resource.Drawable.vik);

                // Set the title and text of the notification:
                bulideer.SetContentTitle("Просрочване");

                foreach (var item in countНotifyInvoiceOverdueCustomers)
                {
                    // Generate a message summary for the body of the notification:

                    string format = "dd.MM.yyyy";
                    string date = item.EndPayDate.ToString(format);

                    inboxStyle.AddLine($"Аб. номер: {item.Nomer.ToString()}, {date}");

                    bulideer.SetContentText($"Аб. номер: {item.Nomer.ToString()}, {date}");
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

        private void SentNoficationForNewInovoice(List<Customer> countNewНotifyNewInvoiceCustomers)
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
                Notification.Builder bulideer = new Notification.Builder(this)
                    .SetContentIntent(pendingIntent)
                     .SetSmallIcon(Resource.Drawable.vik);

                // Set the title and text of the notification:
                bulideer.SetContentTitle("Нова фактура");
                //  bulideer.SetContentText("chimchim@xamarin.com");

                foreach (var item in countNewНotifyNewInvoiceCustomers)
                {
                    // Generate a message summary for the body of the notification:

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

        private void SentNotificationForReading(List<Customer> countНotifyReadingustomers)
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
                Notification.Builder bulideer = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.vik)
                .SetContentIntent(pendingIntent);

                // Set the title and text of the notification:
                bulideer.SetContentTitle("Ден на отчитане");

                foreach (var item in countНotifyReadingustomers)
                {
                    // Generate a message summary for the body of the notification:
                    string format = "dd.MM.yyyy";
                    string date = item.StartReportDate.ToString(format);

                    inboxStyle.AddLine($"Аб. номер: {item.Nomer.ToString()}, {date}");

                    bulideer.SetContentText($"Аб. номер: {item.Nomer.ToString()}, {date}");

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

        private void RefreshProgressDialogAndToastWhenInputIsNotValid()
        {
            string StatusString = "Няма абонат с този номер и егн";
            Toast.MakeText(this, $"{StatusString}", ToastLength.Short).Show();
            progress.Dismiss();
        }

        private void RefreshProgressDialogAndToastWhenNoConnectioToApi()
        {
            string StatusString = "Грешка при извличане на данните";
            Toast.MakeText(this, $"{StatusString}", ToastLength.Short).Show();
            progress.Dismiss();
        }

        private void RefreshProgresDialogAndToastWhenThereIsNoConnection()
        {
            progress.Dismiss();
            string StatusString = "Проверете интернет връзката";
            Toast.MakeText(this, $"{StatusString}", ToastLength.Long).Show();
        }


        private static void SelectWhichCustomersTobeNotified
        (List<Customer> countНotifyReadingustomers, List<Customer> countНotifyInvoiceOverdueCustomers, List<Customer> countNewНotifyNewInvoiceCustomers, List<Customer> mAllUpdateCustomerFromApi) /////mCustomerFromApiNoNotifyToday
        {
            foreach (var customer in mAllUpdateCustomerFromApi)   //// mCustomerFromApiToNotifyToday
            {
                //customer.ReceiveNotifyInvoiceOverdueToday = true;
                //customer.ReceiveNotifyNewInvoiceToday = true;
                //customer.ReciveNotifyReadingToday = true;

                //// isAnyNotifycationCheck
                bool haveToRecieveNotificationToday =
                    (customer.ReceiveNotifyInvoiceOverdueToday == true ||
                    customer.ReceiveNotifyNewInvoiceToday == true ||
                    customer.ReciveNotifyReadingToday == true);
 
                if (haveToRecieveNotificationToday == true)
                {
    
                    if (customer.ReceiveNotifyNewInvoiceToday == true && customer.NotifyNewInvoice == true)
                    {
                       // customer.ReceiveNotifyNewInvoiceToday = false;

                        countNewНotifyNewInvoiceCustomers.Add(customer);
                    }
                    if (customer.ReceiveNotifyInvoiceOverdueToday == true && customer.NotifyInvoiceOverdue == true)
                    {
                        //customer.ReceiveNotifyInvoiceOverdueToday = false;

                        countНotifyInvoiceOverdueCustomers.Add(customer);
                    }
                    if (customer.ReciveNotifyReadingToday == true && customer.NotifyReading == true)
                    {
                       // customer.ReciveNotifyReadingToday = false;

                        countНotifyReadingustomers.Add(customer);
                    }
                }
            }
        }

        private void GetFinalUpdateDateHour()
        {

            mDate.Text = mDate.Text.ToString() ;
            mHour.Text = mHour.Text.ToString() ;
        }

        private void SaveUpdatesInPhone(ISharedPreferences pref, string updateHour, string updateDate)
        {
            DateTime updateHourAndDate = DateTime.Now;
            
            string DateFormatt = "HH:mm";
            string format = "dd.MM.yyyy";

            string shortReportDatetHour = updateHourAndDate.ToString(DateFormatt);

            updateHour = updateHourAndDate.ToShortTimeString();  // + " часа, ";
            updateDate = updateHourAndDate.ToString(format);
            //mHour.Text = updateHour;
            //mDate.Text = updateDate;

           // bool isUpdated = true;

            // convert the list to json
            var listOfCustomersAsJson = JsonConvert.SerializeObject(mAllUpdateCustomerFromApi);  

            ISharedPreferencesEditor editor = pref.Edit();

            // set the value to Customers key
            editor.PutString("Customers", listOfCustomersAsJson);

         //   editor.PutString("isUpdated", Convert.ToString(isUpdated));
            editor.PutString("Hour", updateHour);
            editor.PutString("Date", updateDate);

            // commit the changes
            editor.Commit();
        }

        private void GetUpdateDateAndHour(out string updateHour, out string updateDate)
        {
            updateHour = string.Empty;
            updateDate = string.Empty;

            string localParamHour ;
            string localParamDate;

            RunOnUiThread(() => { GetUpdateDateAndHourForMainThread(out localParamHour, out localParamDate); });
            
        }

        private void GetUpdateDateAndHourForMainThread(out string updateHour, out string updateDate)
        {

            DateTime updateHourAndDate = DateTime.Now;

            string DateFormatt = "HH:mm";
            string shortReportDatetHour = updateHourAndDate.ToString(DateFormatt);

            updateHour = updateHourAndDate.ToString(DateFormatt) + " часа, ";
            updateDate = updateHourAndDate.ToShortDateString();
            mHour.Text = updateHour;
            mDate.Text = updateDate; 

        }

        private void ShowProgressDialog()
        {
          //  Looper.Prepare();

            progress = new Android.App.ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            progress.SetMessage("Обновяване ...");
            progress.SetCancelable(false);
            progress.Show();
        }

    }
}

