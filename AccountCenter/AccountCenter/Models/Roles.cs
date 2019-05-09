using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountCenter.Models
{
    public class Roles : Base
    {


        /// <summary>
        /// 角色名称
        /// </summary>
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [Display(Name = "Intro")]
        [StringLength(2000)]
        public string Intro { get; set; }
    }
}
