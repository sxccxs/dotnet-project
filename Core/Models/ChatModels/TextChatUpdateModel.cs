using Core.Models.MessageModels;

namespace Core.Models.ChatModels;

public class TextChatUpdateModel : ChatUpdateModel
{
    public ICollection<MessageModel> Messages { get; set; }
}