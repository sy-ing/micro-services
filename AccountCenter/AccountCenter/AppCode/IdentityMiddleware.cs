using AccountCenter.Models;
using AccountCenter.Models.Data;
using AccountCenter.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AccountCenter.AppCode
{
    public class IdentityMiddleware
    {
        private readonly RequestDelegate _next;

        public IdentityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            QianMuResult qianMuResult = new QianMuResult();
            try
            {
                
                var Action = context.Request.Headers["Action"].FirstOrDefault();
                if (!string.IsNullOrEmpty(Action))
                {
                    if (Action.ToLower() != "login")
                    {

                        var SecretId = context.Request.Headers["SecretId"].FirstOrDefault();
                        var SecretKey = context.Request.Headers["SecretKey"].FirstOrDefault();
                        var Timestamp = context.Request.Headers["Timestamp"].FirstOrDefault();

                        if (string.IsNullOrEmpty(Timestamp) || string.IsNullOrEmpty(SecretId) || string.IsNullOrEmpty(SecretKey))
                        {
                            //验证无法通过
                            qianMuResult.Code = "403";
                            qianMuResult.Msg = "验证参数不全";
                            qianMuResult.Data = "";
                            await HandleExceptionAsync(context, qianMuResult);
                        }
                        else
                        {
                            var datetime = Method.GetTime(Timestamp);
                            if (DateTime.Now.AddMinutes(-1) > datetime)
                            {
                                //验证无法通过
                                qianMuResult.Code = "403";
                                qianMuResult.Msg = "请求超时";
                                qianMuResult.Data = "";
                                await HandleExceptionAsync(context, qianMuResult);
                            }
                            else
                            {
                                var Parameter = string.Empty;
                                Stream stream = context.Request.Body;

                                if (context.Request.ContentLength != null)
                                {
                                    byte[] buffer = new byte[context.Request.ContentLength.Value];
                                    stream.Read(buffer, 0, buffer.Length);
                                    Parameter = Encoding.UTF8.GetString(buffer);
                                }
                                StringBuilder stringBuilder = new StringBuilder();
                                stringBuilder.Append("Action=");
                                stringBuilder.Append(Action);
                                stringBuilder.Append("Parameter=");
                                stringBuilder.Append(Parameter);
                                stringBuilder.Append("SecretId=");
                                stringBuilder.Append(SecretId);
                                stringBuilder.Append("Timestamp=");
                                stringBuilder.Append(Timestamp);

                                var _key = EncryptHelper.Sha1(Base64.EncodeBase64(stringBuilder.ToString()));

                                if (_key != SecretKey)
                                {
                                    //验证无法通过
                                    qianMuResult.Code = "403";
                                    qianMuResult.Msg = "密钥验证失败";
                                    qianMuResult.Data = "";
                                    await HandleExceptionAsync(context, qianMuResult);
                                }
                                else
                                {
                                    DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
                                    ContextString dbContext = new ContextString(options);
                                    var _User = dbContext.Account.Where(i => i.LoginSession == SecretId).FirstOrDefault();

                                    if (_User == null)
                                    {
                                        //验证无法通过
                                        qianMuResult.Code = "403";
                                        qianMuResult.Msg = "无效的用户身份";
                                        qianMuResult.Data = "";
                                        await HandleExceptionAsync(context, qianMuResult);
                                    }
                                    else
                                    {
                                        await _next.Invoke(context);
                                    }
                                    
                                }
                            }


                        }



                    }
                    else
                    {
                        await _next.Invoke(context);
                    }
                }
                else
                {
                    //验证无法通过
                    qianMuResult.Code = "403";
                    qianMuResult.Msg = "未能检测到Action";
                    qianMuResult.Data = "";
                    await HandleExceptionAsync(context, qianMuResult);
                }
            




               

            }
            catch (Exception e)
            {
                qianMuResult.Code = "403";
                qianMuResult.Msg = e.Message + "|" + e.GetType().Name;
                qianMuResult.Data = "";
                await HandleExceptionAsync(context, qianMuResult);
            }

            /*
            var cultureQuery = context.Request.Query["culture"];
            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                var culture = new CultureInfo(cultureQuery);

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

            }
            */
            // Call the next delegate/middleware in the pipeline
         //   return this._next(context);
        }


        private async Task HandleExceptionAsync(HttpContext context, QianMuResult  result)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
