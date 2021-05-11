using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tax.Common;
using Xunit;
using Xunit.Abstractions;

namespace Tax.XUnitTest
{
    public class CSRedisCoreOperatorTest
    {
        static string redisConnStr = "";
        static ITestOutputHelper OutPut;
        public CSRedisCoreOperatorTest(ITestOutputHelper tempOutput)
        {
            OutPut = tempOutput;
            redisConnStr = BaseCore.AppSetting.GetValue<string>("redis");
        }

        [Theory]
        [InlineData("tekey","tValue..")]
        public void RedisSetTest(string key , string value)
        {
            var cli = new CSRedisOperator(redisConnStr);
            cli.HSet(key,value);
            var cacheData = cli.HGet<string>(key);
            OutPut.WriteLine(cacheData);
            Assert.True(value==cacheData);

        }

        [Theory]
        [InlineData("csKey","test_value")]
        public void RedisHelperTest(string key, object val)
        {
            var cli = new CSRedisOperator(redisConnStr);
            var taskList = new List<Task>();
            for (var i = 0; i < 200; i++)
            {
                var currentI = i;
                taskList.Add(Task.Run(()=> {
                    for (var j = 0; j < 2000; j++)
                    {
                        cli.HSet(key+"_"+ currentI+"_"+j, val);
                    }
                }));
            }

            Task.WaitAll(taskList.ToArray());
        }
    }
}
