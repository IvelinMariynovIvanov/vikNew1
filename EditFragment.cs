using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ListViewTask
{
   

    // My custom Event  class
    public class OnEditCustomerEventArgs : EventArgs
    {
        private bool isThereANewCharge;
        private bool isThereALateBill;
        private bool isThereAReport;
        private int currentPossition;

        private int possUp;
        private int possDown;

        public OnEditCustomerEventArgs()
        {

        }

        public OnEditCustomerEventArgs(bool isThereANewCharge, bool isThereALateBill, bool isThereAReport, int currentPossition)
        {
            this.IsThereANewCharge = isThereANewCharge;
            this.IsThereALateBill = isThereALateBill;
            this.IsThereAReport = isThereAReport;
            this.CurrentPossition = currentPossition;
        }

        public bool IsThereANewCharge { get => isThereANewCharge; set => isThereANewCharge = value; }
        public bool IsThereALateBill { get => isThereALateBill; set => isThereALateBill = value; }
        public bool IsThereAReport { get => isThereAReport; set => isThereAReport = value; }
        public int CurrentPossition { get => currentPossition; set => currentPossition = value; }

        public int PossUp { get => possUp; set => possUp = value; }
        public int PossDown { get => possDown; set => possDown = value; }
       
    }

    public class EditFragment : DialogFragment
    {
        #region Fields

        private CheckBox mNewCharge;
        private CheckBox mLateCharge;
        private CheckBox mReport;

        private Button mEdit;
        private Button mCancel;

        public ImageView mPossUp;
        public ImageView mPossDown;
        private TextView mCurrentPoss;

        public int mCurrentPosition;
        private int mCustomresCount;

        private bool mIsNewCharge;
        private bool mIsLateCharge;
        private bool mIsReport;

        #endregion

     
        public event EventHandler<OnEditCustomerEventArgs> OnEditCustomerComplete ;
        

        public EditFragment()
        {

        }

        public EditFragment(int mCurrentPosition, int mCustomresCount, bool isNewCharge, bool isLateCharge, bool isReport)
        {
            this.mCurrentPosition = mCurrentPosition;
            this.mCustomresCount = mCustomresCount;
            this.mIsNewCharge = isNewCharge;
            this.mIsLateCharge = isLateCharge;
            this.mIsReport = isReport;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here

        }

        // here i get info in fileds to handle the rotation 
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            this.Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_Animation;

            if (savedInstanceState != null)
            {
                mCurrentPosition = savedInstanceState.GetInt("current_Poss");
                mCustomresCount = savedInstanceState.GetInt("count_Customers");
                mIsNewCharge = savedInstanceState.GetBoolean("newCharge");
                mIsLateCharge = savedInstanceState.GetBoolean("lateCharge");
                mIsReport = savedInstanceState.GetBoolean("report");
               
                #region check Ckeckbox Status
                if (mIsNewCharge == true)
                {
                    mNewCharge.Checked = true;
                }
                else if (mIsNewCharge == false)
                {
                    mNewCharge.Checked = false;
                }

                if (mIsLateCharge == true)
                {
                    mLateCharge.Checked = true;
                }
                else if (mIsLateCharge == false)
                {
                    mLateCharge.Checked = false;
                }

                if (mIsReport == true)
                {
                    mReport.Checked = true;
                }
                else if (mIsReport == false)
                {
                    mReport.Checked = false;
                }
                #endregion

                #region check Position
                if (mCurrentPosition == 0)  // check if the first position can be move up
                {
                    mCurrentPoss.Text = (mCurrentPosition + 1).ToString();
                    mPossUp.Visibility = ViewStates.Invisible;
                    mPossDown.Visibility = ViewStates.Visible;

                }

                if (mCurrentPosition == mCustomresCount - 1) // check if the last position can be move down
                {
                    mCurrentPoss.Text = (mCurrentPosition + 1).ToString();
                    mPossDown.Visibility = ViewStates.Invisible;

                }

                if (mCurrentPosition != 0 && mCurrentPosition != mCustomresCount - 1)
                {
                    mCurrentPoss.Text = (mCurrentPosition + 1).ToString();
                    mPossUp.Visibility = ViewStates.Visible;
                    mPossDown.Visibility = ViewStates.Visible;
                }

                #endregion
            }
           
        }

        // here i save info in fileds to handle the rotation 
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            outState.PutInt("current_Poss", mCurrentPosition);
            outState.PutInt("count_Customers", mCustomresCount);
            outState.PutBoolean("newCharge", mIsNewCharge);
            outState.PutBoolean("lateCharge", mIsLateCharge);
            outState.PutBoolean("report", mIsReport);
        }

        public override void OnResume()
        {
            base.OnResume();

          //  SetContentView(Resource.Layout.EditFragment);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);

            this.Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_Animation;


            View view = InflateControls(inflater, container);

            AttachEvents();

            CheckCheckBoxStatus();

            CheckPossition();

            return view;
        }

        private View InflateControls(LayoutInflater inflater, ViewGroup container)
        {
            var view = inflater.Inflate(Resource.Layout.EditFragment, container, false);

            mNewCharge = view.FindViewById<CheckBox>(Resource.Id.newCharge);
            mLateCharge = view.FindViewById<CheckBox>(Resource.Id.lateBIll);
            mReport = view.FindViewById<CheckBox>(Resource.Id.report);

            mEdit = view.FindViewById<Button>(Resource.Id.editFragment);
            mCancel = view.FindViewById<Button>(Resource.Id.cancelFragment);

            mCancel.Text = "Отказ";
            mEdit.Text = "Запази";
           

            mPossUp = view.FindViewById<ImageView>(Resource.Id.PlusOnePoss);
            mPossDown = view.FindViewById<ImageView>(Resource.Id.MinusOnePoss);
            mCurrentPoss = view.FindViewById<TextView>(Resource.Id.possitionValue);

            return view;
        }

        private void AttachEvents()
        {
            mEdit.Click += MEdit_Click;
            mCancel.Click += MCancel_Click;

            mNewCharge.Click += MNewCharge_Click;
            mLateCharge.Click += MLateBill_Click;
            mReport.Click += MReport_Click;
            mPossUp.Click += MPossUp_Click;
            mPossDown.Click += MPossDown_Click;
        }

        private void CheckCheckBoxStatus()
        {
            if (mIsNewCharge == true)
            {
                mNewCharge.Checked = true;
            }
            else if (mIsNewCharge == false)
            {
                mNewCharge.Checked = false;
            }

            if (mIsLateCharge == true)
            {
                mLateCharge.Checked = true;
            }
            else if (mIsLateCharge == false)
            {
                mLateCharge.Checked = false;
            }

            if (mIsReport == true)
            {
                mReport.Checked = true;
            }
            else if (mIsReport == false)
            {
                mReport.Checked = false;
            }
        }

        private void CheckPossition()
        {
            if (mCurrentPosition == 0)  // check if the first position can be move up
            {
                mCurrentPoss.Text = (mCurrentPosition + 1).ToString();
                mPossUp.Visibility = ViewStates.Invisible;
                mPossDown.Visibility = ViewStates.Visible;
            }

            if (mCurrentPosition == mCustomresCount - 1) // check if the first position can be move down
            {
                mCurrentPoss.Text = (mCurrentPosition + 1).ToString();
                mPossDown.Visibility = ViewStates.Invisible;
            }

            if (mCurrentPosition != 0 && mCurrentPosition != mCustomresCount - 1)
            {
                mCurrentPoss.Text = (mCurrentPosition + 1).ToString();
                mPossUp.Visibility = ViewStates.Visible;
                mPossDown.Visibility = ViewStates.Visible;
            }
        }

        private void MPossDown_Click(object sender, EventArgs e)
        {

            mCurrentPoss.Text = ((++mCurrentPosition)+1).ToString();

            if(mCurrentPosition == 0)
            {
                mPossUp.Visibility = ViewStates.Invisible;
                mPossDown.Visibility = ViewStates.Visible;
            }
            else if (mCurrentPosition != 0 && mCurrentPosition != mCustomresCount - 1)
            {
                mPossUp.Visibility = ViewStates.Visible;
                mPossDown.Visibility = ViewStates.Visible;
            }
            else  if (mCurrentPosition == mCustomresCount - 1)
            {
                mPossDown.Visibility = ViewStates.Invisible;
                mPossUp.Visibility = ViewStates.Visible;
            }
  
        }

        private void MPossUp_Click(object sender, EventArgs e)
        {
            mCurrentPoss.Text = ((--mCurrentPosition) + 1).ToString();

            if (mCurrentPosition == 0)
            {
                mPossUp.Visibility = ViewStates.Invisible;
                mPossDown.Visibility = ViewStates.Visible;

            }
            else if (mCurrentPosition != 0 && mCurrentPosition != mCustomresCount - 1)
            {
                mPossUp.Visibility = ViewStates.Visible;
                mPossDown.Visibility = ViewStates.Visible;
            }
            else if (mCurrentPosition == mCustomresCount - 1)
            {
                mPossDown.Visibility = ViewStates.Invisible;
            }
            
        }

        private void MEdit_Click(object sender, EventArgs e)
        {
            if (OnEditCustomerComplete != null)
            {
                OnEditCustomerComplete.Invoke
               (this, new OnEditCustomerEventArgs(mIsNewCharge, mIsLateCharge, mIsReport, mCurrentPosition));
            }

            this.Dismiss();
        }

        private void MCancel_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void MNewCharge_Click(object sender, EventArgs e)
        {
            if (mNewCharge.Checked)
            {
                mIsNewCharge = true;
            }
            else
            {
                mIsNewCharge = false;
            }
        }

        private void MLateBill_Click(object sender, EventArgs e)
        {
            if (mLateCharge.Checked)
            {
                mIsLateCharge = true;
            }
            else
            {
                mIsLateCharge = false;
            }
        }

        private void MReport_Click(object sender, EventArgs e)
        {
            if (mReport.Checked)
            {
                mIsReport = true;
            }
            else
            {
                mIsReport = false;
            }
        }
    }
  }
