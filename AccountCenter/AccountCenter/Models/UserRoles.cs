using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountCenter.Models
{
    public class UserRoles : Base
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        [Display(Name = "UserCode")]
        [StringLength(50)]
        public string UserCode { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [Required]
        [Display(Name = "RoleCode")]
        [StringLength(50)]
        public string RoleCode { get; set; }
    }
}
