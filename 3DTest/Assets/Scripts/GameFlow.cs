using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    
    //======================= Game States
    //Defines if the game is performing automatic action or player is in control 
    private enum GameState
    {
        FadingIn,   //Player Has No Control 
        Player,     //Is Turing to a different side 
        Death,      //Is Jumping 
        Win,    //Is walking forward 
    }
    private GameState _currentGame = GameState.FadingIn;     //Current state of the game 

    private CrabMovement _crabMovementScript;
    private CrabSpawning _crabSpawningScript;
    private Rigidbody _crabRigidbody;
    private Animator _fadeCanvasAnimator;
    private Animator _winAnimator;
    private Animator _playerAnimator;
    private Animator _crabAnimator;
    
    // Start is called before the first frame update
    private void Start()
    {
        _crabMovementScript = GameObject.Find($"PlayerCrab").GetComponent<CrabMovement>();
        _crabSpawningScript = GameObject.Find($"PlayerCrab").GetComponent<CrabSpawning>();
        _crabRigidbody = GameObject.Find($"PlayerCrab").GetComponent<Rigidbody>();
        _fadeCanvasAnimator = GameObject.Find($"FadeCanvas").GetComponent<Animator>();

        GameObject.Find($"Top Down Camera").transform.Find($"CM vcam1").GetComponent<CinemachineVirtualCamera>().m_Lens
            .NearClipPlane = -100;
        
        _crabMovementScript.enabled = false;
        StartCoroutine(ToPlayer());
    }

    // Update is called once per frame
    private void Update()
    {
        switch (_currentGame)
        {
            case GameState.FadingIn:
            {
                StartCoroutine(ToPlayer());
                break;
            }
            case GameState.Player:
            {
                
                break;
            }
            case GameState.Death:
            {
                
                break;
            }
        }
    }

    private IEnumerator ToPlayer()
    {
        yield return new WaitForSeconds(2);
        _crabMovementScript.enabled = true;
        _currentGame = GameState.Player;
    }

    public IEnumerator Death()
    {
        _crabRigidbody.useGravity = false;
        _crabRigidbody.velocity = Vector3.zero;
        _crabMovementScript.GoIdle();
        _crabMovementScript.enabled = false;
        _fadeCanvasAnimator.Play($"FadeCanvasInAndOut");
        yield return new WaitForSeconds(1f);
        _crabMovementScript.ResetOnDeath();
        _crabSpawningScript.MoveToRespawn();
        yield return new WaitForSeconds(1.1f);
        _crabRigidbody.useGravity = true;
        _crabMovementScript.enabled = true;
        _currentGame = GameState.Player;
    }

    private IEnumerable Win()
    {
        _crabMovementScript.GoIdle();
        _crabMovementScript.enabled = false;
        yield return new WaitForSeconds(1f);
    }
}
