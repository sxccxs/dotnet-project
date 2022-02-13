using System.Linq.Expressions;
using Core.Models;
using DAL.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Workers;

public class GenericDbWorker<T> : IGenericStorageWorker<T>
    where T : BaseModel
{
    private readonly AppContext context;

    public GenericDbWorker(AppContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<T>> GetByConditions(Expression<Func<T, bool>>[] conditions, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = this.context.Set<T>();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        foreach (var condition in conditions)
        {
            query = query.Where(condition);
        }

        return (await query.ToListAsync()).ToHashSet();
    }

    public async Task Create(T entity)
    {
        await this.context.Set<T>().AddAsync(entity);
        await this.Save();
    }

    public async Task Update(T entity)
    {
        this.context.Set<T>().Update(entity);
        await this.Save();
    }

    public async Task Delete(T entity)
    {
        this.context.Set<T>().Remove(entity);
        await this.Save();
    }

    private async Task Save()
    {
        await this.context.SaveChangesAsync();
    }
}