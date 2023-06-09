﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CorpCom.Models.CorpCom
{
    [Table("Position")]
    public partial class Position
    {
        public Position()
        {
            Banners = new HashSet<Banner>();
        }

        [Key]
        public int PositionId { get; set; }
        [StringLength(255)]
        public string PositionName { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedByUserId { get; set; }

        [InverseProperty("Position")]
        public virtual ICollection<Banner> Banners { get; set; }
    }
}