using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFlow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Player jumps forward 
        if (Input.GetButtonDown("Jump"))
        {
            SceneManager.LoadScene("Level_One");
        }
    }
}
