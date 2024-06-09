namespace LogAnalyzer.Domain.Interfaces
{
    public interface IRepositoryEF<TEntity>
    {
        Task<TEntity> GetById(Guid id, CancellationToken cancellationToken);
        Task<List<TEntity>> GetAll(CancellationToken cancellationToken);
        Task Add(TEntity entity, CancellationToken cancellationToken);
        Task Update(TEntity entity, CancellationToken cancellationToken);
        Task Delete(TEntity entity, CancellationToken cancellationToken);
    }
}
