using UnityEngine;
using System.Collections;

public class MonoSingleton<T> : MonoBehaviour where T : Component  {
	private static T _instance;
	public static T Instance { get {
			return (_instance);
		}
	}

	[ExecuteInEditMode]
	protected void Awake() {
		if (_instance == null) {
			Init();
			_instance = GetComponent<T>();
		}
		else {
			Debug.LogWarning("An instance of " + _instance.name + " already exists ! GO " + gameObject.name + " will be deleted !");
			Destroy(gameObject);
		}
	}

    protected virtual void OnDestroy() {
        if (_instance == this)
            _instance = null;
    }

	[ExecuteInEditMode]
	protected virtual void Init() { } // Called on Awake, as Awake can't be overloaded
}