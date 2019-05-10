using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountCenter.ViewModels
{
    public class AccountViewModel
    {
    }

    /// <summary>
    /// 注册用户
    /// </summary>
    public class RegisterUser
    {

        /// <summary>
        /// 账户名称
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "账户名称长度应该为1-100个字符", MinimumLength = 1)]
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "用户密码至少需要6个字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// 密码确认
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "二次输入的密码不一致")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "NickName")]
        public string NickName { get; set; }



        /// <summary>
        /// 手机
        /// </summary>
        [Required]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        [Display(Name = "RoleCode")]
        public string RoleCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(2000)]
        [Display(Name = "Remark")]
        public String Remark { get; set; }

    }


    /// <summary>
    /// 登录请求接受
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [Key]
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// 记住用户？
        /// </summary>
        [Display(Name = "Remember me?")]
        public string RememberMe { get; set; }
    }


    public class Input_LoginViewModelBySMS
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Display(Name = "vercode")]
        public string vercode { get; set; }

        /// <summary>
        /// 记住用户？
        /// </summary>
        [Display(Name = "Remember me?")]
        public string RememberMe { get; set; }
    }
}
