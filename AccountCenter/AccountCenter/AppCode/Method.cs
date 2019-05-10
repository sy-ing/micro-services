using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountCenter.AppCode
{
    public class Method
    {
        public static IHostingEnvironment _hostingEnvironment { get; set; }

        //服务器地址
        public static string ServerAddr { get; set; }


        //数据库字符串
        public static string ContextStr { get; set; }


        /// <summary>    
        /// Unix时间戳转为C#格式时间    
        /// </summary>    
        /// <param name="timeStamp">Unix时间戳格式,例如1482115779</param>    
        /// <returns>C#格式时间</returns>    
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);

            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }


        /// <summary>
        /// 字符串进行PBKDF2加密
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>

        public static string StringToPBKDF2Hash(string inputStr)
        {
            byte[] salt = new byte[128 / 8];

            string outString = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                     password: inputStr,//明文
                                                     salt: salt,//盐
                                                     prf: KeyDerivationPrf.HMACSHA1,//加密方式
                                                     iterationCount: 10000,//迭代次数
                                                     numBytesRequested: 256 / 8));

            return outString;
        }

        /// <summary>    
        /// DateTime时间格式转换为Unix时间戳格式    
        /// </summary>    
        /// <param name="time"> DateTime时间格式</param>    
        /// <returns>Unix时间戳格式</returns>    
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);//等价的建议写法
            return (int)(time - startTime).TotalSeconds;
        }


        /// <summary>
        /// 获取用户IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserIp(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
    }
}
