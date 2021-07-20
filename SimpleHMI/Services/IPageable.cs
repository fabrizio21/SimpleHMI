using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHMI.Services
{
    interface IPageable {

        int PageSize { get; set; }
        int CurrentPage { get; set; }
        int TotalPages { get; set; }
        bool CanMoveNext { get; set; }
        bool CanMovePrev { get; set; }

        void RecalculatePageItems();
    }
}
