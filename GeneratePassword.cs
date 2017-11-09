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
    public  class GeneratePassword
    {
        public  string secretPass = "vasP#df40176_gooW";

        public string GetDeviceName()
        {
           // var ds = DateTime.Now.Ticks;

            string manufacturer = Build.Manufacturer;
            string model = Build.Model;

            if (model.StartsWith(manufacturer))
            {
                return model;
                //  return capitalize(model);
            }
            else
            {
                return manufacturer + model;
                // return capitalize(manufacturer) + " " + model; //"Samsung GT-N8010"             }         }
            }
        }

        public string GetDateTimeTiks()
        {
            var currentTiks = DateTime.Now.Ticks;

            return currentTiks.ToString();

        }
    }
}