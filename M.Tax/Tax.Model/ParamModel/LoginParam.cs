using System.ComponentModel.DataAnnotations;

namespace Tax.Model.ParamModel
{
    public class LoginParam
    {
        [Required(ErrorMessage = "登录账号不能为空")]
        [StringLength(32,ErrorMessage ="账号最大长度不能超过32位")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "登录密码不能为空")]
        [StringLength(32,ErrorMessage ="密码最大长度不能超过32位")]
        public string Password { get; set; }
    }
}
