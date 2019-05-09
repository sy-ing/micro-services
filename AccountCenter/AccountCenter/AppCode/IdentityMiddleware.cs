using AccountCenter.Models.Data;
using AccountCenter.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public Task Invoke(HttpContext context, InputViewModel model)
        {
            try
            {

                Stream stream = context.Request.Body;
                byte[] buffer = new byte[context.Request.ContentLength.Value];
                stream.Read(buffer, 0, buffer.Length);
                string inputStr = Encoding.UTF8.GetString(buffer);
                model = (InputViewModel)Newtonsoft.Json.JsonConvert.DeserializeObject(inputStr, model.GetType());

                // string SessionID = context.Request.Cookies["SessionID"];

                //  int? _ID = context.Session.GetInt32("AccountID");
                if (string.IsNullOrEmpty(model.Action))
                {
              
                }

       
            }
            catch (Exception e)
            {
                QMLog qMLog = new QMLog();
                qMLog.WriteLogToFile("IdentityMiddleware", e.ToString());
                throw;
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
            return this._next(context);
        }
    }
}
