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
        [StringLength(32)]
        public string Title { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Required(ErrorMessage = "序号不能为空")]
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
