﻿using UnityEngine;

public class BallControl : MonoBehaviour
{
    Rigidbody2D rb2d;
    Vector2 velocity;
    AudioSource audioSource;
    [SerializeField] float smallestSpeedYAxis = 0;

    [SerializeField] public PointEvent OnPoint;

    #region Private Methods

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Resets the ball position and velocity.
    /// </summary>
    private void ResetBall()
    {
        velocity = Vector2.zero;
        rb2d.velocity = velocity;
        transform.position = Vector2.zero;
    }

    /// <summary>
    /// OnCollisionEnter2D event.
    /// </summary>
    /// <param name="collision">The collision parameters.</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.Play();
            velocity.x = rb2d.velocity.x;
            velocity.y = (rb2d.velocity.y / 2.0f) + (collision.collider.attachedRigidbody.velocity.y / 3.0f);

            if (Mathf.Abs(velocity.y) < smallestSpeedYAxis)
            {
                velocity.y = velocity.y > 0 ? smallestSpeedYAxis : -smallestSpeedYAxis;
            }

            rb2d.velocity = velocity;
        }

        else if (collision.gameObject.name == "RightWall")
        {
            OnPoint.Invoke(WallPosition.RightWall);
        }

        else if (collision.gameObject.name == "LeftWall")
        {
            OnPoint.Invoke(WallPosition.LeftWall);
        }

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Starts moving the ball in a random direction
    /// </summary>
    public void GoBall()
    {
        float rand = Random.Range(0f, 2f);
        rb2d.AddForce(new Vector2((rand < 1 ? 1 : -1) * 20, -15));
    }

    /// <summary>
    /// Restarts the 
    /// </summary>
    public void RestartBallAndGo()
    {
        ResetBall();
        GoBall();
    }

    /// <summary>
    /// Sets the ball to active or unactive.
    /// </summary>
    /// <param name="value">Activates or deactivates the object.</param>
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    #endregion
}
