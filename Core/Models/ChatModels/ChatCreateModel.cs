using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace Core.Models.ChatModels;

public class ChatCreateModel
{
    public string Name { get; set; }

    public RoomModel Room { get; set; }

    public bool IsPrivate { get; set; }

    public ICollection<UserModel> Users { get; set; }
}