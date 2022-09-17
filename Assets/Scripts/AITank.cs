using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AITank : MonoBehaviour
{
    public int _playerNum;
    public float m_Speed = 10f;                 // How fast the tank moves forward and back.
    public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
    public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

    public AudioSource _tankSFX;
    public AudioClip _clipTankExplode;
    public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
    public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.

    private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
    private string m_TurnAxisName;              // The name of the input axis for turning.
    private Rigidbody m_Rigidbody;              // Reference used to move the tank.
    private float m_MovementInputValue;         // The current value of the movement input.
    private float m_TurnInputValue;             // The current value of the turn input.
    private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.

    public ParticleSystem _particleExplode;

    //delegate
    public delegate void TankDestroyed(AITank target);  // Function pointer that is cascader/implemented in another script
    public TankDestroyed dTankDestroyed;

    public GameObject[] players;
    public GameObject player;
    public NavMeshAgent agent;

    float shootDistance = 200.0f;
    float shootAngle = 20.0f;

    float shootingRechargeTime = 0;
    public float reloadTime = 1.0f;

    float deathTimer = 0;
    public float deathReviveTime = 2.0f;

    public float agent_speed = 10f;
    
    public float _maxHealth = 100;
    public float mHealth;

    private AITankFiringSystem aiShooting;

    public enum State
    {
        Idle = 0,
        Moving,
        TakingDamage,
        Death,
        Inactive
    };
    protected State mState;

    private void Awake ()
    {
        m_Rigidbody = GetComponent<Rigidbody> ();

        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        aiShooting = agent.GetComponent<AITankFiringSystem> ();
        
        mHealth = _maxHealth;
    }

    private void OnEnable ()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    private void OnDisable ()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }


    private void Start ()
    {
        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        aiShooting = agent.GetComponent<AITankFiringSystem> ();

        players = GameObject.FindGameObjectsWithTag("Player");
        TargetNearestPlayer();

        Debug.Log("Targetting player object", player);

        m_MovementAxisName = "AIVertical" + _playerNum;
        m_TurnAxisName = "AIHorizontal" + _playerNum;
    }

    private void TargetNearestPlayer()
    {
        int numPlayers = players.Length;

        double distance0 = (players[0].transform.position - agent.transform.position).magnitude;
        player = players[0];

        for (int i = 1; i < numPlayers; i++){
            double distance1 = (players[i].transform.position - agent.transform.position).magnitude;
            if (distance1 < distance0){
                player = players[i];
                distance0 = distance1;
            }
        }
    }


    private void Update ()
    {
        agent.speed = agent_speed;

        switch (state)
        {
            case State.Idle:
            case State.Moving:
                // move towards player
                TargetNearestPlayer();
                agent.SetDestination(player.transform.position);
                var rotationAngle = Quaternion.LookRotation ( player.transform.position - transform.position); // we get the angle has to be rotated
                int damp = 5;
                transform.rotation = Quaternion.Slerp ( transform.rotation, rotationAngle, Time.deltaTime * damp);

                if (shootingRechargeTime < reloadTime) {
                    shootingRechargeTime += Time.deltaTime;
                }
                if (shootingRechargeTime >= reloadTime && canFire()) {
                    Debug.Log("Firing at player object", player);
                    aiShooting.Fire();
                    shootingRechargeTime = 0;
                }
                break;
            case State.TakingDamage:
                break;
            case State.Inactive:
            case State.Death:
                if (deathTimer <= deathReviveTime) {
                    deathTimer += Time.deltaTime;
                }
                else {
                    Revive();
                }
                break;
            default:
                break;
        }
    }

    public bool canFire() {
        Vector3 direction = player.transform.position - agent.transform.position;
        float angle = Vector3.Angle(direction, agent.transform.forward);

        if (direction.magnitude < shootDistance && angle < shootAngle) {
            return true;
        }
        return false;
    }


    private void FixedUpdate ()
    {
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        if (state == State.Moving)
            Move ();
            Turn ();
    }


    private void Move ()
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn ()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
    }
    
    public void TakeDamage(float damage)
    {
        if (mState != State.Death)
        {
            mHealth -= damage;
            if (mHealth > 0)
                state = State.TakingDamage;
            else
                state = State.Death;
                _particleExplode.Play();
        }
    }

    public void Death()
    {
        PlaySFX(_clipTankExplode);
        StartCoroutine(ChangeState(State.Inactive, 1f));
    }

    public void Restart(Vector3 pos, Quaternion rot)
    {
        // Reset position, rotation
        transform.position = pos;
        transform.rotation = rot;
        m_Rigidbody.MovePosition(transform.position);
        m_Rigidbody.MoveRotation(transform.rotation);
        
        // Diable kinematic and activate the gameobject and input
        m_Rigidbody.isKinematic = false;
        gameObject.SetActive(true);
    }
    
    public void Revive()
    {
        // Restore health
        mHealth = _maxHealth;

        // Change state
        state = State.Idle;
    }

    private IEnumerator ChangeState(State state, float delay)
    {
        // Delay
        yield return new WaitForSeconds(delay);

        // Change state
        this.state = state;
    }

    public State state
    {
        get { return mState; }
        set
        {
            if (mState != value)
            {
                switch (value)
                {
                    case State.Idle:
                        break;

                    case State.Moving:
                        break;

                    case State.TakingDamage:
                        StartCoroutine(ChangeState(State.Idle, 0.1f));
                        break;

                    case State.Death:
                        Death();
                        break;

                    case State.Inactive:
                        gameObject.SetActive(false);
                        dTankDestroyed.Invoke(this);
                        m_Rigidbody.isKinematic = true;
                        break;
                    default:
                        break;
                }

                mState = value;
            }
        }
    }
    protected void PlaySFX(AudioClip clip)
    {
        _tankSFX.clip = clip;
        _tankSFX.pitch = m_OriginalPitch + Random.Range(-m_PitchRange, m_PitchRange);
        _tankSFX.Play();
    }
}