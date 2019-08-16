namespace Interfaces
{
    public interface IDamageable
    {
        /// <summary>
        /// Causes an object to take damage equivalent to the passed in amount
        /// </summary>
        /// <param name="damage">The damage to take</param>
        /// <returns>A bool indicating if the damage was fatal</returns>
        bool TakeHit(int damage);
    }
}
