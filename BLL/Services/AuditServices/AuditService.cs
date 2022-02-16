using AutoMapper;
using BLL.Abstractions.Interfaces.AuditInterfaces;
using Core.DataClasses;
using Core.Enums;
using Core.Models.AuditModels;

namespace BLL.Services.AuditServices;

public class AuditService : IAuditService
{
    private readonly IActionTypeService actionTypeService;

    private readonly IAuditRecordService auditRecordService;

    public AuditService(IActionTypeService actionTypeService, IAuditRecordService auditRecordService)
    {
        this.actionTypeService = actionTypeService;
        this.auditRecordService = auditRecordService;
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
            string result;
            switch (record.ActionType.Name)
            {
                case nameof(ActionType.AddUserToRoom):
                    result =
                        $"User {this.GetUserUnderActionName(record)} was added to room {record.Room.Name} by user {this.GetActorName(record)}.";
                    break;
                case nameof(ActionType.DeleteUserFromRoom):
                    result =
                        $"User {this.GetUserUnderActionName(record)} was removed from room {record.Room.Name} by user {this.GetActorName(record)}.";
                    break;
                case nameof(ActionType.ChangeUserRoleType):
                    result =
                        $"Role of user {this.GetUserUnderActionName(record)} was changed from {record.OldRole.Name} to {record.NewRole.Name} in room {record.Room.Name} by user {this.GetActorName(record)}.";
                    break;
                case nameof(ActionType.AddUserToTextChat):
                    result =
                        $"User {this.GetUserUnderActionName(record)} was added to text chat {record.TextChat.Name} in room {record.Room.Name} by user {this.GetActorName(record)}.";
                    break;
                case nameof(ActionType.DeleteUserFromTextChat):
                    result =
                        $"User {this.GetUserUnderActionName(record)} was removed to text chat {record.TextChat.Name} in room {record.Room.Name} by user {this.GetActorName(record)}.";
                    break;
                case nameof(ActionType.AddUserToVoiceChat):
                    result =
                        $"User {this.GetUserUnderActionName(record)} was added to text chat {record.VoiceChat.Name} in room {record.Room.Name} by user {this.GetActorName(record)}.";
                    break;
                case nameof(ActionType.DeleteUserFromVoiceChat):
                    result =
                        $"User {this.GetUserUnderActionName(record)} was removed to text chat {record.VoiceChat.Name} in room {record.Room.Name} by user {this.GetActorName(record)}.";
                    break;
                case nameof(ActionType.MessageForward):
                    result =
                        $"User {this.GetActorName(record)} forwarded message from chat {record.TextChat.Name} and user {this.GetUserUnderActionName(record)} in room {record.Room.Name}.";
                    break;
                case nameof(ActionType.MessageReply):
                    result =
                        $"User {this.GetActorName(record)} replied to message of user {this.GetUserUnderActionName(record)} in chat {record.TextChat.Name} in room {record.Room.Name}.";
                    break;
                case nameof(ActionType.EditRoomInfo):
                    result = $"User {this.GetActorName(record)} changed info of room {record.Room.Name}.";
                    break;
                case nameof(ActionType.EditTextChatInfo):
                    result =
                        $"User {this.GetActorName(record)} changed info of text chat {record.TextChat} in room {record.Room.Name}.";
                    break;
                case nameof(ActionType.EditVoiceChatInfo):
                    result =
                        $"User {this.GetActorName(record)} changed info of voice chat {record.VoiceChat} in room {record.Room.Name}";
                    break;
                case nameof(ActionType.UserLeftFromRoom):
                    result = $"User {this.GetActorName(record)} left room {record.Room.Name}.";
                    break;
                case nameof(ActionType.CreateTextChat):
                    result =
                        $"User {this.GetActorName(record)} created text chat {record.TextChat.Name} in room {record.Room.Name}.";
                    break;
                case nameof(ActionType.DeleteTextChat):
                    result =
                        $"User {this.GetActorName(record)} deleted text chat {record.TextChat.Name} in room {record.Room.Name}.";
                    break;
                case nameof(ActionType.CreateVoiceChat):
                    result =
                        $"User {this.GetActorName(record)} created voice chat {record.VoiceChat.Name} in room {record.Room.Name}.";
                    break;
                case nameof(ActionType.DeleteVoiceChat):
                    result =
                        $"User {this.GetActorName(record)} deleted voice chat {record.VoiceChat.Name} in room {record.Room.Name}.";
                    break;
                default:
                    return new OptionalResult<string>(false, "Invalid record type provided.");
            }

            return new OptionalResult<string>(result);
        }
        catch (NullReferenceException ex)
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