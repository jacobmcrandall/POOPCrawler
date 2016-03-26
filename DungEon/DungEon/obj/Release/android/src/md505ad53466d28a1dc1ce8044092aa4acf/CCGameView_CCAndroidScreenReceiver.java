package md505ad53466d28a1dc1ce8044092aa4acf;


public class CCGameView_CCAndroidScreenReceiver
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("CocosSharp.CCGameView+CCAndroidScreenReceiver, CocosSharp, Version=1.7.1.0, Culture=neutral, PublicKeyToken=null", CCGameView_CCAndroidScreenReceiver.class, __md_methods);
	}


	public CCGameView_CCAndroidScreenReceiver () throws java.lang.Throwable
	{
		super ();
		if (getClass () == CCGameView_CCAndroidScreenReceiver.class)
			mono.android.TypeManager.Activate ("CocosSharp.CCGameView+CCAndroidScreenReceiver, CocosSharp, Version=1.7.1.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public CCGameView_CCAndroidScreenReceiver (md505ad53466d28a1dc1ce8044092aa4acf.CCGameView p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == CCGameView_CCAndroidScreenReceiver.class)
			mono.android.TypeManager.Activate ("CocosSharp.CCGameView+CCAndroidScreenReceiver, CocosSharp, Version=1.7.1.0, Culture=neutral, PublicKeyToken=null", "CocosSharp.CCGameView, CocosSharp, Version=1.7.1.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

	java.util.ArrayList refList;
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
