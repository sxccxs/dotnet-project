using System.Reflection;

namespace DAL.Abstractions.Interfaces;

public interface ITransactionsWorker
{
    Task<T> RunAsTransaction<T>(Func<Task<T>> method);
}