using System;
using Tax.Model.DBModel;

namespace Tax.Model.ViewModel
{
    public class Menu : BaseDBModel
    {
        /// <summary>
        /// 菜单名
        /// </summary>
        public string MenuName { get; set; }

        public int IconFileID { get; set; }
        public string IconPath { get; set; }
        /// <summary>
        /// 菜单链接地址
        /// </summary>
        public string MenuUrl { get; set; }

        public string ParentName { get; set; }

        public int SortID { get; set; }

        public string BizCreateTime
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
