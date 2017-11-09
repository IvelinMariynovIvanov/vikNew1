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
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace ListViewTask
{
   public class ConnectToApi
    {
        //   http://87.120.225.253:8080/api/check/258
        //  public static string urlAPI = "http://vik-ruse.com:8080/";
        public static string urlAPI = "http://87.120.225.253:8080/";

        public static string testApi = "http://192.168.2.222/VIKWebApi/api/abonats/";

        public static string wtf = "http://192.168.2.222/VIKWebApi/api/";

    //    public static string sentSignal = "http://192.168.2.222/";

        public static int updateByButtonRefresh = 1;

        public static int updateByAutoService = 0;  //refresh notification from api

        public static int updateByAddCutomerButton = 2; // do not refresh notification from api


        public  Customer GetCustomerFromApi(string jsonResponse)
        {
            try
            {
                // parse the list
                var jsonArray = JArray.Parse(jsonResponse);

                Customer newCustomer = new Customer();

                //  parse  json response to get the fullname, moneytopay, reportday
                var jsonArrayElement = jsonArray[0];

                newCustomer.FullName = (jsonArrayElement["Name"].ToString());
                newCustomer.Nomer = (jsonArrayElement["Nomer"].ToString());
                newCustomer.EGN = (jsonArrayElement["EGN"].ToString());
                newCustomer.Address = (jsonArrayElement["Address"].ToString());
                newCustomer.MoneyToPay = (double)(jsonArrayElement["Sum"]);
                newCustomer.OldBill = (double)(jsonArrayElement["SumOld"]);

                newCustomer.ReceiveNotifyNewInvoiceToday = (bool)(jsonArrayElement["notifyNewInvoice"]);
                newCustomer.ReceiveNotifyInvoiceOverdueToday = (bool)(jsonArrayElement["notifyInvoiceOverdue"]);
                newCustomer.ReciveNotifyReadingToday = (bool)(jsonArrayElement["notifyReading"]);

                newCustomer.EndPayDate = (DateTime)jsonArrayElement["PaymentDate"];
                newCustomer.StartReportDate = (DateTime)jsonArrayElement["DataOtchetOt"];
                newCustomer.EndReportDate = (DateTime)jsonArrayElement["DataOtchetDo"];

                newCustomer.IsExisting = (bool)(jsonArrayElement["IsExisting"]);

                return newCustomer;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public Message GetMessageFromApi(string jsonResponse)
        {

            Message message = new Message();

            JsonConvert.PopulateObject(jsonResponse, message);

            return message;
        }

        public string FetchApiDataAsync(string url)
        {
            /// new thread
            /// 

            string jsonDoc = null;

            var client = new System.Net.Http.HttpClient();

            try
            {
                var response = client.GetAsync(url).Result;

                var result = response.Content.ReadAsStringAsync().Result;

                jsonDoc = result;
            }
            catch (Exception ex)
            {
                return null;
            }


            return jsonDoc;
        }

        public bool CheckConnectionOfVikSite()
        {
            bool result = false;
 
            try
            {

                HttpClient httpClient = new HttpClient();

                httpClient.Timeout = TimeSpan.FromMilliseconds(15000);  /// was 1000

                var status = httpClient.GetAsync("http://vik-ruse.com").Result.StatusCode;  //Result.StatusCode

                if (status == System.Net.HttpStatusCode.OK)
                {
                    result = true;
                }
                else   // without else
                {
                    result = false;
                }

            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }
    }
}