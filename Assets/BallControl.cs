using UnityEngine;

public class BallControl : MonoBehaviour
{
    private Rigidbody2D rb2d = null;
    private Vector2 velocity;

    [SerializeField] public PointEvent OnPoint;

    #region Private Methods

    /// <summary>
    /// Resets the ball position and velocity.
    /// </summary>
    private void ResetBall()
    {
        if (rb2d == null)
        {
            rb2d = GetComponent<Rigidbody2D>();
        }
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
            velocity.x = rb2d.velocity.x;
            velocity.y = (rb2d.velocity.y / 2.0f) + (collision.collider.attachedRigidbody.velocity.y / 3.0f);
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

    #endregion
}
