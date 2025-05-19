using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HLSMP.Models;

[Table("VIL_MAS")]
public partial class VilMa
{
    [Column("DIS_CODE")]
    [StringLength(2)]
    public int DisCode { get; set; } 

    [Column("TEH_CODE")]
    [StringLength(3)]
    public int TehCode { get; set; }

    [Key]
    [Column("VIL_CODE")]
    [StringLength(5)]
    public string VilCode { get; set; } = null!;

    [Column("VIL_NAME")]
    [StringLength(50)]
    public string VilName { get; set; } = null!;

    [Column("VIL_NAMH")]
    [StringLength(50)]
    public string? VilNamh { get; set; }

    [Column("BLK_CODE")]
    [StringLength(4)]
    public string? BlkCode { get; set; }

    [Column("SDO_CODE")]
    [StringLength(2)]
    public string? SdoCode { get; set; }

    [Column("CON_CODE")]
    [StringLength(2)]
    public string? ConCode { get; set; }

    [Column("PCON_CODE")]
    [StringLength(5)]
    public string? PconCode { get; set; }

    [Column("STA_CODE")]
    [StringLength(2)]
    public string? StaCode { get; set; }

    [Column("CENSUS_VCODE")]
    [StringLength(4)]
    public string? CensusVcode { get; set; }

    [Column("VILL_TYPE")]
    [StringLength(1)]
    public string? VillType { get; set; }

    [Column("MERGED_IN_TW")]
    [StringLength(1)]
    public string? MergedInTw { get; set; }

    [Column("RURAL_URBAN")]
    [StringLength(1)]
    public string? RuralUrban { get; set; }

    [Column("MC_CODE")]
    [StringLength(3)]
    public string? McCode { get; set; }

    [Column("DIV_CODE")]
    [StringLength(2)]
    public string? DivCode { get; set; }

    [Column("HBT_NO")]
    [StringLength(15)]
    public string? HbtNo { get; set; }

    public byte[]? Version { get; set; }

    [Column("insert_user")]
    [StringLength(50)]
    public string? InsertUser { get; set; }

    [Column("Insert_date", TypeName = "datetime")]
    public DateTime? InsertDate { get; set; }

    [Column("update_user")]
    [StringLength(50)]
    public string? UpdateUser { get; set; }

    [Column("update_date", TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }
}
