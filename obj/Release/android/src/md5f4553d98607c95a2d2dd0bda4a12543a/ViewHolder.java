package md5f4553d98607c95a2d2dd0bda4a12543a;


public class ViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("ListViewTask.ViewHolder, ListViewTask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ViewHolder.class, __md_methods);
	}


	public ViewHolder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ViewHolder.class)
			mono.android.TypeManager.Activate ("ListViewTask.ViewHolder, ListViewTask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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
