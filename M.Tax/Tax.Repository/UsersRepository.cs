using Dapper;
using System.Threading.Tasks;
using Tax.Model.DBModel;
using Tax.Model.ParamModel;

namespace Tax.Repository
{
    public class UsersRepository : BaseRepository<Users>
    {
        public UsersRepository(RepositoryOption option) : base(option)
        {
        }
        /// <summary>
        /// 插入user
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> InsertUserAsync(InsertUserParam param)
        {
            var sql = $@"insert into users (UserName,Password,Email) values(@UserName,@Password,@Email);";
            using (var conn = CreateMysqlConnection())
            {
                return await conn.ExecuteAsync(sql, param);
            }
        }
    }
}

