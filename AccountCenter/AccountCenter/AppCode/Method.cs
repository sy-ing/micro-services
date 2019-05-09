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
    }
}
