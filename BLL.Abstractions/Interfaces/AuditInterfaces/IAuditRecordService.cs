using System.Linq.Expressions;
using Core.DataClasses;
using Core.Models.AuditModels;
using Core.Models.ChatModels;

namespace BLL.Abstractions.Interfaces.AuditInterfaces;

public interface IAuditRecordService
{
    Task<IEnumerable<AuditRecordModel>> GetByConditions(params Expression<Func<AuditRecordModel, bool>>[] conditions);

    Task<AuditRecordModel> GetAuditRecordById(int id);

    Task<OptionalResult<AuditRecordModel>> Create(AuditRecordModel auditRecord);

    Task<ExceptionalResult> Delete(int id);
}