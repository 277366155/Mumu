using Microsoft.Extensions.Configuration;
<<<<<<< HEAD
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
=======
using StackExchange.Redis;
>>>>>>> origin/dev
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Tax.Common;
using Tax.Common.Logs;
using Tax.Model.DBModel;
using Tax.Repository;
using Xunit;
using Xunit.Abstractions;

namespace Tax.XUnitTest
{
    public class UsersRepositoryTest
    {
        static string ConnStr = "";
        static ITestOutputHelper OutPut;
        public UsersRepositoryTest(ITestOutputHelper tempOutput)
        {
            OutPut = tempOutput;
            ConnStr = BaseCore.Configuration.GetConnectionString("TaxDB");
        }
        [Fact]
        public void InsertTest()
        {

            var sc = new ServiceCollection();
            sc.Configure<RepositoryOption>(BaseCore.Configuration.GetSection("ConnectionStrings"));
            sc.AddTransient<UsersRepository>();
            var sp = sc.BuildServiceProvider();
            var userRep = sp.GetService<UsersRepository>();
            var result=  userRep.InsertAsync(new Users() {  UserName="boo", Password="123", CreateTime= DateTime.Now}).Result;

            OutPut.WriteLine($"data:{result}"); ;
            Assert.Equal(1, result);

            //var data = userRep.GetModel("3").Result;
            // Assert.Equal(data.UserName,"boo");
        }

        [Fact]
        public void GrayLogTest()
        {
            GrayLogHelper.Log("test log", "fact graylog test message . ");
            Assert.True(true);
        }
        [Theory]
        [InlineData("8BrVyZqsJf3MA116kk2tXA==")]
        public void PwdTest(string pwd)
        {
            var d = DEncrypt.Decrypt(pwd);
            OutPut.WriteLine(d);
        }

        [Theory]
        [InlineData(10000)]
        [InlineData(50000)]
        public void ReflectTestMethod(int num)
        {
            var data = new Users();
            var sp = new Stopwatch();
            sp.Start();
            for (var i = 0; i < num; i++)
            {
                ReflectTest(data,true);
            }
            sp.Stop();
            OutPut.WriteLine($"使用缓存循环{num}次，耗时：【{sp.ElapsedMilliseconds/1000.0}(s)】");

            sp.Restart();
            for (var i = 0; i < num; i++)
            {
                ReflectTest(data, false);
            }
            sp.Stop();
            OutPut.WriteLine($"不用缓存循环{num}次，耗时：【{sp.ElapsedMilliseconds/1000.0}(s)】");

        }

        static Dictionary<string, string> sqlDic = new Dictionary<string, string>();
        static object lockObj = new object();
        private string ReflectTest<T>(T model, bool cacheEnable)
        {
            var dicKey = typeof(T).Name;
            if (cacheEnable)
            {
                if (!sqlDic.ContainsKey(dicKey))
                {
                    lock (lockObj)
                    {
                        if (!sqlDic.ContainsKey(dicKey))
                        {
                            var sql = GetSql(model);
                            sqlDic.Add(dicKey, sql);
                        }
                    }
                }
                return sqlDic[dicKey];
            }
            return GetSql(model);
        }

        private string GetSql<T>(T model)
        {
            var sql = $"insert into `{typeof(T).Name.ToLower()}` ";
            var columns = new StringBuilder();
            var values = new StringBuilder();

            foreach (var p in typeof(T).GetProperties())
            {
                if (p.Name == "ID")
                {
                    continue;
                }
                columns.Append($"`{p.Name}`,");
                var pValue = p.GetValue(model);
                if (pValue == null)
                {
                    values.Append($"  null ,");
                }
                else
                {
                    switch (pValue.GetType().Name.ToLower())
                    {
                        case "string":
                        case "datetime":
                            values.Append($"'{pValue}',");
                            break;
                        default:
                            values.Append($" {pValue} ,");
                            break;
                    }
                }
            }
            sql += $"({columns.ToString().TrimEnd(',')}) value ({values.ToString().TrimEnd(',')});";
            return sql;
        }
    }

}
