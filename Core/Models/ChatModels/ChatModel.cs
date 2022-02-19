using System.ComponentModel.DataAnnotations;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace Core.Models.ChatModels;

public class ChatModel : BaseModel
{
    [Required]
    [MaxLength(32)]
    public string Name { get; set; }

    [Required]
    public RoomModel Room { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    public bool IsPrivate { get; set; }

    public ICollection<UserModel> Users { get; set; } = new HashSet<UserModel>();
}