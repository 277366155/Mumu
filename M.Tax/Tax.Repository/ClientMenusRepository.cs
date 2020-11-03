using Dapper;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tax.Model.DBModel;
using Tax.Model.ViewModel;

namespace Tax.Repository
{
    public class ClientMenusRepository : BaseRepository<ClientMenus>
    {
        public ClientMenusRepository(RepositoryOption option) : base(option)
        {
        }

        public async Task<int> InsertMenuAsync(ClientMenus menuParam,IDbTransaction tran=null)
        {
            var sql = $@"insert into clientmenus (MenuName,MenuUrl,ParentID,IconFileId,SortID,Type,Enable,Version,CreateTime) 
                                    values(@MenuName,@MenuUrl,@ParentID,@IconFileId,@SortID,@Type,1,@Version,now());";
            var conn = (tran == null ||tran.Connection!=null)? CreateMysqlConnection() : tran.Connection;
            using (conn )
            {
                return await conn.ExecuteAsync(sql, menuParam,tran);
            }
        }

        public  async Task<Pager<Menu>> GetMenuPageListAsync(string filter, int pageIndex, int pageSize, object param = null, string sort = "ID desc")
        {
            var result = new Pager<Menu>();
            using (var conn = CreateMysqlConnection())
            {
                var sql = $@"select c.ID, c.MenuName,c.MenuUrl, c.SortID,c.CreateTime,IFNULL(pc.MenuName, '无') as ParentName,c.IconFileId,s.SavePath as IconPath
                                        from clientmenus c
                                        LEFT JOIN staticfiles s  on c.IconFileId = s.ID and s.Type = 0
                                        LEFT JOIN clientmenus pc on c.ParentID = pc.ID
                                        where 1=1 {filter} 
                                        order by {sort}
                                        limit {pageSize * (pageIndex - 1)},{pageSize};";

                var countSql = $@"select count(1) from `clientmenus`  c   where 1=1 {filter} ;";
                result.DataList = (await conn.QueryAsync<Menu>(sql, param))?.ToList();
                result.Total = (await conn.QueryAsync<int>(countSql, param)).FirstOrDefault();
                result.PageIndex = pageIndex;
                result.PageSize = pageSize;
            }
            return result;
        }
    }
}
