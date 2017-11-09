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
   public class GrudMessageFromPreferemces
    {
        public Message GetMessageFromPreferencesInPhone()
        {
            // get shared preferences
            ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var messageAsString = pref.GetString("MessageFromApi", null);

            // if preferences return null, initialize listOfCustomers
            if (messageAsString == null)
            {
                return new Message();
            }

            var message = JsonConvert.DeserializeObject<Message>(messageAsString);

            if (message == null)
                return new Message();

            return message;
        }

        public  void SaveMessageInPhone(Message newMessage)
        {
            ISharedPreferences pref =
            Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // convert the list to json
            var messageAsJson = JsonConvert.SerializeObject(newMessage);

            ISharedPreferencesEditor editor = pref.Edit();

            // set the value to Customers key
            editor.PutString("MessageFromApi", messageAsJson);

            // commit the changes
            editor.Commit();
        }
    }
}