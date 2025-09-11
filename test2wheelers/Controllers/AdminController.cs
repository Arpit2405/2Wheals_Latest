using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;
using System.Data;
using _2whealers.Models;
using Microsoft.AspNetCore.Authorization;
using Rotativa.AspNetCore;
using System.Transactions;
using test2wheelers.Helpers;

namespace test2wheelers.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly SqlHelper _sqlHelper;
        private readonly IWebHostEnvironment _env;

        public AdminController(SqlHelper sqlHelper, IWebHostEnvironment env)
        {
            _env = env;
            _sqlHelper = sqlHelper;
        }


        public ActionResult Dashboard()
        {
            var counts = GetCounts();
            ViewBag.PreSaleCount = counts.PreSaleCount;
            ViewBag.SaleCount = counts.SaleCount;
            ViewBag.TotalVisitors = counts.SaleCount;

            return View();
        }

        /////// Brands /////////////////////////////////////////////////////////////////////
        public ActionResult Brands()
        {
            List<Brands> brands = new List<Brands>();

            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "GetAllBrands")
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            foreach (DataRow row in dt.Rows)
            {
                brands.Add(new Brands
                {
                    Id = Convert.ToInt32(row["id"]),
                    BrandName = row["BrandName"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                });
            }

            return View(brands);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBrands(Brands brand)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid input. Please try again.";
                return RedirectToAction("Brands");
            }

            try
            {
                SqlParameter[] parameters =
                {
            new SqlParameter("@BrandName", brand.BrandName),
            new SqlParameter("@calltype", "InsertBrands")
        };

                // Execute SP and get result as DataTable
                var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    int result = Convert.ToInt32(row["Result"]);
                    string message = row["Message"].ToString();

                    if (result == -1)
                        TempData["Error"] = message;
                    else
                        TempData["Success"] = message;
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong while saving.";
            }

            return RedirectToAction("Brands");
        }

        public ActionResult EditBrands(int id)
        {
            Brands brand = null;

            SqlParameter[] parameters = {
        new SqlParameter("@id", id),
        new SqlParameter("@calltype", "GetBrandsById")
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                brand = new Brands
                {
                    Id = Convert.ToInt32(row["Id"]),
                    BrandName = row["BrandName"].ToString()
                };
            }

            return PartialView("_EditBrandsModal", brand);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBrands(Brands model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditBrandsModal", model);
            }

            try
            {
                SqlParameter[] parameters = {
            new SqlParameter("@id", model.Id),
            new SqlParameter("@BrandName", model.BrandName),
            new SqlParameter("@calltype", "UpdateBrands")
        };

                _sqlHelper.ExecuteNonQuery("sp_Users", parameters);

                TempData["Success"] = "Brand updated successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong while updating.";
            }

            return RedirectToAction("Brands");
        }

        public ActionResult DeleteBrands(int id)
        {
            try
            {
                SqlParameter[] parameters = {
            new SqlParameter("@id", id),
            new SqlParameter("@calltype", "DeleteBrands")
        };

                _sqlHelper.ExecuteNonQuery("sp_Users", parameters);

                TempData["Success"] = "Brand deleted successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Error occurred while deleting Brand.";
            }

            return RedirectToAction("Brands");
        }


        /////// Others /////////////////////////////////////////////////////////////////////


        private List<SelectListItem> GetBrands()
        {
            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "GetAllBrands")
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            var brands = dt.AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["BrandName"].ToString()
                }).ToList();

            return brands;
        }
        public DashboardCounts GetCounts()
        {
            var counts = new DashboardCounts();

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_GetTransactionCounts");

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                counts.PreSaleCount = Convert.ToInt32(row["PreSaleCount"]);
                counts.SaleCount = Convert.ToInt32(row["SaleCount"]);

            }
            else
            {
                counts.PreSaleCount = 0;
                counts.SaleCount = 0;
            }
            return counts;
        }

        public ActionResult LoadBrandDropdown()
        {
            var model = new BikeModels();

            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "GetAllBike")
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            foreach (DataRow row in dt.Rows)
            {
                model.BrandList.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["BrandName"].ToString()
                });
            }

            return PartialView("_AddBikeModelsModal", model);
        }

        [HttpGet]
        public ActionResult AddModelPartial()
        {
            var model = new BikeModels
            {
                BrandList = GetBrands()
            };

            return PartialView("_AddBikeModelsModal", model);
        }

        private List<SelectListItem> GetBikeModels()
        {
            var bikeModels = new List<SelectListItem>();

            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "GetAllBikeModels")
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            foreach (DataRow row in dt.Rows)
            {
                bikeModels.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["ModelName"].ToString()
                });
            }

            return bikeModels;
        }

        [HttpGet]
        public IActionResult GetModelsByBike(int bikeId)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "GetAllBikeModelsById"),
        new SqlParameter("@brand_Id_Fk", bikeId)
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            var models = dt.AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Value = row["id"].ToString(),
                    Text = row["ModelName"].ToString()
                }).ToList();

            return Json(models);
        }

        private List<SelectListItem> GetBankList()
        {
            SqlParameter[] parameters = { new SqlParameter("@calltype", "GetAllBanks") };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            return dt.AsEnumerable()
                     .Select(row => new SelectListItem
                     {
                         Value = row["BankName"].ToString(),
                         Text = row["BankName"].ToString()
                     }).ToList();
        }


        /////// Models /////////////////////////////////////////////////////////////////////

        public ActionResult Models()
        {
            var bikeModels = new List<BikeModels>();

            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "GetAllBrandsModels")
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            foreach (DataRow row in dt.Rows)
            {
                bikeModels.Add(new BikeModels
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ModelName = row["ModelName"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    BrandName = row["BrandName"].ToString()
                });
            }

            return View(bikeModels);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddModels(BikeModels model)
        {
            SqlParameter[] parameters = {
            new SqlParameter("@calltype", "InsertModelName"),
            new SqlParameter("@Brand_Id_Fk", model.Brand_Id_Fk),
            new SqlParameter("@ModelName", model.ModelName)
        };

            _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);
            return Json(new { success = true, message = "Model saved successfully!" });
        }


        public ActionResult EditModels(int id)
        {
            BikeModels model = null;

            SqlParameter[] parameters = {
        new SqlParameter("@id", id),
        new SqlParameter("@calltype", "GetModelById")
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                model = new BikeModels
                {
                    Id = Convert.ToInt32(row["Id"]),
                    BrandName = row["BrandName"].ToString(),
                    ModelName = row["ModelName"].ToString()
                };
            }

            return PartialView("_EditBikeModelsModal", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditModels(BikeModels model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditModelsModal", model);
            }

            try
            {
                SqlParameter[] parameters = {
            new SqlParameter("@id", model.Id),
            new SqlParameter("@ModelName", model.ModelName),
            new SqlParameter("@calltype", "UpdateModel")
        };

                _sqlHelper.ExecuteNonQuery("sp_Users", parameters);

                TempData["Success"] = "Model updated successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong while updating.";
            }

            return RedirectToAction("Models");
        }

        public ActionResult DeleteModels(int id)
        {
            try
            {
                SqlParameter[] parameters = {
            new SqlParameter("@id", id),
            new SqlParameter("@calltype", "DeleteModel")
        };

                _sqlHelper.ExecuteNonQuery("sp_Users", parameters);

                TempData["Success"] = "Model deleted successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Error occurred while deleting Model.";
            }

            return RedirectToAction("Models");
        }



        /////// Pre-Sales /////////////////////////////////////////////////////////////////////

        public ActionResult PreSales()
        {
            SqlParameter[] parameters = { new SqlParameter("@calltype", "GetAllPreSales") };

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

        public ActionResult AddPreSale()
        {
            var model = new PurchaseViewModel();

            ViewBag.BankList = GetBankList();
            ViewBag.BikeList = GetBrands();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPreSale(PurchaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();

                TempData["Errors"] = errors;
                return View("PreSales", model);
            }
            SqlParameter[] parameters = {
            new SqlParameter("@calltype", "InsertPreSales"),
            new SqlParameter("@userName", model.Name ),
            new SqlParameter("@MobileNo", model.MobileNo ),
            new SqlParameter("@Address", model.Address1 ),
            new SqlParameter("@ModelId", model.ModelId),
            new SqlParameter("@BikeId", model.BikeId),
            new SqlParameter("@VehicleNo", (model.RegNo).ToUpper()),
            new SqlParameter("@DateOfSale", model.dateofsale ),
            new SqlParameter("@ChassisNo", model.ChassisNo ),
            new SqlParameter("@EngineNo", model.EngineNo ),
            new SqlParameter("@CashAmount", Convert.ToDecimal(model.CashAmount)),
            new SqlParameter("@InsuranceValid", model.InsuranceValid ),
            new SqlParameter("@BankName", model.FinancedBy ),
            new SqlParameter("@NocValid", model.NocValid ),
        };

            _sqlHelper.ExecuteNonQuery("sp_Users", parameters);

            TempData["Success"] = "Pre-sale added successfully!";
            return RedirectToAction("PreSales");
        }


        public ActionResult PreSaleDetails(int id)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "GetPreSaleById"),
        new SqlParameter("@id", id)
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            var model = new PurchaseViewModel();

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                model.Id = Convert.ToInt32(row["TransactionId"]);
                model.Name = row["Name"].ToString();
                model.MobileNo = row["Mobile"].ToString();
                model.Address1 = row["Address1"].ToString();
                model.dateofsale = Convert.ToDateTime(row["DateOfSale"]);
                model.RegNo = row["vehicleno"].ToString();
                model.ChassisNo = row["ChassisNo"].ToString();
                model.EngineNo = row["EngineNo"].ToString();
                model.CashAmount = Convert.ToDecimal(row["CashAmount"]);
                model.ModelName = row["ModelName"].ToString();
                model.FinancedBy = row["BankName"].ToString();
            }

            return View("PreSaleDetails", model);
        }


        /////// Sales /////////////////////////////////////////////////////////////////////
        public ActionResult Sales()
        {
            SqlParameter[] parameters = { new SqlParameter("@calltype", "GetAllSales") };

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

            return View(salesList);
        }


        public ActionResult AddSales()
        {
            var model = new TransactionViewModel();
            ViewBag.BankList = GetBankList();
            ViewBag.BikeList = GetBrands();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSales(TransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();

                TempData["Errors"] = errors;
                return View("Sales", model);
            }

            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "InsertSales"),
        new SqlParameter("@Name", model.Name),
        new SqlParameter("@MobileNo", model.MobileNo),
        new SqlParameter("@Address1", model.Address1 ),
        new SqlParameter("@Address2", model.Address2 ),
        new SqlParameter("@DateOfBirth", model.DateOfBirth),
        new SqlParameter("@Email", model.Email ),
        new SqlParameter("@Location", model.Location ),
        new SqlParameter("@Pincode", model.Pincode ),
        new SqlParameter("@State", model.State ),
        new SqlParameter("@City", model.City ),
        new SqlParameter("@BikeId", model.BikeId),
        new SqlParameter("@ChassisNo", model.ChassisNo ),
        new SqlParameter("@DRM", model.DRM),
        new SqlParameter("@DateOfSale", model.dateofsale),
        new SqlParameter("@DownPayment", model.Downpayment),
        new SqlParameter("@EMIAmount", model.CashAmount),
        new SqlParameter("@EngineNo", model.EngineNo ),
        new SqlParameter("@FinancedBy", model.FinancedBy ),
        new SqlParameter("@InsuranceDueDate", model.InsuranceDuDate),
        new SqlParameter("@MR", model.MeterReading),
        new SqlParameter("@ManufacturingYear", model.ManufacturingYear.ToString()),
        new SqlParameter("@ModelId", model.ModelId),
        new SqlParameter("@NoOfInstallments", model.NoOfInstallments),
        new SqlParameter("@Notes", model.Notes ),
        new SqlParameter("@RegNo", model.RegNo ),
        new SqlParameter("@IsRegularService", model.IsRegularService),
        new SqlParameter("@ServicingReminderDate", model.dateofsale)
    };

            _sqlHelper.ExecuteNonQuery("sp_Sales", parameters);

            TempData["Success"] = "Sales added successfully!";
            return RedirectToAction("Sales");

        }



        private TransactionViewModel GetSaleById(int id)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "GetSaleById"),
        new SqlParameter("@id", id)
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Sales", parameters);

            var model = new TransactionViewModel();

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                model.Id = Convert.ToInt32(row["TransactionId"]);
                model.Name = row["Name"].ToString();
                model.MobileNo = row["Mobile"].ToString();
                model.ModelName = row["ModelName"].ToString();
                model.Address1 = row["Address1"].ToString();
                model.dateofsale = Convert.ToDateTime(row["DateOfSale"]);
                model.RegNo = row["RegNo"].ToString();
                model.ChassisNo = row["ChassisNo"].ToString();
                model.EngineNo = row["EngineNo"].ToString();
                model.DRM = Convert.ToInt32(row["DRM"]);
                model.FinancedBy = row["FinancedBy"].ToString();
                model.NoOfInstallments = Convert.ToInt32(row["NoOfInstallments"]);
                model.CashAmount = Convert.ToDecimal(row["EMIAmount"]);
                model.Notes = row["Notes"].ToString();
                model.Downpayment = Convert.ToDecimal(row["Downpayment"].ToString());
                model.DateOfSaleReminder = Convert.ToDateTime(row["ServicingReminderDate"]);
                model.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
            }

            return model;
        }

        public ActionResult SaleDetails(int id)
        {
            var model = GetSaleById(id);
            return View("SaleDetails", model);
        }
        [HttpPost]
        public JsonResult UpdateRegNo(int id, string regNo)
        {
            if (string.IsNullOrWhiteSpace(regNo))
                return Json(new { success = false, message = "Registration Number is required" });

            if (regNo.Length > 20)
                return Json(new { success = false, message = "Registration No cannot exceed 20 characters" });

            var regex = new System.Text.RegularExpressions.Regex(@"^[A-Za-z]{2}[0-9]{1,2}[A-Za-z]{1,3}[0-9]{1,4}$");
            if (!regex.IsMatch(regNo))
                return Json(new { success = false, message = "Enter valid Registration No (e.g. MH12AB1234 or DL2CAE3422)" });

            try
            {
                SqlParameter[] parameters = {
            new SqlParameter("@calltype", "UpdateRegId"),
            new SqlParameter("@id", id),
            new SqlParameter("@RegNo", regNo)
        };

                var dt = _sqlHelper.ExecuteStoredProcedure("sp_Sales", parameters);

                return Json(new { success = true, regNo = regNo, message = "Registration Number updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }

            return Json(new { success = false, message = "Something went wrong" });
        }



        /////// Receipts /////////////////////////////////////////////////////////////////////


        public ActionResult Receipts()
        {
            SqlParameter[] parameters = { new SqlParameter("@calltype", "GetAllReceipts") };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Receipts", parameters);

            var receiptList = dt.AsEnumerable().Select(row => new Receipts
            {
                Id = Convert.ToInt32(row["Id"]),
                CustomerName = row["CustomerName"].ToString(),
                MobileNo = row["MobileNo"].ToString(),
                Amount = Convert.ToDecimal(row["Amount"]),
                ReceiptDate = Convert.ToDateTime(row["ReceiptDate"]),
                PaymentMode = row["PaymentMode"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"])
            }).ToList();

            return View(receiptList);
        }

        public ActionResult AddReceipts()
        {
            ViewBag.BrandsList = GetBrands();
            return View(new Receipts());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReceipts(Receipts model)
        {
            if (ModelState.IsValid)
            {
                SqlParameter[] parameters = {
            new SqlParameter("@calltype", "InsertReceipts"),
            new SqlParameter("@CustomerName", model.CustomerName),
            new SqlParameter("@MobileNo", model.MobileNo),
            new SqlParameter("@Amount", model.Amount),
            new SqlParameter("@ReceiptDate", model.ReceiptDate),
            new SqlParameter("@BrandsId", model.BrandsId),
            new SqlParameter("@ModelId", model.ModelId),
            new SqlParameter("@PaymentMode", model.PaymentMode),
        };

                var result = _sqlHelper.ExecuteStoredProcedure("sp_Receipts", parameters);

                TempData["Success"] = "Receipt added successfully!";
                return RedirectToAction("Receipts");
            }

            TempData["Error"] = "Please check the form fields.";
            return View(model);
        }


        private Receipts GetReceiptsById(int id)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@calltype", "GetReceiptsById"),
                new SqlParameter("@id", id)
                };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Receipts", parameters);

            var model = new Receipts();

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                model.Id = Convert.ToInt32(row["id"]);
                model.CustomerName = row["CustomerName"].ToString();
                model.MobileNo = row["MobileNo"].ToString();
                model.Amount = (decimal)row["Amount"];
                model.ReceiptDate = Convert.ToDateTime(row["ReceiptDate"]);
                model.PaymentMode = row["PaymentMode"].ToString();
                model.ModelName = row["modelname"].ToString();

            }
            return model;
        }
        public ActionResult ReceiptsDetails(int id)
        {
            var model = GetReceiptsById(id);
            return View("ReceiptsDetails", model);
        }

        public ActionResult DownloadSales()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DownloadSalesData(string dateRange)
        {
            if (string.IsNullOrEmpty(dateRange))
                return Json(new { success = false, message = "Please select a date range." });

            try
            {
                var dates = dateRange.Split(" to ");
                DateTime startDate = DateTime.ParseExact(dates[0], "dd-MM-yyyy", null);
                DateTime endDate = DateTime.ParseExact(dates[1], "dd-MM-yyyy", null);


                SqlParameter[] parameters = {
            new SqlParameter("@StartDate", startDate),
            new SqlParameter("@EndDate", endDate),
            new SqlParameter("@calltype", "GetSalesByDateRange")
        };

                var dt = _sqlHelper.ExecuteStoredProcedure("sp_Sales", parameters);

                var salesList = dt.AsEnumerable()
              .Select(row => new TransactionViewModel
              {
                  Id = Convert.ToInt32(row["TransactionId"]),
                  Name = row["name"].ToString(),
                  MobileNo = row["Mobile"].ToString(),
                  ModelName = row["modelName"].ToString(),
                  Downpayment = Convert.ToDecimal(row["DownPayment"]),
                  FinancedBy = row["Financedby"].ToString(),
                  CashAmount = Convert.ToDecimal(row["EMIAmount"]),
                  NoOfInstallments = Convert.ToInt32(row["NoOfInstallments"]),
                  MeterReading = Convert.ToInt32(row["meterreading"]),
                  InsuranceDuDate = Convert.ToDateTime(row["InsuranceDueDate"]),
                  dateofsale = Convert.ToDateTime(row["DateOfSale"]),
                  DateOfSaleReminder = Convert.ToDateTime(row["ServicingReminderDate"])
              }).ToList();

                return Json(new { success = true, data = salesList });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


    }
}