using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tax.Model.Enum;

namespace Tax.Model.ParamModel
{
    public class SaveMenuIconParam : SaveStaticFilesParamBase
    {
        public override int Id { get; set; }
        public override string Title
        {
            get { return "pc菜单图标"; }
            set { }
        }
        public override int SortID
        {
            get { return 0; }
            set { }
        }
        [Required(ErrorMessage = "请选择图片")]
        public override string SavePath { get; set; }

        public override FileType Type => FileType.MenuIcon;

        public override string Description { get; set; }

        public override DateTime? CreateTime { get; set; }
    }
}
