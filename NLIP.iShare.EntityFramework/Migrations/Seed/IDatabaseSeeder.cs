namespace NLIP.iShare.EntityFramework
{
    /// <summary>
    /// Defines a seeding strategy from one database migration to another
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IDatabaseSeeder<TContext>
    {
        void Seed();
        string EnvironmentName { get; }
    }
}
