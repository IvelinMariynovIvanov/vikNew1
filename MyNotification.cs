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
using Android.Support.V7.App;
using Android.App.Job;

namespace ListViewTask
{
    public class MyNotification : AppCompatActivity
    {
        //private Intent mIntent;
        ////private Context mContext;

        ////public MyNotification(Context context)
        ////{
        ////    this.mContext = context;
        ////}

        public void SentNotificationForOverdue(List<Customer> countНotifyInvoiceOverdueCustomers)
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
                    inboxStyle.AddLine($"Абонатен номер: {item.Nomer.ToString()}");

                    bulideer.SetContentText($"Абонатен номер: {item.Nomer.ToString()}");
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
                    inboxStyle.AddLine($"Абонатен номер: {item.Nomer.ToString()}");

                    bulideer.SetContentText($"Абонатен номер: {item.Nomer.ToString()}");
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
                Notification.Builder bulideer = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.vik)
                .SetContentIntent(pendingIntent);

                // Set the title and text of the notification:
                bulideer.SetContentTitle("Ден на отчитане");

                foreach (var item in countНotifyReadingustomers)
                {
                    // Generate a message summary for the body of the notification:
                    inboxStyle.AddLine($"Абонатен номер: {item.Nomer.ToString()}");

                    bulideer.SetContentText($"Абонатен номер: {item.Nomer.ToString()}");

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

        public void SentNotificationWithoutSubscribe(Message newMessage)
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

        //public override bool OnStartJob(JobParameters @params)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool OnStopJob(JobParameters @params)
        //{
        //    throw new NotImplementedException();
        //}
    }
}