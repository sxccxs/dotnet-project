using Core.Models.MessageModels;

namespace Core.Models.ChatModels;

public class TextChatModel : ChatModel
{
    public ICollection<MessageModel> Messages { get; set; } = new HashSet<MessageModel>();
}