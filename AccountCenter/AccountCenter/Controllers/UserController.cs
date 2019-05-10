using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AccountCenter.AppCode;
using AccountCenter.Models;
using AccountCenter.Models.Data;
using AccountCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountCenter.Controllers
{
    public class UserController : Controller
    {

 

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>       
        [HttpPost]
        public IActionResult Login(LoginViewModel model, [FromServices] ContextString dbContext, string returnUrl = null)
        {


            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (LoginViewModel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            if (string.IsNullOrEmpty(model.AccountName) || string.IsNullOrEmpty(model.Password))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入正确格式的用户名或密码";
                _Result.Data = "";
                return Json(_Result);
            }

            if (model.Password.Length < 6)
            {
                _Result.Code = "510";
                _Result.Msg = "用户密码应为6-20位字符";
                _Result.Data = "";
                return Json(_Result);
            }
            Account _User = dbContext.Account.Where(i => i.Activity == true && (i.AccountName == model.AccountName || i.Phone == model.AccountName)).FirstOrDefault();
            //判断系统中是否存在用户
            if (_User == null)
            {
                _Result.Code = "503";
                _Result.Msg = "用户不存在或不可用";
                _Result.Data = "";
                return Json(_Result);
            }

            //加密用户密码
            string _PWD = Method.StringToPBKDF2Hash(model.Password);

            if (_User.PassWord == _PWD)
            {
            

                string _LoginSession = Guid.NewGuid().ToString();//会话唯一标记
                                                                 //保存会话状态
                var _InvalidTime = DateTime.Now.AddDays(1);
                if (model.RememberMe == "on")
                {
                    _InvalidTime = DateTime.Now.AddDays(7);
                }

                //更新用户信息
                var ip = Method.GetUserIp(this.HttpContext);
                string _LastLoginIP = ip;
                DateTime _LastLoginTime = DateTime.Now;

                _User.InvalidTime = _InvalidTime;
                _User.LoginSession = _LoginSession;
                _User.LastLoginTime = _LastLoginTime;
                _User.LastLoginIP = _LastLoginIP;
                dbContext.Account.Update(_User);
                dbContext.SaveChanges();


                //日志记录
                try
                {
                    dbContext.SysLog.Add(new SysLog { AccountName = _User.AccountName, ModuleName = "用户模块", LogMsg = _User.AccountName + "登陆了系统", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "登录", IP = ip, SystemModule = _User.SystemModule, MallCode = _User.MallCode });
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {

                    QMLog qm = new QMLog();
                    qm.WriteLogToFile("", e.ToString());
                }
                _Result.Code = "200";
                _Result.Msg = "登陆成功";
                _Result.Data = _LoginSession;


            }
            else
            {
                _Result.Code = "2";
                _Result.Msg = "用户名或密码不正确";
                _Result.Data = "";
            }


            return Json(_Result);


        }





        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>       
        [HttpPost]
        public async Task<IActionResult> LoginBySMS(Input_LoginViewModelBySMS model, [FromServices] ContextString dbContext, string returnUrl = null)
        {


            QianMuResult _Result = new QianMuResult();

            Stream stream = HttpContext.Request.Body;
            byte[] buffer = new byte[HttpContext.Request.ContentLength.Value];
            stream.Read(buffer, 0, buffer.Length);
            string inputStr = Encoding.UTF8.GetString(buffer);
            model = (Input_LoginViewModelBySMS)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());


            if (string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.vercode))
            {
                _Result.Code = "510";
                _Result.Msg = "请输入手机号和验证码";
                _Result.Data = "";
                return Json(_Result);
            }

            //判断系统中是否存在用户

            var accnum = dbContext.Account.Where(i => i.Phone == model.Phone && i.Activity == true).Count();

            if (accnum <= 0)
            {
                _Result.Code = "1";
                _Result.Msg = "用户不存在或不可用";
                _Result.Data = "";
                return Json(_Result);
            }

            var count = await dbContext.PhoneRecord.Where(i => i.Phone == model.Phone && i.VerCode == model.vercode).CountAsync();

            if (count > 0)
            {
                var pr = await dbContext.PhoneRecord.Where(i => i.Phone == model.Phone).OrderByDescending(o => o.AddTime).FirstOrDefaultAsync();

                if (pr.VerCode == model.vercode && pr.UpdateTime >= DateTime.Now.AddMinutes(-5))
                {
                    pr.Status = true;
                    pr.UpdateTime = DateTime.Now;

                    dbContext.PhoneRecord.Update(pr);
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        Account _User = dbContext.Account.Where(i => i.Phone == model.Phone).FirstOrDefault();

                        string _LoginSession = Guid.NewGuid().ToString();//会话唯一标记
                                                                         //保存会话状态
                        var _InvalidTime = DateTime.Now.AddDays(1);
                        if (model.RememberMe == "on")
                        {
                            _InvalidTime = DateTime.Now.AddDays(7);
                        }

                        //更新用户信息
                        var ip = Method.GetUserIp(this.HttpContext);
                        string _LastLoginIP = ip;
                        DateTime _LastLoginTime = DateTime.Now;

                        _User.InvalidTime = _InvalidTime;
                        _User.LoginSession = _LoginSession;
                        _User.LastLoginTime = _LastLoginTime;
                        _User.LastLoginIP = _LastLoginIP;

                        dbContext.Account.Update(_User);
                        dbContext.SaveChanges();


                     
                        //日志记录

                        try
                        {
                            dbContext.SysLog.Add(new SysLog { AccountName = _User.AccountName, ModuleName = "用户模块", LogMsg = _User.AccountName + "登陆了系统", AddTime = DateTime.Now, Code = Guid.NewGuid().ToString(), Type = "登录", IP = ip, SystemModule = _User.SystemModule, MallCode = _User.MallCode });
                            dbContext.SaveChanges();

                        }
                        catch (Exception e)
                        {

                            QMLog qm = new QMLog();
                            qm.WriteLogToFile("", e.ToString());
                        }
                        _Result.Code = "200";
                        _Result.Msg = "登陆成功";
                        _Result.Data = _LoginSession;

                    }
                    else
                    {
                        _Result.Code = "2";
                        _Result.Msg = "用户名或密码不正确";
                        _Result.Data = "";
                    }

                }
                else
                {
                    _Result.Code = "2";
                    _Result.Msg = "验证码不正确";
                    _Result.Data = "";
                }


            }









            return Json(_Result);


        }


        /// <summary>
        /// 检测号码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbContext"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>       
        public IActionResult CheckPhoneNum(string phone, [FromServices] ContextString dbContext, string returnUrl = null)
        {
            QianMuResult _Result = new QianMuResult();
            //判断系统中是否存在用户

            var accnum = dbContext.Account.Where(i => i.Phone == phone && i.Activity == true).Count();

            if (accnum <= 0)
            {
                _Result.Code = "1";
                _Result.Msg = "用户不存在或不可用";
                _Result.Data = "";
                return Json(_Result);
            }
            else
            {
                _Result.Code = "200";
                _Result.Msg = "号码正确";
                _Result.Data = "";
            }
            return Json(_Result);
        }

        public IActionResult GetLocalIP([FromServices] ContextString dbContext)
        {
            try
            {
                var SecretId = HttpContext.Request.Headers["SecretId"].FirstOrDefault();
                var user = dbContext.Account.Where(i => i.LoginSession == SecretId).FirstOrDefault();
                int _ID = user.ID;
                 string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                string ip = "";
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        ip = ip + " | " + IpEntry.AddressList[i].ToString();
                    }
                }
                return Json("ID:" + _ID + "         IP:" + ip);
            }
            catch (Exception ex)
            {
                return Json("获取本机IP出错:" + ex.Message); ;
            }
        }
    }
}