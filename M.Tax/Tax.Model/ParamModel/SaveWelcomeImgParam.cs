using System;
using System.ComponentModel.DataAnnotations;
using Tax.Model.Enum;

namespace Tax.Model.ParamModel
{
    public class SaveWelcomeImgParam : SaveStaticFilesParamBase
    {
        /// <summary>
        /// ID
        /// </summary>
        public override int Id { get; set; }
        /// <summary>
        /// 上传文件的标题
        /// </summary>
        public override string Title
        {
            get { return "首页欢迎图片"; }
            set { }
        }

        public override int SortID
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// 保存路径
        /// </summary>
        [Required(ErrorMessage = "请选择图片并上传")]
        public override string SavePath { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public override FileType Type => FileType.WelcomeImg;

        public override DateTime? CreateTime { get; set; }

        [StringLength(300, ErrorMessage = "备注信息长度不能超过200个字")]
        public override string Description { get; set; }
    }
}
