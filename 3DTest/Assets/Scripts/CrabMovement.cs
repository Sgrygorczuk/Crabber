using System;
using System.Collections;
using UnityEngine;

public class CrabMovement : MonoBehaviour
{
    //======================= Player Direction Facing 
    //Defines the four directions the player can be facing, it's used to move around 
    private enum Facing             
    {
        Up,      
        Left,
        Right,   
        Down,           
    }
    private Facing _currentlyFacing = Facing.Up;     //The direction the player is currently facing 
    
    //======================= Player Movement State  
    //Defines the ways the player can be acting 
    private enum MovementState
    {
        Idle,       //Stands idling 
        Turning,    //Is Turing to a different side 
        Jumping,    //Is Jumping 
        Walking,    //Is walking forward 
    }
    private MovementState _currentMovement = MovementState.Idle;     //Current state the player is in 

    //======================= Obstacle Detection 
    private bool _walkIsBlocked;     //Tells us if there is an obstacle in front of the player 
    private bool _jumpIsBlocked;     //Tells us theres an obstacle 2 spaces from placyer 
    
    //======================= Turning Around 
    private bool _turnDirection;        //Determines if the player will turn left or right
    private float _turnGoal;            //Tells us what angle the player is spinning to 

    //======================= Moving Froward 
    private const float SpeedVar = 0.05f;       //How fast the player moves forward 
    private float _moveDistance;                //How far the player has to move 2 [Walk] and 4 [Jump]
    private bool _positiveMovement;             //Is the player moving along the positive or negative  x/z axis
    private Vector3 _originalPosition;          //Where the player started, to reset after player lands on their position 
    public float jumpVelocity = 6.5f;           //How high the player jumps 
    private ParticleSystem _dustParticleSystem;

    //======================= Components 
    private Rigidbody _rigidbody;               //Used to set the player to jump
    private Animator _animator;                 //Used to switch between different animations
    
    //==================================================================================================================
    // Base Functions 
    //==================================================================================================================
    
    //Initializes components 
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _dustParticleSystem = transform.Find($"DustSmoke").transform.Find("Particle System")
            .GetComponent<ParticleSystem>();
        _originalPosition = transform.position;
    }

    //Depending on state listens for player input or moves the character 
    public void Update()
    {
        switch (_currentMovement)
        {
            case MovementState.Idle:
            {
                PlayerInput();
                break;
            }
            case MovementState.Turning:
            {
                Turn();
                break;
            }
            case MovementState.Walking:
            {
                MoveForward(2f);
                break;
            }
            case MovementState.Jumping:
            {
                MoveForward(4f);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    //==================================================================================================================
    // Player Input  
    //==================================================================================================================

    //Listens for player input 
    private void PlayerInput()
    {
        //Checks which direction the player wants to move to 
        Facing playerAction;
        if (Input.GetButtonDown($"Up"))
        {
            playerAction = Facing.Up;
        }
        else if (Input.GetButtonDown($"Down"))
        {
            playerAction = Facing.Down;
        }
        else if (Input.GetButtonDown($"Right"))
        {
            playerAction = Facing.Right;
        }
        else if (Input.GetButtonDown($"Left"))
        {
            playerAction = Facing.Left;
        }
        //Player jumps forward 
        else if (Input.GetButtonDown("Jump"))
        {
            MoveForwardGoal(false);
            return;
        }
        else
        {
            return;
        }
        
        //If the player is facing the direction they want to move to, move forward 
        if (playerAction == _currentlyFacing)
        {
            MoveForwardGoal(true);
        }
        //Else the player spins to the selected direction 
        else
        {
            ChooseTurnDirection(playerAction);
            _currentlyFacing = playerAction;
            TurnGoal();
        }
    }

    //==================================================================================================================
    // Turning 
    //==================================================================================================================

    //Figures out which way should the player spin, left or right depending which direction is closer 
    private void ChooseTurnDirection(Facing playerAction)
    {
        //Turn the state to numeric angle 
        var player = TuringToNumber(playerAction);
        var current = TuringToNumber(_currentlyFacing);

        //If the distance is 90/-270 same thing turn right else turn left 
        var differance = player - current;
        switch (differance)
        {
            case 90:
            case -270:
                _turnDirection = true;
                break;
            case -90:
            case 270:
                _turnDirection = false;
                break;
        }
    }

    //Converts the Facing States into numeric angles 
    private static int TuringToNumber(Facing facing)
    {
        return facing switch
        {
            Facing.Up => 0,
            Facing.Left => 270,
            Facing.Right => 90,
            Facing.Down => 180,
            _ => throw new ArgumentOutOfRangeException(nameof(facing), facing, null)
        };
    }
    
    //Sets the goal to which the player will turn to and starts the process of turning 
    private void TurnGoal()
    {
        _turnGoal = _currentlyFacing switch
        {
            Facing.Up => 0,
            Facing.Down => 180,
            Facing.Right => 90,
            Facing.Left => 270,
            _ => _turnGoal
        };

        _animator.Play($"MonsterArmature|Walk");
        _currentMovement = MovementState.Turning;
    }

    //Is used by the Update function to move rotate the player in desired direction and when achieved goes back to Idle 
    private void Turn()
    {
        //Updates the position 
        var transformReference = transform;
        transformReference.eulerAngles += new Vector3(0.0f, _turnDirection ? 2.5f : -2.5f, 0.0f);

        //Checks if is done turning 
        if (!(Math.Abs(_turnGoal - transformReference.eulerAngles.y) < 1.5f)) return;
        
        //Set to idle 
        _currentMovement = MovementState.Idle;
        _animator.Play($"MonsterArmature|Idle");
        
        //Readjust in reference to the desired angle 
        transformReference.eulerAngles = _currentlyFacing switch
        {
            Facing.Up => new Vector3(0.0f, 0.0f, 0.0f),
            Facing.Down => new Vector3(0.0f, 180.0f, 0.0f),
            Facing.Right => new Vector3(0.0f, 90.0f, 0.0f),
            Facing.Left => new Vector3(0.0f, 270.0f, 0.0f),
            _ => transformReference.eulerAngles
        };

    }

    //==================================================================================================================
    // Walking and Jumping 
    //==================================================================================================================
    
    //Checks if there is anything blocking the player, if not proceeds to set up movement forward 
    //Setting 2 distance for walking and 4 for jumping and initiate the state and animations 
    private void MoveForwardGoal(bool isWalking)
    {
        //Check for blocks 
        if(isWalking && _walkIsBlocked)
        {
            return;
        }
        
        //Save position for future update 
        _originalPosition = transform.position;
        
        //Check if we're moving along positive or negative axis 
        if (_currentlyFacing == Facing.Up || _currentlyFacing == Facing.Right)
        {
            _moveDistance = isWalking ? 2 : 4;
            _positiveMovement = true;
        }
        else
        {
            _moveDistance = isWalking ? -2 : -4;
            _positiveMovement = false;
        }

        //Initiate either walking or jumping 
        if (isWalking)
        {
            _animator.Play($"MonsterArmature|Walk");   
            _currentMovement = MovementState.Walking;
        }
        else
        {
            StartCoroutine(JumpGoal());
        }
        
    }

    //Used by Update function to move the player forward, once the player reached their destination readjust and 
    //set to idle 
    private void MoveForward(float distance)
    {
        //Moving forward or backwards 
        if (_currentlyFacing == Facing.Up || _currentlyFacing == Facing.Down)
        {
            transform.position += _positiveMovement ? new Vector3(0, 0, SpeedVar) : new Vector3(0, 0, -SpeedVar) ;
        }
        else
        {
            transform.position += _positiveMovement ? new Vector3(SpeedVar, 0, 0) : new Vector3(-SpeedVar, 0, 0);
        }
        
        //Count down till we reach the destination 
        _moveDistance += _positiveMovement ? -SpeedVar : SpeedVar;

        //Check if we're done 
        if ((!(Math.Abs(_moveDistance - 0) < 0.01f))) return;

        //Rest to idle 
        _currentMovement = MovementState.Idle;
        _animator.Play($"MonsterArmature|Idle");
        
        //Readjust in reference to original position 
        var transformReference = transform;
        transformReference.position = _currentlyFacing switch
        {
            Facing.Up => _originalPosition + new Vector3(0.0f, 0.0f, distance),
            Facing.Down => _originalPosition + new Vector3(0.0f, 0.0f, -distance),
            Facing.Right =>_originalPosition + new Vector3(distance, 0.0f, 0.0f),
            Facing.Left =>_originalPosition + new Vector3(-distance, 0.0f, 0.0f),
            _ => transformReference.position
        };
    }

    //Starts the jump and gives player velocity 
    private IEnumerator JumpGoal()
    {
        if (_jumpIsBlocked)
        {
            yield break;
        }
        
        _animator.Play($"MonsterArmature|Jump");
        yield return new WaitForSeconds(0.12f);
        _currentMovement = MovementState.Jumping;
        _rigidbody.velocity = new Vector3(0, jumpVelocity, 0);
        yield return new WaitForSeconds(1.3f);
        _dustParticleSystem.Play();
    }
    
    //==================================================================================================================
    // Death 
    //==================================================================================================================

    //Stops any movement updating after death 
    public void GoIdle()
    {
        _currentMovement = MovementState.Idle;
    }
    
    //Makes the crab face forward after dying 
    public void ResetOnDeath()
    {
        _animator.Play($"MonsterArmature|Idle");
        _currentlyFacing = Facing.Up;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    }

    //==================================================================================================================
    // Triggers 
    //==================================================================================================================
    
    private void OnTriggerEnter(Collider hitBox)
    {
        if (hitBox.CompareTag($"Obstacle"))
        {
            var dist = Vector3.Distance(hitBox.transform.position, transform.position);
            Debug.Log(dist);
            if (dist > 0 && dist < 2.5f)
            {
                _walkIsBlocked = true;
            }
            else if (dist > 2.5f && dist < 4.5f)
            {
                _jumpIsBlocked = true;
            }
        }
    }

    private void OnTriggerStay(Collider hitBox)
    {
        if (hitBox.CompareTag($"Obstacle"))
        {
            var dist = Vector3.Distance(hitBox.transform.position, transform.position);
            if (dist > 0 && dist < 2.5f)
            {
                _walkIsBlocked = true;
                _jumpIsBlocked = false;
            }
            else if (dist > 2.5f && dist < 4.5f)
            {
                _jumpIsBlocked = true;
                _walkIsBlocked = false;
            }
            Debug.Log(dist + " " + _walkIsBlocked);
        }
    }

    private void OnTriggerExit(Collider hitBox)
    {
        if (hitBox.CompareTag($"Obstacle"))
        {
            _walkIsBlocked = false;
            _jumpIsBlocked = false;
        }
    }
}
