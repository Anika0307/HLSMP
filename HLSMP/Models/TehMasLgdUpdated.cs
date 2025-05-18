using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HLSMP.Models;

[Keyless]
[Table("TEH_MAS_LGD_UPDATED")]
public partial class TehMasLgdUpdated
{
    [Column("DIS_CODE")]
    [StringLength(2)]
    [Unicode(false)]
    public string DisCode { get; set; } = null!;

    [Column("TCODE_OLD")]
    [StringLength(2)]
    public string? TcodeOld { get; set; }

    [Column("TEH_NAME")]
    [StringLength(25)]
    public string TehName { get; set; } = null!;

    [Column("TEH_NAMH")]
    [StringLength(25)]
    public string TehNamh { get; set; } = null!;

    [Column("TEH_TYPE")]
    [StringLength(50)]
    public string TehType { get; set; } = null!;

    [Column("SDO_CODE")]
    [StringLength(2)]
    [Unicode(false)]
    public string? SdoCode { get; set; }

    [Column("TEH_CODE")]
    [StringLength(3)]
    [Unicode(false)]
    public string TehCode { get; set; } = null!;

    [Column("PTEH_CODE")]
    [StringLength(3)]
    [Unicode(false)]
    public string? PtehCode { get; set; }

    [Column("CTEH_CODE")]
    [StringLength(6)]
    public string? CtehCode { get; set; }

    [Column("insert_user")]
    [StringLength(50)]
    public string? InsertUser { get; set; }

    [Column("insert_date", TypeName = "smalldatetime")]
    public DateTime? InsertDate { get; set; }

    [Column("update_user")]
    [StringLength(50)]
    public string? UpdateUser { get; set; }

    [Column("update_date", TypeName = "smalldatetime")]
    public DateTime? UpdateDate { get; set; }

    [Column("LGD_CODE")]
    [StringLength(10)]
    public string? LgdCode { get; set; }
}
