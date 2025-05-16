using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        // create the instance
        if (instance == null) instance = this as T;
        else Destroy(this);
    }
}
