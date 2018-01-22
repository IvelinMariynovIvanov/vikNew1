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
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Newtonsoft.Json;
using Org.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Java.Lang;
using System.Net;


namespace ListViewTask
{
    [Activity(Label = "Добави абонат")]
    public class AddCustomer : AppCompatActivity
    {
        private Button mAddCustomer;
        private Android.Support.V7.Widget.Toolbar mToolBar;
        private List<Customer> mCustomers;
        private EditText mBillNumber;
        private EditText mEgn;
        private TextView mError;
        // private ProgressBar mProgressBar;

        private Android.App.ProgressDialog progress;

        private bool mIsExist = false;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AddCustomerView);

            mCustomers = new List<Customer>();

            mError = FindViewById<TextView>(Resource.Id.error);
            mAddCustomer = FindViewById<Button>(Resource.Id.AddCustomer);
            mBillNumber = FindViewById<EditText>(Resource.Id.BillNumber);
            mEgn = FindViewById<EditText>(Resource.Id.EGN);
            mToolBar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.basicToolbar1);
           // mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
         
            mError.Visibility = ViewStates.Invisible;
            //mProgressBar.Visibility = ViewStates.Invisible;
           
            SetSupportActionBar(mToolBar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            mAddCustomer.Click += MAddCustomer_Click;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            this.Finish();
        }

        private void MAddCustomer_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() => { StarShowingPtogressDialog(); });
   
            //antother thread
            Thread thread = new Thread(AddNewCustomer);

            thread.Start();
        }

        private void StarShowingPtogressDialog()
        {
            progress = new Android.App.ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            progress.SetMessage("Добавяне на абонат ...");
            progress.SetCancelable(false);
            progress.Show();
        }

        private void AddNewCustomer()
        {
            

            RunOnUiThread(() => { mError.Visibility = ViewStates.Invisible; });
   
            // get shared preferences
            ISharedPreferences pref = Application.Context.GetSharedPreferences("PREFERENCE_NAME", FileCreationMode.Private);

            // read exisiting value
            var customersJsonString = pref.GetString("Customers", null);

            // if preferences return null, initialize listOfCustomers
            if (customersJsonString == null || customersJsonString == "[]")
            {
                this.mCustomers = new List<Customer>();
            }

            else
            {
                this.mCustomers = JsonConvert.DeserializeObject<List<Customer>>(customersJsonString);
            }

            // if deserialization return null, initialize listOfCustomers
            if (this.mCustomers == null )
            {
                this.mCustomers = new List<Customer>();
            }

            #region add your object to list of customers

            string billNumber = mBillNumber.Text.ToString();
            string egn = mEgn.Text.ToString();

            if(mCustomers.Count < 5)
            {
                if (billNumber.ToString().Trim().Length > 3 &&
                   egn.ToString().Trim().Length > 9)
                {
                    // there is customers in phone
                    if (mCustomers.Count > 0)
                    {
                        AddNewCustomerWhenTheCollectionInNotEmpty(pref);
                    }
                    // there is no customers in phone
                    else
                    {
                        AddOneCustomer(pref, ref billNumber, ref egn);
                    }
                }
                // Incorrect egn or billNumber
                else
                {
                    RunOnUiThread(() => RefreshProgressDialogAndToastWhenInputForEgnAndBillNumberIsIncorrect());
                }
            }
            else
            {
                RunOnUiThread(() => 
                    {
                        mError.Text = "Можете да добавяте до пет абоната";
                        mError.Visibility = ViewStates.Visible;

                        progress.Dismiss();
                    }
                );
            }


            #endregion
        }

        private void RefreshProgressDialogAndToastWhenInputForEgnAndBillNumberIsIncorrect()
        {
            mError.Text = "Некоректни данни";
            mError.Visibility = ViewStates.Visible;

            progress.Dismiss();
        }

        private  void AddNewCustomerWhenTheCollectionInNotEmpty(ISharedPreferences pref)
        {
            string billNumber = mBillNumber.Text.ToString();
            string egn = mEgn.Text.ToString();

            bool isThisCustomerAlreadyExist = false;

            Looper.Prepare();
            
            foreach (var customer in mCustomers)
            {
                // check in phone is there a customer with this billNumber
                if (customer.Nomer == billNumber) //!=
                {
                    isThisCustomerAlreadyExist = true;

                    RunOnUiThread(() => 
                    {
                        RefreshErrorAndProgressBarWhenCustomerAlreadyExist(billNumber);
                    });
                }
            }
            if (isThisCustomerAlreadyExist == false)
            {
                AddOneCustomer(pref, ref billNumber, ref egn);
            }
        }

        private void RefreshErrorAndProgressBarWhenCustomerAlreadyExist(string billNumber)
        {
            Toast.MakeText(this, $" Вече има абонат с абонатен номер {billNumber}", ToastLength.Long).Show();
            mError.Visibility = ViewStates.Visible;
            mError.Text = "Абоната е вече добавен";
            //mProgressBar.Visibility = ViewStates.Invisible;

            progress.Dismiss();
        }

        // when there is no customers in phone
        private void AddOneCustomer(ISharedPreferences pref, ref string billNumber, ref string egn)
        {
            EncrypConnection encryp = new EncrypConnection();


            string crypFinalPass = encryp.Encrypt();

            ConnectToApi connectToApi = new ConnectToApi();

            // to use RefreshErrorAndProgresBarWhenSuccsesfullyAddACustomer
            string localParamBillNumber = billNumber;   

            //check the connection
            bool connection = connectToApi.CheckConnectionOfVikSite(); 

            // check if connection is ok
            if (connection == true)
            {

                // test api url
                //  string realUrl = ConnectToApi.urlAPI + "api/abonats/" + crypFinalPass + "/" + billNumber + "/" + egn;

                ////real api url
                string realUrl = ConnectToApi.urlAPI + "api/abonats/" + crypFinalPass + "/" + billNumber + "/" + egn + "/"
                                + ConnectToApi.updateByAddCutomerButton + "/";

                var jsonResponse = connectToApi.FetchApiDataAsync(realUrl); 

                //check the api
                if (jsonResponse == null)
                {
                    RunOnUiThread(() => 
                    {
                        RefreshErrorAndProgressBarWhenCanNotConnectToApi();
                    });

                    return;
                }

                // check is billNumber correct and get and save customer in phone
                else if (jsonResponse != null)
                {
                    Customer newCustomer = connectToApi.GetCustomerFromApi(jsonResponse);

                    mIsExist = newCustomer.IsExisting;

                   // check in vikSite is there a customer with this billNumber(is billNumber correct)
                        if (mIsExist == false)
                        {
                            RunOnUiThread(() =>
                            {
                                RefreshErrorAndProgressBarWhenEgnOrBillNumberIsNotCorrect();
                            });
                        }

                        else if(newCustomer != null && mIsExist == true)
                        {
                            mCustomers.Add(newCustomer);

                            // convert the list to json
                            var listOfCustomersAsJson = JsonConvert.SerializeObject(this.mCustomers);

                            ISharedPreferencesEditor editor = pref.Edit();

                            bool isAddedAnewCustomer = true;
                            bool isAlreadyBeenUpdated = false;

                            string isAddedAnewCustomerAsString = JsonConvert.SerializeObject(isAddedAnewCustomer);
                            string isAlreadyBeenUpdatedAsString = JsonConvert.SerializeObject(isAlreadyBeenUpdated);

                            // set the value to Customers key
                            editor.PutString("Customers", listOfCustomersAsJson);

                            editor.PutString("isAddedAnewCustomer", isAddedAnewCustomerAsString);
                            editor.PutString("isAlredyBeenUpdated", isAlreadyBeenUpdatedAsString);

                            // commit the changes
                            editor.Commit();

                            RunOnUiThread(() =>
                            {
                                RefreshErrorAndProgresBarWhenSuccsesfullyAddACustomer(localParamBillNumber);
                            });

                            var intent = new Intent(this, typeof(MainActivity));
                            StartActivity(intent);
                        }

                   else
                    {
                        RunOnUiThread(() => 
                        {
                            RefreshErrorAndProgressBarWhenCanNotConnectToApi();
                        });
                    }
                }
            }

           // check if connection is not ok
            else
            {
                RunOnUiThread(() => RefreshProgressDialogAndToastWhenThereIsNoConnection());

                return;   
            }
        }

        private void RefreshProgressDialogAndToastWhenThereIsNoConnection()
        {
            progress.Dismiss();
            Toast.MakeText(this, "Проверете интернет връзката", ToastLength.Long).Show();
            mError.Text = "Проверете интернет връзката";
            mError.Visibility = ViewStates.Visible;


        }


        private void RefreshErrorAndProgresBarWhenSuccsesfullyAddACustomer(string billNumber)
        {
            Toast.MakeText(this, $" Успешно добавихте абонат с абонатен номер {billNumber}", ToastLength.Long).Show();
            mError.Visibility = ViewStates.Invisible;
        }

        private void RefreshErrorAndProgressBarWhenEgnOrBillNumberIsNotCorrect()
        {
            mError.Visibility = ViewStates.Visible;
            string StatusString = "Несъщесвуващ абонат";  //
            Toast.MakeText(this, $"{StatusString}", ToastLength.Long).Show();

            mError.Text = StatusString;
            //mProgressBar.Visibility = ViewStates.Invisible;

            progress.Dismiss();
        }

        private void RefreshErrorAndProgressBarWhenCanNotConnectToApi()
        {
            mError.Visibility = ViewStates.Visible;
            string StatusString = "Грешка при извличане на данни";
            Toast.MakeText(this, $"{StatusString}", ToastLength.Long).Show();

            mError.Text = StatusString;
          
            progress.Dismiss();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.basic_menu, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}