using domain.Entities;


namespace domain.Repositories
{
    public interface ITransFormRepository<T> where T : class 
    {
        Task<List<T>> GetTransforms(Connection connection);
    }
}
