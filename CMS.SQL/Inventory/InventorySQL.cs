using CMS.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.SQL.Inventory
{
    public static class InventorySQL
    {
        public static string GetInventorySQL(InventoryEnumCommands sEnumCommand)
        {
            string query = string.Empty;
            switch (sEnumCommand)
            {
                #region Inventory Department
                case InventoryEnumCommands.FetchInventorydepartment:
                    {
                        query = @"SELECT 
                                DEPARTMENT_ID, DEPARTMENT_NAME
                            FROM
                                INVENT_DEPARTMENT
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case InventoryEnumCommands.FetchInventorydepartmentById:
                    {
                        query = @"SELECT 
                                    DEPARTMENT_ID, DEPARTMENT_NAME
                                FROM
                                    INVENT_DEPARTMENT
                                WHERE
                                    IS_DELETED != 1 AND DEPARTMENT_ID =?DEPARTMENT_ID;";
                        break;
                    }
                case InventoryEnumCommands.SaveInventoryDepartment:
                    {
                        query = @"INSERT INTO INVENT_DEPARTMENT(DEPARTMENT_NAME) VALUES(?DEPARTMENT_NAME);";
                        break;
                    }
                case InventoryEnumCommands.UpdateInventoryDepartment:
                    {
                        query = @"UPDATE INVENT_DEPARTMENT 
                                    SET 
                                        DEPARTMENT_NAME = ?DEPARTMENT_NAME
                                    WHERE
                                        DEPARTMENT_ID = ?DEPARTMENT_ID;";
                        break;
                    }
                case InventoryEnumCommands.DeleteInventorydepartment:
                    {
                        query = @"UPDATE INVENT_DEPARTMENT 
                                    SET 
                                        IS_DELETED = 1
                                    WHERE
                                        DEPARTMENT_ID = ?DEPARTMENT_ID;";
                        break;
                    }
                #endregion
                #region Inventory Items
                case InventoryEnumCommands.FetchInventoryItems:
                    {
                        query = @"SELECT 
                                ITEM_ID, ITEM_NAME
                            FROM
                                INVENT_ITEMS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case InventoryEnumCommands.FetchInventoryItemsById:
                    {
                        query = @"SELECT 
                                    ITEM_ID, ITEM_NAME
                                FROM
                                    INVENT_ITEMS
                                WHERE
                                    IS_DELETED != 1 AND ITEM_ID =?ITEM_ID;";
                        break;
                    }
                case InventoryEnumCommands.SaveInventoryItems:
                    {
                        query = @"INSERT INTO INVENT_ITEMS(ITEM_NAME) VALUES(?ITEM_NAME);";
                        break;
                    }
                case InventoryEnumCommands.UpdateInventoryItems:
                    {
                        query = @"UPDATE INVENT_ITEMS 
                        SET 
                            ITEM_NAME = ?ITEM_NAME
                        WHERE
                            ITEM_ID = ?ITEM_ID;";
                        break;
                    }
                case InventoryEnumCommands.DeleteInventoryItems:
                    {
                        query = @"UPDATE INVENT_ITEMS 
                                SET 
                                    IS_DELETED = 1
                                WHERE
                                    ITEM_ID = ?ITEM_ID;";
                        break;
                    }
                #endregion
                #region Invent Vendor Details
                case InventoryEnumCommands.FetchAllVendor:
                    {
                        query = @"SELECT 
                                VENDOR_ID, VENDOR_NAME, MOBILE, ADDRESS, ORGANIZATION_NAME
                            FROM
                                INVENT_VENDOR_DETAILS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case InventoryEnumCommands.FetchVendorById:
                    {
                        query = @"SELECT 
                                VENDOR_ID, VENDOR_NAME, MOBILE, ADDRESS, ORGANIZATION_NAME
                            FROM
                                INVENT_VENDOR_DETAILS
                            WHERE
                                IS_DELETED != 1 AND VENDOR_ID = ?VENDOR_ID;";
                        break;
                    }
                case InventoryEnumCommands.UpdateVendors:
                    {
                        query = @"UPDATE INVENT_VENDOR_DETAILS 
                                SET 
                                    VENDOR_NAME =?VENDOR_NAME,
                                    MOBILE =?MOBILE,
                                    ADDRESS =?ADDRESS,
                                    ORGANIZATION_NAME =?ORGANIZATION_NAME
                                WHERE
                                    VENDOR_ID = ?VENDOR_ID;";
                        break;
                    }
                case InventoryEnumCommands.SaveVendors:
                    {
                        query = @"INSERT INTO INVENT_VENDOR_DETAILS(
                                 VENDOR_NAME,
                                 MOBILE,
                                 ADDRESS, ORGANIZATION_NAME)
                                 VALUES(
                                 ?VENDOR_NAME,
                                 ?MOBILE,
                                 ?ADDRESS, 
                                 ?ORGANIZATION_NAME);";
                        break;
                    }
                case InventoryEnumCommands.DeleteVendors:
                    {
                        query = @"UPDATE INVENT_VENDOR_DETAILS 
                                    SET 
                                        IS_DELETED = 1
                                    WHERE
                                        VENDOR_ID = ?VENDOR_ID;";
                        break;
                    }

                #endregion
                #region Invent Purchase  Details
                case InventoryEnumCommands.FetchPurchaseDetails:
                    {
                        query = @"SELECT 
                                PD.PURCHASE_ID,
                                PD.INVOICE_NO,
                                DATE_FORMAT(PD.INVOICE_DATE, '%Y/%m/%d') AS INVOICE_DATE,
                                DATE_FORMAT(PD.PURCHASE_DATE, '%Y/%m/%d') AS PURCHASE_DATE,
                                II.ITEM_NAME,
                                IV.VENDOR_NAME,
                                PD.VENDOR_ID,
                                PD.ITEM_ID,
                                PD.MODEL_NAME,
                                PD.QUANTITY,
                                PD.TOTAL_PRICE
                            FROM
                                INVENT_PURCHASE_DETAILS AS PD
                                    INNER JOIN
                                INVENT_ITEMS AS II ON PD.ITEM_ID = II.ITEM_ID
                                    INNER JOIN
                                INVENT_VENDOR_DETAILS AS IV ON IV.VENDOR_ID = PD.VENDOR_ID
                            WHERE
                                PD.IS_DELETED != 1
                                    AND PD.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case InventoryEnumCommands.FetchPurchaseDetailsById:
                    {
                        query = @"SELECT 
                                PD.PURCHASE_ID,
                                PD.INVOICE_NO,
                                DATE_FORMAT(PD.INVOICE_DATE, '%d/%m/%Y') AS INVOICE_DATE,
                                DATE_FORMAT(PD.PURCHASE_DATE, '%d/%m/%Y') AS PURCHASE_DATE,
                                II.ITEM_NAME,
                                IV.VENDOR_NAME,
                                PD.VENDOR_ID,
                                PD.ITEM_ID,
                                PD.MODEL_NAME,
                                PD.QUANTITY,
                                PD.TOTAL_PRICE
                            FROM
                                INVENT_PURCHASE_DETAILS AS PD
                                    INNER JOIN
                                INVENT_ITEMS AS II ON PD.ITEM_ID = II.ITEM_ID
                                    INNER JOIN
                                INVENT_VENDOR_DETAILS AS IV ON IV.VENDOR_ID = PD.VENDOR_ID
                            WHERE
                                PD.IS_DELETED != 1 AND PURCHASE_ID = ?PURCHASE_ID
                                    AND PD.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case InventoryEnumCommands.UpdatePurchaseDetails:
                    {
                        query = @"UPDATE INVENT_PURCHASE_DETAILS 
                                    SET 
                                        INVOICE_NO = ?INVOICE_NO,
                                        INVOICE_DATE = ?INVOICE_DATE,
                                        ITEM_ID = ?ITEM_ID,
                                        VENDOR_ID = ?VENDOR_ID,
                                        QUANTITY = ?QUANTITY,
                                        TOTAL_PRICE = ?TOTAL_PRICE,
                                        PURCHASE_DATE =?PURCHASE_DATE,
                                        MODEL_NAME = ?MODEL_NAME
                                    WHERE
                                        PURCHASE_ID =?PURCHASE_ID;";
                        break;
                    }
                case InventoryEnumCommands.SavePurchaseDetails:
                    {
                        query = @"INSERT INTO 
                                INVENT_PURCHASE_DETAILS(INVOICE_NO, 
                                INVOICE_DATE, ITEM_ID,
                                 VENDOR_ID, QUANTITY,
                                 TOTAL_PRICE, 
                                 PURCHASE_DATE, MODEL_NAME)
                                VALUES(?INVOICE_NO,
                                ?INVOICE_DATE, ?ITEM_ID,
                                 ?VENDOR_ID, ?QUANTITY,
                                 ?TOTAL_PRICE, 
                                 ?PURCHASE_DATE, ?MODEL_NAME);";
                        break;
                    }
                case InventoryEnumCommands.DeletePurchaseDetails:
                    {
                        query = @"UPDATE INVENT_PURCHASE_DETAILS 
                                SET 
                                    IS_DELETED = 1
                                WHERE
                                    PURCHASE_ID = ?PURCHASE_ID;";
                        break;
                    }
                case InventoryEnumCommands.FetchInventItems:
                    {
                        query = @"SELECT 
                                ITEM_ID, ITEM_NAME
                            FROM
                                INVENT_ITEMS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case InventoryEnumCommands.FetchInventVendors:
                    {
                        query = @"SELECT 
                                VENDOR_ID, VENDOR_NAME
                            FROM
                                INVENT_VENDOR_DETAILS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                #endregion
                #region Inventory Stock Request and Approval
                case InventoryEnumCommands.FetchStockRequestByUserId:
                    {
                        query = @"SELECT 
                                SR.STOCK_REQUEST_ID,
                                SR.DEP_ID,
                                SR.QUANTITY,
                                SR.ITEM_ID,
                                II.ITEM_NAME,
                                DEP.DEPARTMENT_NAME,
                                SR.PURPOSE,
                                SR.APPROVED_QUANTITY,
                                SR.STATUS,
                                SS.STATUS_NAME
                            FROM
                                INVENT_STOCK_REQUEST AS SR
                                    INNER JOIN
                                INVENT_DEPARTMENT AS DEP ON DEP.DEPARTMENT_ID = SR.DEP_ID
                                    INNER JOIN
                                INVENT_ITEMS AS II ON II.ITEM_ID = SR.ITEM_ID
                                    LEFT JOIN
                                SUP_INVENT_STATUS AS SS ON SS.STATUS_ID = SR.STATUS
                            WHERE
                                SR.IS_DELETED != 1
                                    AND SR.ACADEMIC_YEAR = ?AC_YEAR
                                    AND SR.STAFF_ID = ?STAFF_ID;";
                        break;
                    }
                case InventoryEnumCommands.FetchStockRequestById:
                    {
                        query = @"SELECT 
                                SR.STOCK_REQUEST_ID,
                                SR.DEP_ID,
                                SR.QUANTITY,
                                SR.ITEM_ID,
                                II.ITEM_NAME,
                                DEP.DEPARTMENT_NAME,
                                SR.PURPOSE,
                                SR.APPROVED_QUANTITY,
                                SR.STATUS,
                                SS.STATUS_NAME
                            FROM
                                INVENT_STOCK_REQUEST AS SR
                                    INNER JOIN
                                INVENT_DEPARTMENT AS DEP ON DEP.DEPARTMENT_ID = SR.DEP_ID
                                    INNER JOIN
                                INVENT_ITEMS AS II ON II.ITEM_ID = SR.ITEM_ID
                                    LEFT JOIN
                                SUP_INVENT_STATUS AS SS ON SS.STATUS_ID = SR.STATUS
                            WHERE
                                SR.IS_DELETED != 1
                                    AND SR.ACADEMIC_YEAR = ?AC_YEAR
                                    AND STOCK_REQUEST_ID=?STOCK_REQUEST_ID;";
                        break;
                    }
                case InventoryEnumCommands.SaveStockRequest:
                    {
                        query = @"INSERT INTO INVENT_STOCK_REQUEST(DEP_ID, ITEM_ID,
                                 QUANTITY, ACADEMIC_YEAR, 
                                PURPOSE, STAFF_ID) 
                                VALUES(?DEP_ID, ?ITEM_ID, ?QUANTITY,
                                 ?ACADEMIC_YEAR, ?PURPOSE,
                                 ?STAFF_ID);";
                        break;
                    }
                case InventoryEnumCommands.UpdateStockRequest:
                    {
                        query = @"UPDATE INVENT_STOCK_REQUEST 
                                SET 
                                    DEP_ID =?DEP_ID,
                                    ITEM_ID =?ITEM_ID,
                                    QUANTITY =?QUANTITY,
                                    ACADEMIC_YEAR =?ACADEMIC_YEAR,
                                    PURPOSE =?PURPOSE,
                                    STAFF_ID =?STAFF_ID
                                WHERE
                                    STOCK_REQUEST_ID =?STOCK_REQUEST_ID;";
                        break;
                    }
                case InventoryEnumCommands.DeleteStockRequest:
                    {
                        query = @"
                                UPDATE INVENT_STOCK_REQUEST 
                                SET 
                                    IS_DELETED = 1
                                WHERE
                                    STOCK_REQUEST_ID = ?STOCK_REQUEST_ID;";
                        break;
                    }
                case InventoryEnumCommands.FetchStockrequestforApproval:
                    {
                        query = @"SELECT 
                                SR.STOCK_REQUEST_ID,
                                SR.DEP_ID,
                                SR.QUANTITY,
                                SR.ITEM_ID,
                                II.ITEM_NAME,
                                DEP.DEPARTMENT_NAME,
                                SR.PURPOSE,
                                SR.APPROVED_QUANTITY,
                                SR.STATUS,
                                SS.STATUS_NAME
                            FROM
                                INVENT_STOCK_REQUEST AS SR
                                    INNER JOIN
                                INVENT_DEPARTMENT AS DEP ON DEP.DEPARTMENT_ID = SR.DEP_ID
                                    INNER JOIN
                                INVENT_ITEMS AS II ON II.ITEM_ID = SR.ITEM_ID
                                    LEFT JOIN
                                SUP_INVENT_STATUS AS SS ON SS.STATUS_ID = SR.STATUS
                            WHERE
                                SR.IS_DELETED != 1
                                    AND SR.ACADEMIC_YEAR = ?AC_YEAR;";
                        break;
                    }
                case InventoryEnumCommands.UpdateStockRequestApproval:
                    {
                        query = @"UPDATE INVENT_STOCK_REQUEST 
                                SET 
                                    APPROVED_QUANTITY =?APPROVED_QUANTITY,
                                    STATUS =?STATUS
                                WHERE
                                    STOCK_REQUEST_ID =?STOCK_REQUEST_ID;";
                        break;
                    }
                case InventoryEnumCommands.FetchInventDepartment:
                    {
                        query = @"SELECT 
                                DEPARTMENT_ID, DEPARTMENT_NAME
                            FROM
                                INVENT_DEPARTMENT
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }
                case InventoryEnumCommands.FetchSupInventStatus:
                    {
                        query = @"SELECT 
                                STATUS_ID,STATUS_NAME
                            FROM
                                SUP_INVENT_STATUS
                            WHERE
                                IS_DELETED != 1;";
                        break;
                    }

                #endregion
                #region Invent Stock Distribution
                case InventoryEnumCommands.FetchStockDestributed:
                    {
                        query = @"SELECT 
                                SD.STOCK_DESTRIBUTION_ID,
                                SD.DEP_ID,
                                DEP.DEPARTMENT_NAME,
                                SD.ITEM_ID,
                                II.ITEM_NAME,
                                SD.VENDOR_ID,
                                VD.VENDOR_NAME,
                                SD.QUANTITY,
                                DATE_FORMAT(SD.DATE, '%Y/%m/%d') AS DATE
                            FROM
                                INVENT_STOCK_DISTRIBUTION AS SD
                                    INNER JOIN
                                INVENT_DEPARTMENT AS DEP ON DEP.DEPARTMENT_ID = SD.DEP_ID
                                    INNER JOIN
                                INVENT_ITEMS AS II ON II.ITEM_ID = SD.ITEM_ID
                                    INNER JOIN
                                INVENT_VENDOR_DETAILS AS VD ON VD.VENDOR_ID = SD.VENDOR_ID
                            WHERE
                                SD.IS_DELETED != 1;";
                        break;
                    }
                case InventoryEnumCommands.FetchAllStockRequested:
                    {
                        query = @"SELECT 
                                SR.DEP_ID,
                                DEP.DEPARTMENT_NAME,
                                II.ITEM_NAME,
                                SR.ITEM_ID,
                                SR.APPROVED_QUANTITY,
                                SR.STATUS,
                                SS.STATUS_NAME,
                                DATE_FORMAT(SR.REQUEST_DATE, '%Y/%m/%d') AS REQUEST_DATE,
                                SR.STOCK_REQUEST_ID
                            FROM
                                INVENT_STOCK_REQUEST AS SR
                                    INNER JOIN
                                INVENT_DEPARTMENT AS DEP ON DEP.DEPARTMENT_ID = SR.DEP_ID
                                    INNER JOIN
                                INVENT_ITEMS AS II ON II.ITEM_ID = SR.ITEM_ID
                                    INNER JOIN
                                SUP_INVENT_STATUS AS SS ON SS.STATUS_ID = SR.STATUS
                                    LEFT JOIN
                                INVENT_STOCK_DISTRIBUTION AS SD ON SD.STOCK_REQUEST_ID = SR.STOCK_REQUEST_ID
                            WHERE
                                SR.STATUS = 1 AND SR.IS_DELETED != 1
                                    AND SD.STOCK_DESTRIBUTION_ID IS NULL;";
                        break;
                    }

                case InventoryEnumCommands.FetchStockDistributionById:
                    {
                        query = @"SELECT 
                                    SD.STOCK_DESTRIBUTION_ID,
                                    SD.DEP_ID,
                                    DEP.DEPARTMENT_NAME,
                                    SD.ITEM_ID,
                                    II.ITEM_NAME,
                                    SD.VENDOR_ID,
                                    VD.VENDOR_NAME,
                                    SD.QUANTITY,
                                    DATE_FORMAT(SD.DATE, '%Y/%m/%d') AS DATE
                                FROM
                                    INVENT_STOCK_DISTRIBUTION AS SD
                                        INNER JOIN
                                    INVENT_DEPARTMENT AS DEP ON DEP.DEPARTMENT_ID = SD.DEP_ID
                                        INNER JOIN
                                    INVENT_ITEMS AS II ON II.ITEM_ID = SD.ITEM_ID
                                        INNER JOIN
                                    INVENT_VENDOR_DETAILS AS VD ON VD.VENDOR_ID = SD.VENDOR_ID
                                WHERE
                                    SD.IS_DELETED != 1
                                        AND SD.STOCK_DESTRIBUTION_ID = ?STOCK_DESTRIBUTION_ID;";
                        break;
                    }
                case InventoryEnumCommands.FetchAvailableStock:
                    {
                        query = @"SELECT 
                                SUM(PD.QUANTITY) - IFNULL(SUM(SD.QUANTITY), 0) AS QUANTITY
                            FROM
                                INVENT_PURCHASE_DETAILS AS PD
                                    LEFT JOIN
                                INVENT_STOCK_DISTRIBUTION AS SD ON SD.ITEM_ID = PD.ITEM_ID
                                    AND SD.VENDOR_ID = PD.VENDOR_ID
                            WHERE
                                PD.IS_DELETED != 1 AND PD.ITEM_ID = ?ITEM_ID
                                    AND PD.VENDOR_ID = ?VENDOR_ID
                            GROUP BY PD.ITEM_ID , PD.VENDOR_ID;";
                        break;
                    }
                case InventoryEnumCommands.SaveStockDistribution:
                    {
                        query = @"INSERT INTO  
                                INVENT_STOCK_DISTRIBUTION(DEP_ID, ITEM_ID, VENDOR_ID, QUANTITY,STOCK_REQUEST_ID) 
                                VALUES(?DEP_ID, ?ITEM_ID, ?VENDOR_ID, ?QUANTITY,?STOCK_REQUEST_ID);";
                        break;
                    }
                case InventoryEnumCommands.SaveStockTranscation:
                    {
                        query = @"INSERT INTO 
                                INVENT_STOCK_TRANSCAION(STOCK_DIS_ID, CREDIT) 
                                VALUES(?STOCK_DIS_ID, ?CREDIT);";
                        break;
                    }
                case InventoryEnumCommands.UpdateStockDistribution:
                    {
                        query = @"UPDATE INVENT_STOCK_DISTRIBUTION 
                                SET 
                                    DEP_ID = ?DEP_ID,
                                    ITEM_ID = ?ITEM_ID,
                                    VENDOR_ID =?VENDOR_ID,
                                    QUANTITY = ?QUANTITY
                                WHERE
                                    STOCK_DESTRIBUTION_ID = ?STOCK_DESTRIBUTION_ID;";
                        break;
                    }
                case InventoryEnumCommands.UpdateStockTranscation:
                    {
                        query = @"UPDATE INVENT_STOCK_TRANSCAION 
                                    SET 
                                        CREDIT =?CREDIT
                                    WHERE
                                        STOCK_DIS_ID =?STOCK_DIS_ID;";
                        break;
                    }
                case InventoryEnumCommands.FetchVendorItemsDetails:
                    {
                        query = @"SELECT 
                                SUM(PD.QUANTITY) - IFNULL(SUM(SD.QUANTITY), 0) AS QUANTITY,
                                PD.VENDOR_ID,
                                VD.VENDOR_NAME,
                                II.ITEM_NAME,
                                PD.ITEM_ID
                            FROM
                                INVENT_PURCHASE_DETAILS AS PD
                                    INNER JOIN
                                INVENT_ITEMS AS II ON II.ITEM_ID = PD.ITEM_ID
                                    INNER JOIN
                                INVENT_VENDOR_DETAILS AS VD ON VD.VENDOR_ID = PD.VENDOR_ID
                                    LEFT JOIN
                                INVENT_STOCK_DISTRIBUTION AS SD ON SD.ITEM_ID = PD.ITEM_ID
                                    AND SD.VENDOR_ID = PD.VENDOR_ID
                            WHERE
                                PD.IS_DELETED != 1 AND PD.ITEM_ID = ?ITEM_ID
                            GROUP BY PD.ITEM_ID , PD.VENDOR_ID;";
                        break;
                    }
                    #endregion

            }
            return query;
        }
    }
}
