using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HLSMP.Models;

[Table("TblRole_MAS")]
public partial class TblRoleMa
{
    [Key]
    public int RoleId { get; set; }

    [StringLength(20)]
    public string? Role { get; set; }
}
