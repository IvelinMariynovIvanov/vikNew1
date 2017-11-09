package md5f4553d98607c95a2d2dd0bda4a12543a;


public class MyJobService
	extends android.app.job.JobService
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onStopJob:(Landroid/app/job/JobParameters;)Z:GetOnStopJob_Landroid_app_job_JobParameters_Handler\n" +
			"n_onStartJob:(Landroid/app/job/JobParameters;)Z:GetOnStartJob_Landroid_app_job_JobParameters_Handler\n" +
			"";
		mono.android.Runtime.register ("ListViewTask.MyJobService, ListViewTask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MyJobService.class, __md_methods);
	}


	public MyJobService () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MyJobService.class)
			mono.android.TypeManager.Activate ("ListViewTask.MyJobService, ListViewTask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public boolean onStopJob (android.app.job.JobParameters p0)
	{
		return n_onStopJob (p0);
	}

	private native boolean n_onStopJob (android.app.job.JobParameters p0);


	public boolean onStartJob (android.app.job.JobParameters p0)
	{
		return n_onStartJob (p0);
	}

	private native boolean n_onStartJob (android.app.job.JobParameters p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
