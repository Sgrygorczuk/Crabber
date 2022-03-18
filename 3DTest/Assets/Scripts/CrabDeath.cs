using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabDeath : MonoBehaviour
{
    private Transform _respawn;
    
    // Start is called before the first frame update
    void Start()
    {
        _respawn = GameObject.Find($"Respawn").transform;
    }

    private void OnTriggerEnter(Collider hitBox)
    {
        if (hitBox.CompareTag($"Death"))
        {
            Debug.Log("Die");
            transform.position = _respawn.position;
        }
    }
}
