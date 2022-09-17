using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public float _moveSpeed = 15f;

    public LayerMask _explosionMask;
    public ParticleSystem _explosionParticles;         // Reference to the particles that will play on explosion.
    public float _explosionRadius = 5f;
    public float _explosionForce = 1000f;
    public float _maxDamage = 100f;
    public float _maxLifeTime = 4f;                    // The time in seconds before the shell is removed.

    public enum State
    {
        Moving,
        Explode
    }
    protected State mState;
    protected Rigidbody mRigidBody;

    // Start is called before the first frame update
    void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when collide with other collider
    protected void OnTriggerEnter(Collider other)
    {
        if(state == State.Moving)
            state = State.Explode;
    }

    private void FixedUpdate()
    {
        switch (mState)
        {
            case State.Moving:
                Move();
                break;

            case State.Explode:
                break;
            default:
                break;
        }
    }

    public void Move()
    {
        Vector3 moveVect = transform.forward * _moveSpeed * Time.deltaTime;
        mRigidBody.MovePosition(mRigidBody.position + moveVect);
    }

    public void Explosion()
    {
        // Get all the tanks caught in the explosion
        Collider[] tanksCollider = Physics.OverlapSphere(transform.position, _explosionRadius, _explosionMask);
        // Unparent the particles from the shell.
        _explosionParticles.transform.parent = null;

        // Play the particle system.
        _explosionParticles.Play();

        // Loop through the collider to apply force and damage
        foreach( Collider collider in tanksCollider)
        {
            // Apply physics to the tank
            Rigidbody trbody = collider.GetComponent<Rigidbody>();
            if (trbody == null)
                continue;
            trbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);

            if (collider.gameObject.CompareTag("Player")){
                // Apply effects to tanks
                Tank tank = collider.GetComponent<Tank>();
                tank.TakeDamage( CalculateDamage(tank.transform.position) );
            }
            else if (collider.gameObject.CompareTag("Enemy")){
                // Apply effects to tanks
                Debug.Log("Enemy hit!");
                AITank tank = collider.GetComponent<AITank>();
                tank.TakeDamage( CalculateDamage(tank.transform.position) );
            }
        }
        
        Destroy(gameObject);
    }

    public float CalculateDamage(Vector3 targetPos)
    {
        // Create a vector from the shell to the target
        Vector3 explosionToTarget = targetPos - transform.position;

        // Get the distance between shell and the target
        float distance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away
        float relativeDistance = (_explosionRadius - distance) / _explosionRadius;

        // Damage is proportional to the distance
        float damage = relativeDistance * _maxDamage;

        return Mathf.Max(0, damage);
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
                    case State.Moving:
                        break;

                    case State.Explode:
                        Explosion();
                        break;

                    default:
                        break;
                }

                mState = value;
            }
        }
    }
}
