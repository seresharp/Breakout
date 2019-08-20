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
    private int _points;

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
            GameManager.Instance.Score += _points;
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

    public Color GetColor()
    {
        return _renderer.color;
    }

    public void SetPoints(int points)
    {
        _points = points;
    }

    private void UpdateSprite()
    {
        int healthLost = _maxHealth - health;

        _renderer.sprite = images[healthLost];
    }
}
