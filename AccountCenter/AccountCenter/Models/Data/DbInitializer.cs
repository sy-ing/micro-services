using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountCenter.Models.Data
{
    public class DbInitializer
    {
        public static void Initialize(ContextString context)
        {

            //如果当前数据库不存在按照当前 model 创建
            context.Database.EnsureCreated();


            // Look for any files.
            if (context.Account.Any())
            {
                return;   // DB has been seeded
            }

            context.SaveChanges();


            var admin = new Account
            {
                AccountName = "admin",
                Code = Guid.NewGuid().ToString(),
                Activity = true,
                AddTime = DateTime.Now,
                AvatarSrc = "",
                Birthday = DateTime.Now,
                Email = "",
                Gender = "",
                LastLoginIP = "",
                NickName = "超级管理员",
                Phone = "",
                PassWord = "iUZHVuuOHrtoyI9wisHwUQbt78/nBA6YItQNst2sYi4=",
                MallCode = "",
                SystemModule = "Manage"
            };

            context.Account.Add(admin);

            context.SaveChanges();
        }

    }
}
