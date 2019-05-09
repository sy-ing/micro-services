using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountCenter.Models;
using AccountCenter.Models.Data;
using Microsoft.AspNetCore.Mvc;

namespace AccountCenter.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUser model, [FromServices] ContextString dbContext)
        {
            QianMuResult _Result = new QianMuResult();


            //检测用户登录情况
            //string username = Method.GetLoginUserName(dbContext, this.HttpContext).UserName;
            //if (string.IsNullOrEmpty(username))
            //{
            //    _Result.Code = "401";
            //    _Result.Msg = "请登陆后再进行操作";
            //    _Result.Data = "";
            //    return Json(_Result);
            //}


            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (RegisterUser)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

            if (string.IsNullOrEmpty(model.RoleCode))
            {
                _Result.Code = "510";
                _Result.Msg = "Erro:角色编码不可为空";
                _Result.Data = "";
                return Json(_Result);
            }
            //判断ID是否都为有效角色

            var q = await dbContext.Roles.Where(i => i.Code == model.RoleCode).AsNoTracking().CountAsync();
            if (q <= 0)
            {
                _Result.Code = "510";
                _Result.Msg = "Erro:没有Code为：" + model.RoleCode + "的角色";
                _Result.Data = "";
                return Json(_Result);
            }



            var _AvatarSrc = @"\images\DefaultAvatar.png";

            if (string.IsNullOrEmpty(model.AccountName)
                || string.IsNullOrEmpty(model.Password)
                || string.IsNullOrEmpty(model.Phone)
                || string.IsNullOrEmpty(model.Email)
                || model.Password != model.ConfirmPassword)
            {
                _Result.Code = "510";
                _Result.Msg = "输入信息不正确";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.Password.Length < 6)
            {
                _Result.Code = "510";
                _Result.Msg = "密码长度不因少于6位";
                _Result.Data = "";
                return Json(_Result);
            }

            //判断系统中是否存在用户
            if (Method.FindAllByName(dbContext, model.AccountName))
            {
                _Result.Code = "1";
                _Result.Msg = "当前用户名已存在";
                _Result.Data = "";
                return Json(_Result);
            }

            var phonenum = await dbContext.Account.Where(i => i.Phone == model.Phone && i.Activity).CountAsync();

            if (phonenum > 0)
            {
                _Result.Code = "1";
                _Result.Msg = "手机号码已被使用";
                _Result.Data = "";
                return Json(_Result);
            }
            var emailnum = await dbContext.Account.Where(i => i.Email == model.Email && i.Activity).CountAsync();

            if (emailnum > 0)
            {
                _Result.Code = "1";
                _Result.Msg = "邮箱已被使用";
                _Result.Data = "";
                return Json(_Result);
            }

            //创建用户
            var _User = new Account()
            {
                AccountName = model.AccountName,
                Code = Guid.NewGuid().ToString(),
                UpdateTime = DateTime.Now,
                PassWord = Method.StringToPBKDF2Hash(model.Password),
                NickName = model.NickName,
                Phone = model.Phone,
                Email = model.Email,
                AvatarSrc = _AvatarSrc,
                Activity = true,
                AddTime = DateTime.Now,
                MallCode = "",
                SystemModule = "Manage",
                Remark = model.Remark

            };

            int _AccountID = Method.CreateAccount(dbContext, _User).Result;

            //添加账户 角色关系
            if (_AccountID > 0)
            {

                try
                {
                    dbContext.UserRoles.Add(new UserRoles { UserCode = _User.Code, RoleCode = model.RoleCode });

                    await dbContext.SaveChangesAsync();
                    _Result.Code = "200";
                    _Result.Msg = "创建用户成功";
                    _Result.Data = "";

                    var ip = Method.GetUserIp(this.HttpContext);
                    dbContext.SysLog.Add(new SysLog { AccountName = username, ModuleName = "用户模块", LogMsg = username + "创建了用户名为：" + model.AccountName + "的用户,访问信息：" + inputStr, AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "创建", IP = ip, MallCode = "", SystemModule = "Manage" });
                    dbContext.SaveChanges();

                }
                catch (Exception e)
                {
                    _Result.Code = "500";
                    _Result.Msg = "Erro:关联用户-角色关系失败 " + e.ToString();
                    _Result.Data = "";

                }

            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "创建用户失败";
                _Result.Data = "";
            }
            return Json(_Result);
        }
    }
}