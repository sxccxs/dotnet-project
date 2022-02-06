﻿using Core.Models;
using Core.Settings;
using DAL.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DAL.Workers
{
    internal class GenericJsonWorker<T> : IGenericStorageWorker<T>
        where T : BaseModel
    {
        private readonly IReaderWriter readerWriter;

        private readonly string storagePath;

        private readonly ILogger logger;

        public GenericJsonWorker(ILogger<GenericJsonWorker<T>> logger, IReaderWriter readerWriter, IOptions<JsonDbSettings> settings)
        {
            this.logger = logger;
            this.readerWriter = readerWriter;
            try
            {
                this.storagePath = this.GetFilePath(settings.Value);
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetByCondition(Func<T, bool> condition)
        {
            return (await this.GetAll()).Where(condition);
        }

        public async Task<int> GetNextId()
        {
            return (await this.GetAll()).OrderBy(x => x.Id).LastOrDefault()?.Id + 1 ?? 1;
        }

        public async Task Create(T entity)
        {
            var data = (await this.GetAll()).ToList();
            data.Add(entity);
            await this.readerWriter.Write(this.storagePath, data);
        }

        public async Task Delete(T entity)
        {
            var data = (await this.GetAll()).ToList();
            var item = data.FirstOrDefault(x => x.Id == entity.Id);
            data.Remove(item);
            await this.readerWriter.Write(this.storagePath, data);
        }

        public async Task Update(T entity)
        {
            var data = (await this.GetAll()).ToList();
            var index = data.IndexOf(data.FirstOrDefault(x => x.Id == entity.Id));
            data[index] = entity;
            await this.readerWriter.Write(this.storagePath, data);
        }

        private string GetFilePath(JsonDbSettings settings)
        {
            string removePostfix = "Model", addPostfix = "Directory";
            var propertyName = typeof(T).Name.Replace(removePostfix, addPostfix);
            return settings.GetType()
                           .GetProperty(propertyName)
                           ?.GetValue(settings)
                           ?.ToString()
                           ?? throw new ArgumentNullException($"There are no settings config or no json file for {typeof(T).FullName}");
        }

        private async Task<IEnumerable<T>> GetAll()
        {
            return await this.readerWriter.Read<IEnumerable<T>>(this.storagePath);
        }
    }
}
