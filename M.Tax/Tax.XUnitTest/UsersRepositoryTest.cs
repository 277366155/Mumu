using Microsoft.Extensions.Configuration;
using System;
using Tax.Common;
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
            var userRep = new UsersRepository(new RepositoryOption(ConnStr));
          var result=  userRep.Insert(new Users() {  UserName="boo", Password="123", CreateTime= DateTime.Now}).Result;
            OutPut.WriteLine($"data:{result}"); ;
            Assert.Equal(1, result);

           //var data = userRep.GetModel("3").Result;
           // Assert.Equal(data.UserName,"boo");
        }
    }
}
