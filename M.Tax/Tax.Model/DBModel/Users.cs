namespace Tax.Model.DBModel
{
    public class Users: BaseDBModel
    {
        public string UserName{ get; set; }
        public string Password{ get; set; }
        public string Email { get; set; }
        public bool Enable { get; set; }
    }
}
