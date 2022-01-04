using Core.Models;

namespace DAL.Abstractions.Interfaces
{
    public interface IGenericStorageWorker<T> where T : BaseModel
    {
        public IEnumerable<T> GetAll();

        public IEnumerable<T> GetByCondition(Func<T, bool> condition);

        public int GetNextId();

        public void Create(T entity);

        public void Update(T entity);

        public void Delete(T entity);
    }
}
