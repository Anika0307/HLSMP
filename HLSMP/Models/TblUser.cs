using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HLSMP.Models;

[Keyless]
[Table("TblUser")]
public partial class TblUser
{
    public int? UserId { get; set; }

    public int? RoleId { get; set; }

    public int? DisCode { get; set; }

    public bool? IsActive { get; set; }
}
