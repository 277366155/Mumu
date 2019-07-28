using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tax.Model.DBModel;
using Tax.Model.ViewModel;

namespace Tax.Repository
{
    public class BaseRepository<T> where T:BaseDBModel
    {
        static string _conStr;
        static object lockObj = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected static IDbConnection CreateMysqlConnection()
        {
           var conn= new MySqlConnection(_conStr);
            conn.Open();
            return conn;
        }

        public BaseRepository(RepositoryOption option)
        {            
            _conStr = option.ConnectionString;
        }

        public virtual async Task<T> GetModel(int id)
        {
            using (var conn=CreateMysqlConnection())
            {
                var sql = $"select * from `{typeof(T).Name.ToLower()}`  where ID=@id;";
                return await conn.QueryFirstOrDefaultAsync<T>(sql, new { id });
            }
        }

        public virtual async Task<IEnumerable<T>> GetList(string filter,object param=null)
        {
            using (var conn = CreateMysqlConnection())
            {
                var sql = $"select * from `{typeof(T).Name.ToLower()}`  where 1=1 {filter};";
                return await conn.QueryAsync<T>(sql, param);
            }
        }
        public virtual async Task<T> FirstOrDefault(string filter, object param = null)
        {
            var list = await GetList(filter, param);

            return list.FirstOrDefault();
        }

        public virtual async Task<Pager<T>> GetPageList(string filter,int pageIndex,int pageSize, object param = null,string sort="ID desc")
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
                result.DataList=( await conn.QueryAsync<T>(sql, param))?.ToList();
                result.Total= (await conn.QueryAsync<int>(countSql, param)).FirstOrDefault();
                result.PageIndex = pageIndex;
                result.PageSize = pageSize;
            }
            return result;
        }
        //update/delete/insert...[todo]

        public virtual async Task<int> Update(T param) 
        {
            var fieldStr = new StringBuilder();
            foreach (var p in typeof(T).GetProperties())
            {
                    fieldStr.Append($" `{p.Name}`=@{p.Name},");              
            }
            using (var conn = CreateMysqlConnection())
            {
                var sql = $"update `{typeof(T).Name.ToLower()}` set {fieldStr.ToString().Trim(',')} where id = {param.ID}";
                return await conn.ExecuteAsync(sql, param);
            }
        }
        /// <summary>
        ///查询存在数量
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<int>  Count(string filter, object param = null)
        {
            using (var conn = CreateMysqlConnection())
            {
                var sql = $@"select count(1) from  `{typeof(T).Name.ToLower()}`  where 1=1 {filter};";
                return await conn.QueryFirstOrDefaultAsync<int>(sql,param);
            }
        }

        public virtual async Task<int> Insert(T model)
        {
            using (var conn = CreateMysqlConnection())
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
                sql+=$"({columns.ToString().TrimEnd(',')}) value ({values.ToString().TrimEnd(',')});";
                return await conn.ExecuteAsync(sql.ToString());
            }
        }

        public virtual async Task<int> Delete(int id)
        {
            using (var conn = CreateMysqlConnection())
            {
                var sql = $"delete from `{typeof(T).Name.ToLower()}` where id ={id};";
                return await conn.ExecuteAsync(sql);
            }
        }
    }
}
