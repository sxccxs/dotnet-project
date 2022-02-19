using System.ComponentModel.DataAnnotations;

namespace Core.Models.AuditModels;

public class ActionTypeModel : BaseModel
{
    [Required]
    public string Name { get; set; }
}