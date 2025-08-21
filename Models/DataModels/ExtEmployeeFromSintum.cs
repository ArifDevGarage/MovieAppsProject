using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieAppsProject.Models.DataModels;

[Table("ExtEmployeeFromSinta", Schema = "ExternalData")]
public partial class ExtEmployeeFromSintum
{
    [Key]
    public int Id { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? EmployeeId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? EmployeeName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string PositionId { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? PositionName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Area { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? PlantArea { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Directorate { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Function { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Department { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Level { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? SuperiorId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? SuperiorPositionId { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string? UserName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Unit { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Posgrd { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? CostCenter { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Entity { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdate { get; set; }

    [Column("Helper_IsDelegate")]
    public bool? HelperIsDelegate { get; set; }

    [Column("Helper_EmployeePositionTypeId")]
    public byte? HelperEmployeePositionTypeId { get; set; }
}
