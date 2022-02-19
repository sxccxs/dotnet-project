using Core.Models.UserModels;

namespace Core.Models.ChatModels;

public class ChatUpdateModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<UserModel> Users { get; set; }
}