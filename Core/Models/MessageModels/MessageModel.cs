using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Models.ChatModels;
using Core.Models.UserModels;

namespace Core.Models.MessageModels;

public class MessageModel : BaseModel
{
    [Required]
    [Column(TypeName = "ntext")]
    public string Text { get; set; }

    [Required]
    public UserModel Author { get; set; }

    [Required]
    public TextChatModel Chat { get; set; }

    [Required]
    public DateTime SendingTime { get; set; }

    [Required]
    public bool IsEdited { get; set; }

    public MessageModel ReplyTo { get; set; }

    public UserModel ForwardedFrom { get; set; }
}