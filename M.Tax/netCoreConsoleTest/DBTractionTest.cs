using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Tax.Common;
using Tax.Repository;

namespace netCoreConsoleTest
{
    public  class DBTractionTest
    {
        static UsersRepository usersRep;

        public DBTractionTest()
        {
            Init();
        }

        public void Init()
        {
            var sc = new ServiceCollection();
            sc.Configure<RepositoryOption>(BaseCore.Configuration.GetSection("ConnectionStrings"));
            sc.AddSingleton<UsersRepository>();
            var sp = sc.BuildServiceProvider();
            usersRep= sp.GetService<UsersRepository>();
        }

        public async void TranTest()
        {
            using (var con = usersRep.CreateMysqlConnection())
            {
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        await usersRep.UpdateAsync(new Tax.Model.DBModel.Users {ID=1, UserName = "test", Password = "222", Email = "test@test.com", CreateTime=DateTime.Now }, tran);
                        con.Close();
                        Thread.Sleep(20 * 1000);
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                    }
                }
            }
        }
    }
}
