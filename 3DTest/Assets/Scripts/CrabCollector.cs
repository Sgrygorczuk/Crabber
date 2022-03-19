using UnityEngine;

public class CrabCollector : MonoBehaviour
{
    //============= Components 
    private MeshRenderer _appleMeshRenderer;      //The display apple that will have it's mesh rendered update 
    private Material[] _materials;                //The materials that the apple will receive 
    private GameFlow _gameFlowScript;             //Game Flow to update when player touched a crab 
    
    //==================================================================================================================
    // Base Functions 
    //==================================================================================================================

    //Connects the components to 
    private void Start()
    {
        _materials = GameObject.Find($"Collectibles").transform.Find($"Fruit").GetComponent<MeshRenderer>().materials;
        _gameFlowScript = GameObject.Find($"GameFlow").GetComponent<GameFlow>();
        _appleMeshRenderer = GameObject.Find($"Top Down Camera").transform.Find($"Camera").transform.Find($"Fruit Collectible").GetComponent<MeshRenderer>();
    }

    //==================================================================================================================
    // Trigger
    //==================================================================================================================
    private void OnTriggerEnter(Collider hitBox)
    {
        //If we walk into a crab start the win process 
        if (hitBox.CompareTag($"Crab"))
        {
            var dist = Vector3.Distance(hitBox.transform.position, transform.position);
            if (!(dist < 1f)) return;
            StartCoroutine(_gameFlowScript.Win());
        }
        
        //If we walk into the fruit update the UI materials and destroy the level fruit 
        if (!hitBox.CompareTag($"Fruit")) return;
        var distance = Vector3.Distance(hitBox.transform.position, transform.position);
        if (!(distance < 1)) return;
        _appleMeshRenderer.materials = _materials;
        Destroy(hitBox.gameObject);
    }
    
}
