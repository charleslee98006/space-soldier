﻿using UnityEngine;
using System.Collections;

public class FootSoldierAI : EnemyAI {

    public int firingDistance;
    public int attackingDistance;
    public float timeBetweenMoves;
    public int bounceVariationDegrees;
    public int movementVariationTime;
    public int shotsFiredPerMovement;
    public float attackDuration;

    private Rigidbody2D rb2d;
    private float nextMoveTime = 0;
    private int shotsFiredThisMovement = 0;
    private BasicEnemyFire enemyFireScript;
    private Wander wanderScript;
    private int numMovementAttempts;
    private Vector2 previousVelocity = Vector2.zero;
    private Vector2 colliderSize;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        enemyFireScript = GetComponent<BasicEnemyFire>();
        wanderScript = GetComponent<Wander>();
        colliderSize = GetComponent<BoxCollider2D>().size;
    }

    void Update()
    {
        if (!LoadLevel.WALL_COLLIDERS_INITIALIZED)
        {
            return;
        }

        ChaseIfNecessary();

        if (EnemyUtil.CanSee(transform.position, Player.PlayerTransform.position))
        {
            CancelInvoke("DeactivateAttack");
            chasing = true;
        }
        else if(chasing)
        {
            Invoke("DeactivateAttack", attackDuration);
        }
        else
        {
            wanderScript.DoWander();
        }

        if (Time.time >= nextMoveTime && chasing)
        {
            if (shotsFiredThisMovement < shotsFiredPerMovement)
            {
                rb2d.velocity = Vector2.zero;
                shotsFiredThisMovement += enemyFireScript.Fire();
            }
            else
            {
                shotsFiredThisMovement = 0;
                nextMoveTime = Time.time + timeBetweenMoves;

                rb2d.velocity =
                    EnemyUtil.CalculateUnblockedDirection(transform.position, colliderSize, 2f, false) * speed;
            }
        }
        else
        {
            shotsFiredThisMovement = 0;
        }

        previousVelocity = rb2d.velocity;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Wall")
        {
            rb2d.velocity = VectorUtil.RotateVector(new Vector2(-previousVelocity.x, -previousVelocity.y),
                Random.Range(-bounceVariationDegrees, bounceVariationDegrees) * Mathf.Deg2Rad).normalized * speed;
            nextMoveTime = Time.time + .1f;
        }
    }

    bool WithinAttackRange()
    {
        return Vector3.Distance(Player.PlayerTransform.position, gameObject.transform.position) <= attackingDistance;
    }

    void DeactivateAttack()
    {
        rb2d.velocity = Vector2.zero;
        DeactivateChase();
    }
}
