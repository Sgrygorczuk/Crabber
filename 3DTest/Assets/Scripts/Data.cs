using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static Data Instance;
    public bool[] _fruitCollectd = new[] { false, false, false, false, false };

    private void Awake()
    {
        //Checks if there already exits a copy of this, if does destroy it and let the new one be created 
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CollectFruit(int pos)
    {
        _fruitCollectd[pos] = true;
    }

    public bool[] GetFruitCollected()
    {
        return _fruitCollectd;
    }
}
