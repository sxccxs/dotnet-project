using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class RoleTypeModel : BaseModel
{
    [Required]
    public string Name { get; set; }
}