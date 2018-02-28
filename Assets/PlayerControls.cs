using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public float speed = 5.0f;
    public float boundY = 2.25f;

    private Rigidbody2D rb2d;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var velocity = rb2d.velocity;
        velocity.y = 0;
        if (Input.GetKey(moveUp))
        {
            velocity.y += speed;
        }
        if (Input.GetKey(moveDown))
        {
            velocity.y -= speed;
        }
        rb2d.velocity = velocity;

        var position = transform.position;
        if (position.y < -boundY || position.y > boundY)
        {
            position.y = Mathf.Clamp(position.y, -boundY, boundY);
            transform.position = position;
        }
    }
}
