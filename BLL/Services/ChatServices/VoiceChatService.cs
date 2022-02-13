using System.Linq.Expressions;
using AutoMapper;
using BLL.Abstractions.Interfaces.ChatInterfaces;
using Core.DataClasses;
using Core.Models.ChatModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.ChatServices;

public class VoiceChatService : IVoiceChatService
{
    private readonly IGenericStorageWorker<VoiceChatModel> storage;

    public VoiceChatService(IGenericStorageWorker<VoiceChatModel> storage)
    {
        this.storage = storage;
    }

    public async Task<IEnumerable<VoiceChatModel>> GetByConditions(
        params Expression<Func<VoiceChatModel, bool>>[] conditions)
    {
        return await this.storage.GetByConditions(
            conditions,
            vc => vc.Users,
            vc => vc.Room);
    }

    public async Task<VoiceChatModel> GetTextChatById(int id)
    {
        return (await this.GetByConditions(vc => vc.Id == id)).FirstOrDefault();
    }

    public async Task<OptionalResult<VoiceChatModel>> Create(ChatCreateModel chatModel)
    {
        var chat = this.MapRoomCreateModelToVoiceChatModel(chatModel);
        await this.storage.Create(chat);

        return new OptionalResult<VoiceChatModel>(chat);
    }

    public async Task<OptionalResult<VoiceChatModel>> Update(ChatUpdateModel chatModel)
    {
        if (await this.GetTextChatById(chatModel.Id) is null)
        {
            return new OptionalResult<VoiceChatModel>(false, $"Voice chat with id {chatModel.Id} does not exist");
        }

        var chat = await this.MapRoomUpdateModelToRoomModel(chatModel);
        await this.storage.Update(chat);

        return new OptionalResult<VoiceChatModel>(chat);
    }

    public async Task<ExceptionalResult> Delete(int id)
    {
        var voiceChat = await this.GetTextChatById(id);
        if (voiceChat is null)
        {
            return new ExceptionalResult(false, $"Voice chat with id {id} does not exist");
        }

        await this.storage.Delete(voiceChat);

        return new ExceptionalResult();
    }

    private VoiceChatModel MapRoomCreateModelToVoiceChatModel(ChatCreateModel createModel)
    {
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<ChatCreateModel, VoiceChatModel>());
        var mapper = new Mapper(mapperConfiguration);
        var chat = mapper.Map<VoiceChatModel>(createModel);

        return chat;
    }

    private async Task<VoiceChatModel> MapRoomUpdateModelToRoomModel(ChatUpdateModel updateModel)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ChatUpdateModel, VoiceChatModel>().ForAllMembers(opt => opt.AllowNull());
            });
            var mapper = new Mapper(mapperConfiguration);
            var room = mapper.Map<VoiceChatModel>(updateModel);
            var changingRoom = await this.GetTextChatById(updateModel.Id);

            foreach (var prop in room.GetType().GetProperties())
            {
                if (prop.GetValue(room) is not null)
                {
                    prop.SetValue(changingRoom, prop.GetValue(room));
                }
            }

            return changingRoom;
        }
    }
