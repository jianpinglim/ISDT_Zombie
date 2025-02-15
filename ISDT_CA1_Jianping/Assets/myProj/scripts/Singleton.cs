using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    #region Variables
    private static T instance;
    #endregion

    #region Getter
    public static T Instance
    {
        get { return instance; }
    }
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        // Check for any instance of the Singleton
        if (instance != null && instance != this)
        {
            Debug.LogWarning("[Singleton] Trying to instantiate a second instance of Singleton class");
            Destroy(gameObject); // Destroy GameObject with this Singleton Script
            return; // Ensure that the method exits here
        }
        instance = (T)this;
        DontDestroyOnLoad(gameObject); // Optional: Keep the instance across scene loads
    }

    protected virtual void OnDestroy()
    {
        // Check if destroyed object is this
        if (instance == this)
        {
            instance = null;
        }
    }
    #endregion
}

public class PersistentXROrigin : Singleton<PersistentXROrigin>
{
    protected override void Awake()
    {
        base.Awake(); // Ensure the Singleton base behavior is applied
    }
}

