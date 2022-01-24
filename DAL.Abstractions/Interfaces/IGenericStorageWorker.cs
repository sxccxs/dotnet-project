using Core.Models;

namespace DAL.Abstractions.Interfaces
{
    public interface IGenericStorageWorker<T>
        where T : BaseModel
    {
        Task<IEnumerable<T>> GetAll();

        Task<IEnumerable<T>> GetByCondition(Func<T, bool> condition);

        Task<int> GetNextId();

        Task Create(T entity);

        Task Update(T entity);

        Task Delete(T entity);
    }
}
