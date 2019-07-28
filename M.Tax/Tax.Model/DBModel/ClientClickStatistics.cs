using Tax.Model.Enum;

namespace Tax.Model.DBModel
{
    public class ClientClickStatistics : BaseDBModel
    {
        /// <summary>
        /// 菜单或表单id
        /// </summary>
        public int ClickID { get; set; }  
        /// <summary>
        /// 菜单链接地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 点击类型
        /// </summary>
        public ClickTypeEnum ClickType { get; set; }
        /// <summary>
        /// 点击数量
        /// </summary>
        public long Hits { get; set; }       
    }
}
