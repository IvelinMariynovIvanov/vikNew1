package md5f4553d98607c95a2d2dd0bda4a12543a;


public class MyNotification
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("ListViewTask.MyNotification, ListViewTask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MyNotification.class, __md_methods);
	}


	public MyNotification () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MyNotification.class)
			mono.android.TypeManager.Activate ("ListViewTask.MyNotification, ListViewTask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

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
