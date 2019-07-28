using System;
using System.ComponentModel.DataAnnotations;
using Tax.Model.Enum;

namespace Tax.Model.ParamModel
{
    public  class SaveSlideParam
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 上传文件的标题
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(32, ErrorMessage = "名称长度不能超过32个字")]
        public string Title { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Required(ErrorMessage = "显示序号不能为空")]
        [Range(maximum:999, minimum:0,ErrorMessage ="排列序号范围为0~999")]
        public int SortID { get; set; }

        /// <summary>
        /// 保存路径
        /// </summary>
        [Required(ErrorMessage = "请选择图片并上传")]
        public string SavePath { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public FileType Type => FileType.Slide;

        public DateTime? CreateTime { get; set; }
    }
}
