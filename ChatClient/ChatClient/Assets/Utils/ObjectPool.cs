using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


public class ObjectPool : LocalSingletonBehaviour<ObjectPool> {
	
	Dictionary<int, List<GameObject>> freeItems = new Dictionary<int, List<GameObject>>();
	Dictionary<GameObject, int> usedItems = new Dictionary<GameObject, int>();
	
	[SerializeField]
	private GameObject[] prefabs;
	
	
	//--------------------------------------------------------------------------------
	public override void DoAwake () {
		base.DoAwake();
		
		for(int i = 0; i < prefabs.Length; i++){
			freeItems.Add(i, new List<GameObject>() );
		}
	}
	
	//--------------------------------------------------------------------------------
	
	private void AddItem(int type) {
		GameObject item = null;
		int numPrefab = (int)type;
		if ( numPrefab < prefabs.Length && prefabs[numPrefab] != null ) {
			item = Instantiate( prefabs[numPrefab] ) as GameObject;
			item.transform.SetParent( transform, false);
			item.gameObject.SetActive( false );
			freeItems[type].Add( item );
		}
	}

    private void AddItem(int type, Transform parent)
    {
        GameObject item = null;
        int numPrefab = (int)type;
        if (numPrefab < prefabs.Length && prefabs[numPrefab] != null)
        {
            item = Instantiate(prefabs[numPrefab]) as GameObject;
            item.transform.SetParent(parent, false);
            item.gameObject.SetActive(false);
            freeItems[type].Add(item);
        }
    }

    //--------------------------------------------------------------------------------
    public GameObject GetItem(int type) {
		GameObject item = null;
		int numPrefab = (int)type;
		if ( numPrefab < prefabs.Length && freeItems.ContainsKey( type ) ) {
			if ( freeItems[type].Count == 0 )
				AddItem(type);
			
			if ( freeItems[type].Count > 0 ) {
				item = freeItems[type][0];
				usedItems.Add( item, type );
				freeItems[type].RemoveAt( 0 );
			}
		}
		
		if ( item != null ) {
			item.SetActive( true );
			// item.GetComponent<int>().OnAddToScene();
		}

		return item;
	}

    public GameObject GetItem(int type, Transform Parent) {
        GameObject item = null;
        int numPrefab = (int)type;
        if (numPrefab < prefabs.Length && freeItems.ContainsKey(type)) {
            if (freeItems[type].Count == 0)
                AddItem(type, Parent);

            if (freeItems[type].Count > 0) {
                item = freeItems[type][0];
                usedItems.Add(item, type);
                freeItems[type].RemoveAt(0);
            }
        }

        if (item != null)
        {
            item.SetActive(true);
            item.transform.SetParent(Parent);
        }

        return item;
    }

    //--------------------------------------------------------------------------------
    public void FreeItem( GameObject item ) {
		if ( item == null )
			return;
		
		if ( usedItems.ContainsKey( item ) ) {
			int type = usedItems[item];
			// item.GetComponent<int>().OnRemoveFromScene();
			item.transform.SetParent( gameObject.transform, true );
			item.SetActive( false );
			freeItems[type].Add( item );
			usedItems.Remove( item );
		}
	}

    public void FreeItem(GameObject item, Transform Parent)
    {
        if (item == null)
            return;

        if (usedItems.ContainsKey(item))
        {
            int type = usedItems[item];
            // item.GetComponent<int>().OnRemoveFromScene();
            item.transform.SetParent(Parent, true);
            item.SetActive(false);
            freeItems[type].Add(item);
            usedItems.Remove(item);
        }
    }

    //--------------------------------------------------------------------------------
    public void FreeAll(int type) {
		
		if ( usedItems.ContainsValue( type ) ) {
			
			List<GameObject> delObjs = new List<GameObject>();
			
			foreach( var item in usedItems.Where(x => x.Value == type) ) {
				delObjs.Add(item.Key);
			}
			
			while(delObjs.Count > 0)
			{
				FreeItem(delObjs[0]);
				delObjs.RemoveAt(0);
			}
		}
	}
	
	//--------------------------------------------------------------------------------
	public void FreeAll() {
		
		foreach( GameObject item in usedItems.Keys ) {
			
			int type = usedItems[item];
			// item.GetComponent<int>().OnRemoveFromScene();
			item.SetActive( false );
			item.transform.parent = gameObject.transform;
			freeItems[type].Add( item );
		}
		
		usedItems.Clear();
	}
}