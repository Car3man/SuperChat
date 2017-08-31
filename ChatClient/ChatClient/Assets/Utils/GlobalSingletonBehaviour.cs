using UnityEngine;
using System.Collections;

public class GlobalSingletonBehaviour<T> : MonoBehaviour where T : GlobalSingletonBehaviour<T>
{
	private static bool _destroyed;
	private static T _instance;
	protected bool awaken;
	
	//---------------------------------------------------
	public static T Instance {
		get	{ return GlobalSingletonBehaviour<T>._instance != null || _destroyed ? GlobalSingletonBehaviour<T>._instance : GlobalSingletonBehaviour<T>.Load(); }
		set { GlobalSingletonBehaviour<T>._instance = value; }
	}
	
	//---------------------------------------------------
	public static T I {
		get { return Instance; }
	}
	
	//---------------------------------------------------
	public static void Init() {
		if ( GlobalSingletonBehaviour<T>._instance == null )
			GlobalSingletonBehaviour<T>.Load();
	}
	
	//---------------------------------------------------
	private static T Load() {
		var inst = (T)FindObjectOfType(typeof(T));
		if ( inst == null ) {
			var obj = new GameObject(typeof(T).Name);
			inst = obj.AddComponent<T>();
		}

		inst.Awake();
		return inst;
	}
	
	//---------------------------------------------------
	public virtual void Awake()
	{
		if (this.awaken)
			return;

		this.awaken = true;
	
		if ( GlobalSingletonBehaviour<T>._instance != null && GlobalSingletonBehaviour<T>._instance != this ) {
			Object.Destroy(this.gameObject);
			return;
		}
	
		GlobalSingletonBehaviour<T>._instance = (T)this;
		Object.DontDestroyOnLoad(this.gameObject);
		this.DoAwake();
	}
	
	//---------------------------------------------------
	public void OnDestroy() {
		if ( GlobalSingletonBehaviour<T>._instance == this ) {
			_destroyed = true;
			this.DoDestroy();
		}
	}
	
	//---------------------------------------------------
	public virtual void DoAwake() {
	}
	
	//---------------------------------------------------
	public virtual void DoDestroy() {
	}
}
