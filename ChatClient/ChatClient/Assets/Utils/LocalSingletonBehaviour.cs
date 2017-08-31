using UnityEngine;
using System.Collections;

public class LocalSingletonBehaviour<T> : MonoBehaviour where T : LocalSingletonBehaviour<T> {
	
	private static T _instance;
	protected bool awaken;

	public static bool Inited {
		get { return _instance != null; }
	}
	
	//---------------------------------------------------
	public static T Instance {
		get { return LocalSingletonBehaviour<T>._instance != null ? LocalSingletonBehaviour<T>._instance : LocalSingletonBehaviour<T>.Load (); }
		set { LocalSingletonBehaviour<T>._instance = value; }
	}
	
	//---------------------------------------------------
	public static T I {
		get { return Instance; }
	}
	
	//---------------------------------------------------
	public static void Init () {
		if (LocalSingletonBehaviour<T>._instance == null)
			LocalSingletonBehaviour<T>.Load ();
	}
	
	//---------------------------------------------------
	private static T Load () {
		var instance = (T)FindObjectOfType (typeof(T));

		if (instance == null) {
			var obj = new GameObject (typeof(T).Name);
			instance = obj.AddComponent<T> ();
		}

		instance.Awake ();
		return instance;
	}
	
	//---------------------------------------------------
	protected virtual void Awake () {
		if (this.awaken)
			return;

		this.awaken = true;
	
		if (LocalSingletonBehaviour<T>._instance != null && LocalSingletonBehaviour<T>._instance != this) {
			Object.Destroy (this.gameObject);
			return;
		}
	
		LocalSingletonBehaviour<T>._instance = (T)this;
		this.DoAwake ();
	}
	
	//---------------------------------------------------
	public virtual void OnDestroy () {
		if (LocalSingletonBehaviour<T>._instance == this) {
			LocalSingletonBehaviour<T>._instance = null;
			this.DoDestroy ();
		}
	}
	
	//---------------------------------------------------
	public virtual void DoAwake () {
	}
	
	//---------------------------------------------------
	public virtual void DoDestroy () {
	}
}