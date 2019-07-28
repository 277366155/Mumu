using Tax.Model.Enum;

namespace Tax.Model.DBModel
{
    public class ClientMenus : BaseDBModel
    {
        /// <summary>
        /// 菜单名
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 菜单链接地址
        /// </summary>
        public string MenuUrl { get; set; }
        /// <summary>
        /// 父菜单id
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 图标文件ID
        /// </summary>
        public int IcoFileID { get; set; }        
        /// <summary>
        /// 客户端类型
        /// </summary>
        public ClientTypeEnum ClientType { get; set; }
        /// <summary>
        /// 排列序号
        /// </summary>
        public int SortID { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
    }



}
