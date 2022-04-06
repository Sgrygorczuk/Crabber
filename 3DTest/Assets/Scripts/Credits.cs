using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    private bool _loaded;

    private void Awake()
    {
        StartCoroutine(PreLoad());
    }

    // Update is called once per frame
    public void Update()
    {
        if (!Input.GetButtonDown("Jump") || _loaded) return;
        StartCoroutine(Load());
    }

    private IEnumerator PreLoad()
    {
        yield return new WaitForSeconds(15.1f);
        if (_loaded) yield break;
        StartCoroutine(Load());
    }
    
    private IEnumerator Load()
    {   
        _loaded = true;
        GameObject.Find($"FadeCanvas").GetComponent<Animator>().Play("FadeCanvasIn");
        yield return new WaitForSeconds(2.1f);
        
        SceneManager.LoadScene("MainMenu");
    }
}
