using Microsoft.AspNetCore.Hosting;
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
        /// DateTime时间格式转换为Unix时间戳格式    
        /// </summary>    
        /// <param name="time"> DateTime时间格式</param>    
        /// <returns>Unix时间戳格式</returns>    
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);//等价的建议写法
            return (int)(time - startTime).TotalSeconds;
        }
    }
}
