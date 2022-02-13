using System.ComponentModel.DataAnnotations;

namespace Core.Models.RoleModels;

public class RoleTypeModel : BaseModel
{
    [Required]
    public string Name { get; set; }
}