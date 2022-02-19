using Core.DataClasses;
using Core.Models.AuditModels;

namespace BLL.Abstractions.Interfaces.AuditInterfaces;

public interface IAuditService
{
    Task<ExceptionalResult> CreateAuditRecord(CreateAuditRecordModel createModel);

    OptionalResult<string> GetRecordMessage(AuditRecordModel record);
}