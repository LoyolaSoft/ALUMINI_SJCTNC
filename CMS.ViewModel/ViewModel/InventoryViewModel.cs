using CMS.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CMS.ViewModel.ViewModel
{
    public class InventoryViewModel
    {
        #region Properties
       
        #endregion

        #region  List
        public List<INVENT_DEPARTMENT> liinventdepartment { get; set; }
        public List<INVENT_ITEMS> liinventitems { get; set; }
        public List<INVENT_VENDOR_DETAILS> livendors { get; set; }
        public List<INVENT_PURCHASE_DETAILS> lipurchasedetails { get; set; }
        public List<INVENT_STOCK_REQUEST> listockrequest { get; set; }
        public List<INVENT_STOCK_DISTRIBUTION> listockdistribution { get; set; }



        #endregion

        #region Select List
        public SelectList ITEMS { get; set; }
        public SelectList DEPARTMENT { get; set; }
        public SelectList VENDOR { get; set; }
        public SelectList STATUS { get; set; }
       
        #endregion

        #region Objects

        #endregion
    }
}
