namespace Nukleus.Domain.SeedWork
{
    // Depending on Audit implementation (rollback or not).
    // May or may not be used.
    public interface ISoftDeleteEntity
    {
        bool IsDeleted { get; }
        void SetDeleted(bool isDeleted = true);
    }
}