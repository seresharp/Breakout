using Interfaces;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Ball : MonoBehaviour
{
    public Vector2 velocity;

    private Rigidbody2D _rb2d;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _rb2d.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Special case for paddle collision
        if (collision.collider.CompareTag("Paddle"))
        {
            // Reflect based on x diff between ball and paddle
            float xDiff = transform.position.x - collision.collider.transform.position.x;
            velocity = new Vector2(xDiff * 3, 1).normalized * velocity.magnitude;
            _rb2d.velocity = velocity;
            return;
        }

        // Reflect ball off of collided surface
        // Using collision normals results in odd bounces on corners, raycast works better
        RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity, 2.5f);
        if (hit.collider)
        {
            velocity = Vector2.Reflect(velocity, hit.normal);
            _rb2d.velocity = velocity;
        }

        // Send hit to surface, if applicable
        collision.gameObject.GetComponent<IDamageable>()?.TakeHit(1);
    }
}
