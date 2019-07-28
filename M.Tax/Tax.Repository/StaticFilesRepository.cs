using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tax.Model.DBModel;

namespace Tax.Repository
{
    public class StaticFilesRepository : BaseRepository<StaticFiles>
    {
        public StaticFilesRepository(RepositoryOption option) : base(option)
        {
        }
        public async Task<int> InsertFile(StaticFiles fileParam)
        {
            var sql = $@"insert into staticfiles (Title,SavePath,Extensions,ShowName,SortID,Type,Description,Version,CreateTime) 
                                    values(@Title,@SavePath,@Extensions,@ShowName,@SortID,@Type,@Description,@Version,now());";
            using (var conn = CreateMysqlConnection())
            {
                return await conn.ExecuteAsync(sql, fileParam);
            }
        }
    }
}
