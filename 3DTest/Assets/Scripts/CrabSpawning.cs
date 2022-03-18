using UnityEngine;

public class CrabSpawning : MonoBehaviour
{
    private Transform _respawn;
    private CapsuleCollider _capsuleCollider;
    private GameFlow _gameFlowScript;
    private Animator _animator;
    
    // Start is called before the first frame update
    private void Start()
    {
        _respawn = GameObject.Find($"Respawn").transform;
        _animator = transform.GetComponent<Animator>();
        _capsuleCollider = transform.GetComponent<CapsuleCollider>();
        _gameFlowScript = GameObject.Find("GameFlow").GetComponent<GameFlow>();
    }

    private void OnTriggerEnter(Collider hitBox)
    {
        if (hitBox.CompareTag($"Death"))
        {
            _animator.Play("MonsterArmature|Death");
            StartCoroutine(_gameFlowScript.Death());
        }
        else if (hitBox.CompareTag($"Checkpoint"))
        {
            _respawn = hitBox.transform;
        }
    }

    public void MoveToRespawn()
    {
        transform.position = _respawn.position;
    }
}
