using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HLSMP.Models;

[Keyless]
[Table("DIS_MAS")]
public partial class DisMa
{
    [Column("DIS_CODE")]
    [StringLength(2)]
    [Unicode(false)]
    public string DisCode { get; set; } = null!;

    [Column("DIS_NAMH")]
    [StringLength(50)]
    public string DisNamh { get; set; } = null!;

    [Column("DIS_NAME")]
    [StringLength(50)]
    [Unicode(false)]
    public string DisName { get; set; } = null!;

    [Column("STA_CODE")]
    [StringLength(2)]
    [Unicode(false)]
    public string StaCode { get; set; } = null!;

    [Column("DIV_CODE")]
    [StringLength(2)]
    [Unicode(false)]
    public string? DivCode { get; set; }

    [Column("CDIS_CODE")]
    [StringLength(3)]
    public string? CdisCode { get; set; }

    public byte[] Version { get; set; } = null!;

    [Column("insert_user")]
    [StringLength(50)]
    public string? InsertUser { get; set; }

    [Column("insert_date", TypeName = "datetime")]
    public DateTime? InsertDate { get; set; }

    [Column("update_user")]
    [StringLength(50)]
    public string? UpdateUser { get; set; }

    [Column("update_date", TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }
}
