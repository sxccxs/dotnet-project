using System.Resources;
using AutoMapper;
using BLL.Abstractions.Interfaces.AuditInterfaces;
using Core.DataClasses;
using Core.Enums;
using Core.Models.AuditModels;
using Core.Settings;
using Microsoft.Extensions.Options;

namespace BLL.Services.AuditServices;

public class AuditService : IAuditService
{
    private readonly IActionTypeService actionTypeService;

    private readonly IAuditRecordService auditRecordService;

    private readonly AppSettings appSettings;

    public AuditService(IOptions<AppSettings> appSettings, IActionTypeService actionTypeService, IAuditRecordService auditRecordService)
    {
        this.actionTypeService = actionTypeService;
        this.auditRecordService = auditRecordService;
        this.appSettings = appSettings.Value;
    }

    public async Task<ExceptionalResult> CreateAuditRecord(CreateAuditRecordModel createModel)
    {
        var actionType =
            (await this.actionTypeService.GetByCondition(at => at.Name == createModel.ActionType.ToString())).FirstOrDefault();
        if (actionType is null)
        {
            return new ExceptionalResult(false, "Invalid action type provided.");
        }

        var record = this.MapCreateModelToRecordModel(createModel);
        record.ActionType = actionType;

        return await this.auditRecordService.Create(record);
    }

    public OptionalResult<string> GetRecordMessage(AuditRecordModel record)
    {
        try
        {
            var resources = new ResourceManager(this.appSettings.RecordMessagesPath, typeof(AuditRecordModel).Assembly);
            string result;
            var name = record.ActionType.Name;
            switch (name)
            {
                case nameof(ActionType.AddUserToRoom) or nameof(ActionType.DeleteUserFromRoom):
                    result = string.Format(resources.GetString(name) !, this.GetUserUnderActionName(record), record.Room.Name, this.GetActorName(record));
                    break;
                case nameof(ActionType.ChangeUserRoleType):
                    result = string.Format(resources.GetString(name) !, this.GetUserUnderActionName(record), record.OldRole.Name, record.Room.Name, this.GetActorName(record));
                    break;
                case nameof(ActionType.AddUserToTextChat) or nameof(ActionType.DeleteUserFromTextChat):
                    result = string.Format(resources.GetString(name) !, this.GetUserUnderActionName(record), record.TextChat.Name, record.Room.Name, this.GetActorName(record));
                    break;
                case nameof(ActionType.AddUserToVoiceChat) or nameof(ActionType.DeleteUserFromVoiceChat):
                    result = string.Format(resources.GetString(name) !, this.GetUserUnderActionName(record), record.VoiceChat.Name, record.Room.Name, this.GetActorName(record));
                    break;
                case nameof(ActionType.MessageForward):
                    result = string.Format(resources.GetString(name) !, this.GetActorName(record), record.TextChat.Name, this.GetUserUnderActionName(record), record.Room.Name);
                    break;
                case nameof(ActionType.MessageReply):
                    result = string.Format(resources.GetString(name) !, this.GetActorName(record), this.GetUserUnderActionName(record), record.TextChat.Name, record.Room.Name);
                    break;
                case nameof(ActionType.EditRoomInfo) or nameof(ActionType.UserLeftFromRoom):
                    result = string.Format(resources.GetString(name) !, this.GetActorName(record), record.Room.Name);
                    break;
                case nameof(ActionType.EditTextChatInfo) or nameof(ActionType.CreateTextChat) or nameof(ActionType.DeleteTextChat):
                    result = string.Format(resources.GetString(name) !, this.GetActorName(record), record.TextChat.Name, record.Room.Name);
                    break;
                case nameof(ActionType.EditVoiceChatInfo) or nameof(ActionType.CreateVoiceChat) or nameof(ActionType.DeleteVoiceChat):
                    result = string.Format(resources.GetString(name) !, this.GetActorName(record), record.VoiceChat.Name, record.Room.Name);
                    break;
                default:
                    return new OptionalResult<string>(false, "Invalid record type provided.");
            }

            return new OptionalResult<string>(result);
        }
        catch (Exception ex)
        {
            return new OptionalResult<string>(false, $"Some record fields was not specified: {ex.Message}");
        }
    }

    private AuditRecordModel MapCreateModelToRecordModel(CreateAuditRecordModel createModel)
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateAuditRecordModel, AuditRecordModel>().ForAllMembers(opt => opt.AllowNull());
        });

        return new Mapper(mapperConfiguration).Map<AuditRecordModel>(createModel);
    }

    private string GetActorName(AuditRecordModel record) => record?.Actor?.UserName ?? "Deleted account";

    private string GetUserUnderActionName(AuditRecordModel record) => record?.UserUnderAction?.UserName ?? "Deleted account";
}
