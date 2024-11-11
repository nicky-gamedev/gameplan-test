using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static bool Initialized { get { return Instance != null; } }
    private static object _lock = new object();
    [SerializeField]
    protected bool _destroyOnLoad = true;
    private static bool _onDestroyedExecuted = false;

    protected static T _instance;
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    if (!_onDestroyedExecuted)
                    {
                        TryLoad();
                    }
                }

                return _instance;
            }
        }
    }


    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (_instance != null && _instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }
            if (!_destroyOnLoad)
                DontDestroyOnLoad(this.gameObject);
            _instance = this.GetComponent<T>();
        }
    }

    protected virtual void OnDestroy()
    {
        lock (_lock)
        {
            _onDestroyedExecuted = true;
        }
    }

    protected virtual void OnInitialize() { }

    private static void TryLoad()
    {
        _instance = (T)FindObjectOfType(typeof(T));
        if (_instance != null)
        {
            Debug.Log("Loaded " + typeof(T) + " from Scene");
        }
        else
        {
            var o = (T)Resources.Load("Singleton/" + typeof(T).Name, typeof(T));
            if (o != null)
            {
                _instance = Instantiate(o.gameObject).GetComponent<T>();
                _instance.gameObject.name = typeof(T).ToString();
                Debug.Log("Loaded " + typeof(T) + " from Resource");
            }
        }
        if (_instance == null)
        {
            Debug.LogWarning("Cannot Find Object of Type " + typeof(T) + " in Scene or in ResourcesPath");
            return;
        }

        if (_instance is Singleton<T> castedInstance)
        {
            castedInstance.OnInitialize();
        }

        _instance.gameObject.SetActive(true);
        if (!(_instance as Singleton<T>)._destroyOnLoad)
        {
            DontDestroyOnLoad(_instance.gameObject);
        }
    }
}
