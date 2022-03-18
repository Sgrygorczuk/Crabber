using UnityEngine;

public class FruitCollector : MonoBehaviour
{
    public Material appleCoreMaterial;
    public Material appleLeafMaterial;
    private MeshRenderer _appleMeshRenderer;
    private Material[] _materials;
    [HideInInspector] public bool _collected = false;

    private void Start()
    {
        _materials = new[] { appleCoreMaterial, appleLeafMaterial };
        _appleMeshRenderer = GameObject.Find($"Top Down Camera").transform.Find($"Camera").transform.Find($"Fruit Collectible").GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider hitBox)
    {
        if (!hitBox.CompareTag($"Crab"))
        {
            
        }
        
        if (!hitBox.CompareTag($"Fruit")) return;
        var dist = Vector3.Distance(hitBox.transform.position, transform.position);
        if (!(dist < 1)) return;
        _appleMeshRenderer.materials = _materials;
        _collected = true;
        Destroy(hitBox.gameObject);
    }
}
