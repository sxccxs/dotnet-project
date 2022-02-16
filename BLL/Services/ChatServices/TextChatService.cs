using System.Linq.Expressions;
using AutoMapper;
using BLL.Abstractions.Interfaces.ChatInterfaces;
using Core.DataClasses;
using Core.Models.ChatModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.ChatServices;

public class TextChatService : ITextChatService
{
   private readonly IGenericStorageWorker<TextChatModel> storage;

   public TextChatService(IGenericStorageWorker<TextChatModel> storage)
   {
      this.storage = storage;
   }

   public async Task<IEnumerable<TextChatModel>> GetByConditions(params Expression<Func<TextChatModel, bool>>[] conditions)
   {
      return await this.storage.GetByConditions(
          conditions,
          tc => tc.Users,
          tc => tc.Room,
          tc => tc.Messages);
   }

   public async Task<TextChatModel> GetTextChatById(int id)
   {
      return (await this.GetByConditions(tc => tc.Id == id)).FirstOrDefault();
   }

   public async Task<OptionalResult<TextChatModel>> Create(ChatCreateModel chatModel)
   {
      var chat = this.MapRoomCreateModelToTextChatModel(chatModel);
      await this.storage.Create(chat);

      return new OptionalResult<TextChatModel>(chat);
   }

   public async Task<OptionalResult<TextChatModel>> Update(TextChatUpdateModel chatModel)
   {
      if (await this.GetTextChatById(chatModel.Id) is null)
      {
         return new OptionalResult<TextChatModel>(false, $"Text chat with id {chatModel.Id} does not exist");
      }

      var chat = await this.MapRoomUpdateModelToRoomModel(chatModel);
      await this.storage.Update(chat);

      return new OptionalResult<TextChatModel>(chat);
   }

   public async Task<ExceptionalResult> Delete(int id)
   {
      var textChat = await this.GetTextChatById(id);
      if (textChat is null)
      {
         return new ExceptionalResult(false, $"Text chat with id {id} does not exist");
      }

      await this.storage.Delete(textChat);

      return new ExceptionalResult();
   }

   private TextChatModel MapRoomCreateModelToTextChatModel(ChatCreateModel createModel)
   {
      var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<ChatCreateModel, TextChatModel>());
      var mapper = new Mapper(mapperConfiguration);
      var chat = mapper.Map<TextChatModel>(createModel);

      return chat;
   }

   private async Task<TextChatModel> MapRoomUpdateModelToRoomModel(TextChatUpdateModel updateModel)
   {
      var mapperConfiguration = new MapperConfiguration(cfg =>
      {
         cfg.CreateMap<TextChatUpdateModel, TextChatModel>().ForAllMembers(opt => opt.AllowNull());
      });
      var mapper = new Mapper(mapperConfiguration);
      var room = mapper.Map<TextChatModel>(updateModel);
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