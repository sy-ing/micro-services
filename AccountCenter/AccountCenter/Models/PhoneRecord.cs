using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountCenter.Models
{
    public class PhoneRecord : Base
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [StringLength(100)]
        [Display(Name = "VerCode")]
        public string VerCode { get; set; }

        /// <summary>
        /// 是否已验证
        /// </summary>
        [Display(Name = "Status")]
        public bool Status { get; set; }
    }
}
