using System.Collections.Generic;
using Interfaces;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Ball : MonoBehaviour
{
    public float speed;

    // For controlling increases in ball speed due to hitting bricks
    private float _initialSpeed;
    private readonly List<Color> _colorsHit = new List<Color>();

    private Rigidbody2D _rb2d;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _rb2d.velocity = new Vector2(0, speed);
        _initialSpeed = speed;
    }

    // Some collisions (eg corners) change speed even with a 100% bouncy physics material
    // Setting speed on every physics frame as a sloppy fix to this
    private void FixedUpdate()
    {
        _rb2d.velocity = speed * _rb2d.velocity.normalized;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Special case for collision with top of paddle
        if (collision.collider.CompareTag("Paddle") && collision.contacts[0].normal == Vector2.up)
        {
            // Reflect based on x diff between ball and paddle
            float xDiff = transform.position.x - collision.collider.transform.position.x;
            _rb2d.velocity = new Vector2(xDiff * 3, 1).normalized * _rb2d.velocity.magnitude;
            return;
        }

        // Send hit to surface, if applicable
        collision.gameObject.GetComponent<IDamageable>()?.TakeHit(1);

        // Increase speed if object is a new color of brick
        Brick brick = collision.gameObject.GetComponent<Brick>();
        if (brick && !_colorsHit.Contains(brick.GetColor()))
        {
            _colorsHit.Add(brick.GetColor());
            speed = _initialSpeed * (1 + _colorsHit.Count * 0.5f);
        }
    }
}
