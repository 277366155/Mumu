namespace Tax.Model.DBModel
{
    public  class SystemSettings: BaseDBModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
    }
}
