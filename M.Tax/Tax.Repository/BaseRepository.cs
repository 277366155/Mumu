using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tax.Common;
using Tax.Model.DBModel;
using Tax.Model.ViewModel;

namespace Tax.Repository
{
    public class BaseRepository<T> where T : BaseDBModel
    {
        static IOptionsMonitor<RepositoryOption> _repOpt;
        static object lockObj = new object();
        IDbConnection conn;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateMysqlConnection()
        {
            if (conn == null)
            {
                lock (lockObj)
                {
                    if (conn == null)
                    {
                        conn = new MySqlConnection(_repOpt.CurrentValue.TaxDB);
                    }
                }
            }
            if (conn.State != ConnectionState.Open)
                conn.Open();
            return conn;
        }

        public BaseRepository(IOptionsMonitor<RepositoryOption> option)
        {
            _repOpt = option;
            _repOpt.OnChange(opt =>
            {
                Console.WriteLine("RepositoryOption was changed :" + opt.ToJson());
            });
        }

        public virtual async Task<T> GetModelAsync(int id)
        {
            using (var conn = CreateMysqlConnection())
            {
                var sql = $"select * from `{typeof(T).Name.ToLower()}`  where ID=@id;";
                return await conn.QueryFirstOrDefaultAsync<T>(sql, new { id });
            }
        }

        public virtual async Task<IEnumerable<T>> GetListAsync(string filter, object param = null)
        {
            using (var conn = CreateMysqlConnection())
            {
                var sql = $"select * from `{typeof(T).Name.ToLower()}`  where 1=1 {filter};";
                return await conn.QueryAsync<T>(sql, param);
            }
        }
        public virtual async Task<T> FirstOrDefaultAsync(string filter, object param = null)
        {
            var list = await GetListAsync(filter, param);

            return list.FirstOrDefault();
        }

        public virtual async Task<Pager<T>> GetPageListAsync(string filter, int pageIndex, int pageSize, object param = null, string sort = "ID desc")
        {
            var result = new Pager<T>();
            using (var conn = CreateMysqlConnection())
            {
                var sql = $@"select * from `{typeof(T).Name.ToLower()}`  
                                    where 1=1 {filter} 
                                    order by {sort}
                                    limit {pageSize * (pageIndex - 1)},{pageSize};";

                var countSql = $@"select count(1) from `{typeof(T).Name.ToLower()}`  
                                    where 1=1 {filter} ;";
                result.DataList = (await conn.QueryAsync<T>(sql, param))?.ToList();
                result.Total = (await conn.QueryAsync<int>(countSql, param)).FirstOrDefault();
                result.PageIndex = pageIndex;
                result.PageSize = pageSize;
            }
            return result;
        }
        //update/delete/insert...[todo]

        public virtual async Task<int> UpdateAsync(T param, IDbTransaction tran = null)
        {
            var fieldStr = new StringBuilder();
            foreach (var p in typeof(T).GetProperties())
            {
                fieldStr.Append($" `{p.Name}`=@{p.Name},");
            }
            var conn = (tran == null || tran.Connection == null) ? CreateMysqlConnection() : tran.Connection;

            var sql = $"update `{typeof(T).Name.ToLower()}` set {fieldStr.ToString().Trim(',')} where id = {param.ID}";
            var result = await conn.ExecuteAsync(sql, param, tran);

            if (tran == null || tran.Connection == null)
                conn.Dispose();

            return result;
        }
        /// <summary>
        ///查询存在数量
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(string filter, object param = null)
        {
            using (var conn = CreateMysqlConnection())
            {
                var sql = $@"select count(1) from  `{typeof(T).Name.ToLower()}`  where 1=1 {filter};";
                return await conn.QueryFirstOrDefaultAsync<int>(sql, param);
            }
        }

        public virtual async Task<int> InsertAsync(T model, IDbTransaction tran = null)
        {
            IDbConnection conn;
            if (tran == null)
            {
                conn = CreateMysqlConnection();
            }
            else
            {
                conn = tran.Connection;
            }

            var sql = $"insert into `{typeof(T).Name.ToLower()}` ";
            var columns = new StringBuilder();
            var paramStrBuild = new StringBuilder();

            foreach (var p in typeof(T).GetProperties())
            {
                if (p.Name.ToLower() == "id")
                {
                    continue;
                }
                columns.Append($"`{p.Name}`,");
                paramStrBuild.Append($"@{p.Name},");
            }
            sql += $"({columns.ToString().TrimEnd(',')}) values ({paramStrBuild.ToString().TrimEnd(',')});";
            var result = await conn.ExecuteAsync(sql.ToString(), model, tran);
            if (tran == null)
            {
                conn.Dispose();
            }
            return result;
        }

        public virtual async Task<int> DeleteAsync(int id)
        {
            using (var conn = CreateMysqlConnection())
            {
                var sql = $"delete from `{typeof(T).Name.ToLower()}` where id ={id};";
                return await conn.ExecuteAsync(sql);
            }
        }
    }
}
