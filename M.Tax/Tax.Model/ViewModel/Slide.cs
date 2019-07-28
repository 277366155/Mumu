using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tax.Model.DBModel;

namespace Tax.Model.ViewModel
{
    public class Slide : BaseDBModel
    {
        /// <summary>
        /// 上传文件的标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 排列序号
        /// </summary>
        public int SortID { get; set; }
        /// <summary>
        ///版本号，timeticket
        /// </summary>
        public string Version { get; set; }

        public string BizCreateTime
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
