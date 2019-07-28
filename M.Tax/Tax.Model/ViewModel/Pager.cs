using System.Collections.Generic;

namespace Tax.Model.ViewModel
{
    public class Pager<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 0;
                return Total % PageSize == 0 ? Total / PageSize : Total / PageSize + 1;
            }
        }
        public int Total { get; set; }
        public List<T> DataList { get; set; }
    }
}
