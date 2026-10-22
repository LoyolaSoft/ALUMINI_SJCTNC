using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModel.Model
{
    public class InventoryModel
    {

    }
    public class INVENT_DEPARTMENT
    {
        public string DEPARTMENT_ID { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }       
    }
    public class INVENT_PURCHASE_DETAILS
    {
        public string PURCHASE_ID { get; set; }
        public string INVOICE_NO { get; set; }
        public string INVOICE_DATE { get; set; }
        public string ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string VENDOR_ID { get; set; }
        public string VENDOR_NAME { get; set; }

        public string QUANTITY { get; set; }
        public string TOTAL_PRICE { get; set; }
        public string PURCHASE_DATE { get; set; }
        public string MODEL_NAME { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class INVENT_VENDOR_DETAILS
    {
        public string VENDOR_ID { get; set; }
        public string VENDOR_NAME { get; set; }
        public string MOBILE { get; set; }
        public string ADDRESS { get; set; }
        public string ORGANIZATION_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class INVENT_STOCK_DISTRIBUTION
    {

        public string STOCK_DESTRIBUTION_ID { get; set; }
        public string DEP_ID { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string STOCK_DIS_ID { get; set; }
        public string CREDIT { get; set; }
        public string VENDOR_ID { get; set; }
        public string VENDOR_NAME { get; set; }

        public string QUANTITY { get; set; }
        public string DATE { get; set; }
        public string IS_ACTIVE { get; set; }

        public string IS_DELETED { get; set; }
    }
    public class INVENT_ITEMS
    {
        public string ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class INVENT_STOCK_REQUEST
    {
        public string STOCK_REQUEST_ID { get; set; }
        public string DEP_ID { get; set; }
        public string STOCK_DIS_ID { get; set; }
        public string CREDIT { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string VENDOR_ID { get; set; }
        public string VENDOR_NAME { get; set; }
        public string APPROVED_QUANTITY { get; set; }
        public string QUANTITY { get; set; }
        public string STATUS { get; set; }
        public string STATUS_NAME { get; set; }
        public string REQUEST_DATE { get; set; }
     


        public string PURPOSE { get; set; }
        public string STAFF_ID { get; set; }
        public string ACADEMIC_YEAR { get; set; }  
        public string STOCK_DESTRIBUTION_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }
    }
    public class INVENT_STOCK_TRANSCAION
    {
        public string STOCK_TRANSACTION_ID { get; set; }
        public string STOCK_DIS_ID { get; set; }
        public string CREDIT { get; set; }
        public string DEBIT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }       
        public string STATUS { get; set; }    
    }
    public class SUP_INVENT_STATUS
    {
        public string STATUS_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_DELETED { get; set; }

    }

    public class JSON_ARRAY
    {
        public List<INVENT_PURCHASE_DETAILS> LipurchaseDetails { get; set; }
        public List<INVENT_STOCK_REQUEST> LiStockRequest { get; set; }

    }

}
