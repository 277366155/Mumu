using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tax.Model.ParamModel
{
   public  class InsertUserParam:LoginParam
    {
        public string Email { get; set; }
    }
}
