using _2whealers.Helpers;
using _2whealers.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace _2whealers.Controllers
{
    public class BranchController : Controller
    {
        private readonly SqlHelper _sqlHelper;
        private readonly IWebHostEnvironment _env;

        public BranchController(SqlHelper sqlHelper, IWebHostEnvironment env)
        {
            _env = env;
            _sqlHelper = sqlHelper;
        }
        public IActionResult BranchDetails(int branchId)
        {
            var model = new BranchDetailsViewModel
            {
                BranchId = branchId,
            };

            SqlParameter[] parameters = {
        new SqlParameter("@regionId", branchId)
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_GetTransactionCounts", parameters);

            if (dt.Rows.Count > 0)
            {
                model.PreSalesCount = Convert.ToInt32(dt.Rows[0]["PreSaleCount"]);
                model.SalesCount = Convert.ToInt32(dt.Rows[0]["SaleCount"]);
            }
            else
            {
                model.PreSalesCount = 0;
                model.SalesCount = 0;
            }

            return View(model);
        }



        public IActionResult DownloadSales(int branchId)
        {  
            SqlParameter[] parameters = {
                new SqlParameter("@calltype", "GetAllSales"),
                new SqlParameter("@regionid", branchId)
            };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Sales", parameters);

            var salesList = dt.AsEnumerable()
                .Select(row => new TransactionViewModel
                {
                    Name = row["Name"].ToString(),
                    MobileNo = row["Mobile"].ToString(),
                    Id = Convert.ToInt32(row["TransactionId"]),
                    ModelName = row["ModelName"].ToString(),
                    dateofsale = Convert.ToDateTime(row["DateOfSale"]),
                    Downpayment = Convert.ToDecimal(row["DownPayment"]),
                    CashAmount = Convert.ToDecimal(row["EMIAmount"]),
                    DateOfSaleReminder = Convert.ToDateTime(row["ServicingReminderDate"])
                }).ToList();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Sales Report");
                 
                ws.Cell(1, 2).Value = "Name";
                ws.Cell(1, 3).Value = "Mobile";
                ws.Cell(1, 4).Value = "Model Name";
                ws.Cell(1, 5).Value = "Date of Sale";
                ws.Cell(1, 6).Value = "Downpayment";
                ws.Cell(1, 7).Value = "EMI Amount";
                ws.Cell(1, 8).Value = "Servicing Reminder";

                int row = 2;
                foreach (var item in salesList)
                { 
                    ws.Cell(row, 2).Value = item.Name;
                    ws.Cell(row, 3).Value = item.MobileNo;
                    ws.Cell(row, 4).Value = item.ModelName;
                    ws.Cell(row, 5).Value = item.dateofsale;
                    ws.Cell(row, 6).Value = item.Downpayment;
                    ws.Cell(row, 7).Value = item.CashAmount;
                    ws.Cell(row, 8).Value = item.DateOfSaleReminder;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "SalesReport.xlsx");
                }
                 
            }
        }

        public IActionResult DownloadPreSales(int branchId)
        {
            var RegionId = User.FindFirst("RegionId")?.Value;

            SqlParameter[] parameters = {
                new SqlParameter("@calltype", "GetAllPreSales"),
                new SqlParameter("@id", RegionId)
            };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            var userTransactionList = dt.AsEnumerable().Select(row => new PurchaseViewModel
            {

                Name = row["Name"].ToString(),
                MobileNo = row["Mobile"].ToString(),
                Id = Convert.ToInt32(row["transactionid"]),
                ModelName = row["ModelName"].ToString(),
                CashAmount = Convert.ToDecimal(row["CashAmount"]),
                IsActive = Convert.ToBoolean(row["IsActive"])

            }).OrderByDescending(x => x.Id).ToList();

            return View(userTransactionList);
        }
    }
}
