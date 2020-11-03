using System;
using System.Collections.Generic;
using System.Text;
using Tax.Model.Enum;

namespace Tax.Model.ParamModel
{
   public abstract  class SaveStaticFilesParamBase
    {
        /// <summary>
        /// ID
        /// </summary>
        public abstract int Id { get; set; }
 
        public abstract string Title { get; set; }
 
        public abstract int SortID { get; set; }

        public abstract string SavePath { get; set; }
 
        public abstract FileType Type { get;  }

        public abstract string Description { get; set; }

        public abstract DateTime? CreateTime { get; set; }
    }
}
