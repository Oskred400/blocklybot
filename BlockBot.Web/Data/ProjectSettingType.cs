﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlockBot.Web.Data
{
    public class ProjectSettingType
    {
        [ScaffoldColumn(false)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Determines if a project can have one of this setting type, or many of this setting type
        /// </summary>
        [Display(Name = "Allow Multiple")]
        public bool AllowsMany { get; set; }
    }
}
