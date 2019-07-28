using Tax.Model.Enum;

namespace Tax.Model.DBModel
{
    public class StaticFiles : BaseDBModel
    {
        /// <summary>
        /// 上传文件的标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; set; }
        /// <summary>
        /// 文件后缀名
        /// </summary>
        public string Extensions { get; set; }
        /// <summary>
        /// 展示名称
        /// </summary>
        public string ShowName { get; set; }

        /// <summary>
        /// 排列序号
        /// </summary>
        public int SortID { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public FileType Type { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///版本号，timeticket
        /// </summary>
        public string Version {get;set;}
    }
}
