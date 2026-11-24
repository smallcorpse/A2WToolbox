using UnityEngine;

namespace A2W
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();
        public static T instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton)" + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);
                        }
                    }

                    return _instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;

        // 修复：原来用 OnDestroy 置位，导致每次退出 Play 模式后所有单例 getter
        // 永久返回 null（直到域重载），第二次 Play 必炸。
        // 注意：编辑器内退出 Play 也会回调 OnApplicationQuit，因此编辑器环境直接不置位；
        // 真机/打包运行仍在应用退出时置位（防止 quit 后 getter 再创建新对象）。
        public void OnApplicationQuit()
        {
#if UNITY_EDITOR
            return;
#else
            applicationIsQuitting = true;
#endif
        }
    }
}

