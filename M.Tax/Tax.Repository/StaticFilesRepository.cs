using Dapper;
using Microsoft.Extensions.Options;
using System.Data;
using System.Threading.Tasks;
using Tax.Model.DBModel;

namespace Tax.Repository
{
    public class StaticFilesRepository : BaseRepository<StaticFiles>
    {
        public StaticFilesRepository(IOptions<RepositoryOption> option) : base(option)
        {
        }
        public async Task<int> InsertFileAsync(StaticFiles fileParam,IDbTransaction tran=null)
        {
            var sql = $@"insert into staticfiles (Title,SavePath,Extensions,ShowName,SortID,Type,Description,Version,CreateTime) 
                                    values(@Title,@SavePath,@Extensions,@ShowName,@SortID,@Type,@Description,@Version,now());
                                    SELECT @@identity;";
            var conn = (tran == null || tran.Connection != null) ? CreateMysqlConnection() : tran.Connection;
            using (conn)
            {
                return await conn.QueryFirstOrDefaultAsync<int>(sql, fileParam, tran);
            }
        }
    }
}
