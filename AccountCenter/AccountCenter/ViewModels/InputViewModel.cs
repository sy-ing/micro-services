using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountCenter.ViewModels
{
    public class InputViewModel
    {
        /// <summary>
        /// 方法名
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Action")]
        public string Action { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Parameter")]
        public string Parameter { get; set; }


        /// <summary>
        /// 密钥Id
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SecretId")]
        public string SecretId { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SecretKey")]
        public string SecretKey { get; set; }

        /// <summary>
        /// 当前时间戳
        /// </summary>
        [Display(Name = "Timestamp")]
        public long Timestamp { get; set; }
    }
}
