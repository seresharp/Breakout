using System.IO;
using Interfaces;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Brick : MonoBehaviour, IDamageable
{
    public int health;
    public Sprite[] images;

    private int _maxHealth;

    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();

        if (images == null || images.Length < health)
        {
            throw new InvalidDataException("Brick component must have images set for all hp values");
        }

        _maxHealth = health;
        UpdateSprite();
    }

    /// <inheritdoc />
    public bool TakeHit(int damage)
    {
        if ((health -= damage) <= 0)
        {
            Destroy(gameObject);
            return true;
        }

        UpdateSprite();
        return false;
    }

    public void SetColor(Color c)
    {
        _renderer.color = c;
    }

    private void UpdateSprite()
    {
        int healthLost = _maxHealth - health;

        _renderer.sprite = images[healthLost];
    }
}
