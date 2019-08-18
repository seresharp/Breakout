using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddleController : MonoBehaviour
{
    // Use of constants makes it easier to implement configurable controls later
    private const KeyCode LEFT = KeyCode.LeftArrow;
    private const KeyCode RIGHT = KeyCode.RightArrow;

    public float acceleration;
    public float maxSpeed;

    private Rigidbody2D _rb2d;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Check for user input
        if (Input.GetKey(LEFT))
        {
            _rb2d.AddForce(new Vector2(-acceleration, 0));
        }
        else if (Input.GetKey(RIGHT))
        {
            _rb2d.AddForce(new Vector2(acceleration, 0));
        }
        else if (_rb2d.velocity.magnitude <= 0.1f)
        {
            // Deceleration causes wobbling around 0, this check prevents that
            _rb2d.velocity = Vector2.zero;
        }
        else
        {
            // Decelerate if no buttons are held
            _rb2d.AddForce(new Vector2(_rb2d.velocity.x < 0 ? acceleration : -acceleration, 0));
        }

        // Cap paddle to max velocity
        if (_rb2d.velocity.magnitude > maxSpeed)
        {
            _rb2d.velocity = new Vector2(_rb2d.velocity.x < 0 ? -maxSpeed : maxSpeed, 0);
        }
    }
}
