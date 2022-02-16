using Core.Models.ChatModels;
using Core.Models.UserModels;

namespace Core.Models.MessageModels;

public class CreateMessageModel
{
    public int Id { get; set; }

    public string Text { get; set; }

    public UserModel Author { get; set; }

    public TextChatModel Chat { get; set; }
}