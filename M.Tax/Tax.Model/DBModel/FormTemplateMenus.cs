namespace Tax.Model.DBModel
{
    public  class FormTemplateMenus:BaseDBModel
    {
        /// <summary>
        /// 菜单名/显示名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父级菜单
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 菜单下文件id数组
        /// </summary>
        public string FileIDs { get; set; }
        /// <summary>
        ///排序编号
        /// </summary>
        public int SortID { get; set; }

        public string Version { get; set; }

        public bool Enable { get; set; }
    }
}
