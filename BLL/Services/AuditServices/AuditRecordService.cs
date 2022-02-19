using System.Linq.Expressions;
using BLL.Abstractions.Interfaces.AuditInterfaces;
using Core.DataClasses;
using Core.Models.AuditModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.AuditServices;

public class AuditRecordService : IAuditRecordService
{
    private readonly IGenericStorageWorker<AuditRecordModel> storage;

    public AuditRecordService(IGenericStorageWorker<AuditRecordModel> storage)
    {
        this.storage = storage;
    }

    public async Task<IEnumerable<AuditRecordModel>> GetByConditions(params Expression<Func<AuditRecordModel, bool>>[] conditions)
    {
        return await this.storage.GetByConditions(
            conditions,
            ar => ar.Room,
            ar => ar.Actor,
            ar => ar.UserUnderAction,
            ar => ar.ActionType,
            ar => ar.TextChat,
            ar => ar.VoiceChat);
    }

    public async Task<AuditRecordModel> GetAuditRecordById(int id)
    {
        return (await this.GetByConditions(ar => ar.Id == id)).FirstOrDefault();
    }

    public async Task<OptionalResult<AuditRecordModel>> Create(AuditRecordModel auditRecord)
    {
        await this.storage.Create(auditRecord);

        return new OptionalResult<AuditRecordModel>(auditRecord);
    }

    public async Task<ExceptionalResult> Delete(int id)
    {
        var record = await this.GetAuditRecordById(id);
        if (record is null)
        {
            return new ExceptionalResult(false, $"Record with id {id} does not exist");
        }

        await this.storage.Delete(record);

        return new ExceptionalResult();
    }
}