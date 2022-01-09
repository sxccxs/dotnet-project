using Core.Models;

namespace DAL.Abstractions.Interfaces
{
    public interface IGenericStorageWorker<T>
        where T : BaseModel
    {
        IEnumerable<T> GetAll();

        IEnumerable<T> GetByCondition(Func<T, bool> condition);

        int GetNextId();

        void Create(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
