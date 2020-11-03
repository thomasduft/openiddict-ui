using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tomware.Microip.Web
{
  [Table("ClaimType")]
  public class ClaimType
  {
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public static ClaimType Create(
      string name,
      string description = null
    )
    {
      return new ClaimType
      {
        Id = Guid.NewGuid(),
        Name = name,
        Description = description
      };
    }
  }
}