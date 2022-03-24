using UnityEngine;

public class CrabSpawning : MonoBehaviour
{
    //================== Components 
    private Transform _respawn;         //Current location where the player will respawn if they die
    private GameFlow _gameFlowScript;   //Reference to the gameFlow script that will take away player control during respawn 
    private Animator _animator;         //Controls the player's animations 
    
    //================ Audio 
    private AudioSource _death;
    private AudioSource _checkpoint;
    
    //==================================================================================================================
    // Base Functions 
    //==================================================================================================================

    //Connects components to necessary game objects 
    private void Start()
    {
        _respawn = GameObject.Find($"Respawn").transform;
        _animator = transform.GetComponent<Animator>();
        _gameFlowScript = GameObject.Find("GameFlow").GetComponent<GameFlow>();

        _death = transform.Find($"Audio").transform.Find($"Death").GetComponent<AudioSource>();
        _checkpoint = transform.Find($"Audio").transform.Find($"Checkpoint").GetComponent<AudioSource>();
    }

    //==================================================================================================================
    // Respawn
    //==================================================================================================================

    //Checks if player hit a death hit box or walked onto a new save point 
    private void OnTriggerEnter(Collider hitBox)
    {
        //Checks if the player died, if so tell Game Flow to stop player from acting 
        if (hitBox.CompareTag($"Death"))
        {
            _gameFlowScript.PlayerColliderUpdate(false);
            _death.Play();
            _animator.Play("MonsterArmature|Death");
            StartCoroutine(_gameFlowScript.Death());
        }
        //If player touched a new save spot save that as new location 
        else if (hitBox.CompareTag($"Checkpoint"))
        {
            var distance = Vector3.Distance(hitBox.transform.position, transform.position);
            if (!(distance < 0.8f)) return;
            _checkpoint.Play();
            _respawn = hitBox.transform;
        }
    }

    //Moves the player to the respawn point 
    public void MoveToRespawn()
    {
        transform.position = _respawn.position;
        transform.position += Vector3.up / 4f;
    }
}
