using System;

namespace Tax.Model.DBModel
{
    public class BaseDBModel
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
