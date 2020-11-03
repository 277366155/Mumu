using System;
using System.ComponentModel.DataAnnotations;
using Tax.Model.Enum;

namespace Tax.Model.ParamModel
{
    public  class SaveMenuParam
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public int? ID { get; set; }
        /// <summary>
        /// 菜单名
        /// </summary>
        [Required(ErrorMessage = "菜单名称不能为空")]
        [StringLength(16, ErrorMessage = "菜单名称长度不能超过16个字")]
        public string MenuName { get; set; }
        /// <summary>
        /// 菜单链接地址
        /// </summary>        
        [StringLength(1024, ErrorMessage = "菜单地址长度不能超过1024个字")]
        public string MenuUrl { get; set; }
        /// <summary>
        /// 父菜单id
        /// </summary>
        public int? ParentID { get; set; }
        /// <summary>
        /// 图标文件ID
        /// </summary>
        //[Required(ErrorMessage = "菜单图标不能为空")]
        public int? IconFileID { get; set; }
        /// <summary>
        /// 客户端类型
        /// </summary>
        public ClientTypeEnum ClientType => ClientTypeEnum.PC;
        /// <summary>
        /// 排列序号
        /// </summary>
        [Required(ErrorMessage = "显示序号不能为空")]
        [Range(maximum: 999, minimum: -999, ErrorMessage = "排列序号范围为-999~999")]
        public int SortID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 图标文件
        /// </summary>
        public SaveMenuIconParam MenuIcon { get; set; }
    }
}
