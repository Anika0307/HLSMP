using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HLSMP.Models;

[Table("TblReason_MAS")]
public partial class TblReasonMa
{
    [Key]
    public int ReasonId { get; set; }

    [StringLength(100)]
    public string? Reason { get; set; }
}
