using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
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
            GrayLogHelper.Log("test log","fact graylog test message . ");
            Assert.True(true);
        }
        [Theory]
        [InlineData("8BrVyZqsJf3MA116kk2tXA==")]
        public void PwdTest(string pwd)
        {
            var d= DEncrypt.Decrypt(pwd);
            OutPut.WriteLine(d);

        }
    }
}
