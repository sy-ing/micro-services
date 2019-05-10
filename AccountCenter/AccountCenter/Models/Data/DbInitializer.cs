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
            if (context.Menu.Any())
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
                PassWord = "MEhI/iCoERQ/f9BWC8k8+c+qGS+Xdz02GwXn+CwIth8=",
                MallCode = "",
                SystemModule = "Manage"
            };

            context.Account.Add(admin);

        }

    }
}
