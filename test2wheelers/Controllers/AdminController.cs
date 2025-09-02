using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;
using System.Data;
using _2whealers.Models;
using Microsoft.AspNetCore.Authorization;
using Rotativa.AspNetCore;

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

                _sqlHelper.ExecuteStoredProcedureNonQuery("sp_Users", parameters);

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

                _sqlHelper.ExecuteStoredProcedureNonQuery("sp_Users", parameters);

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

                _sqlHelper.ExecuteStoredProcedureNonQuery("sp_Users", parameters);

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

                _sqlHelper.ExecuteStoredProcedureNonQuery("sp_Users", parameters);

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

            var userTransactionList = dt.AsEnumerable().Select(row => new UserTransactionViewModel
            {
                Purchase = new PurchaseViewModel
                {
                    Name = row["Name"].ToString(),
                    MobileNo = row["Mobile"].ToString(),
                    Id = Convert.ToInt32(row["transactionid"]),
                    ModelName = row["ModelName"].ToString(),
                    CashAmount = Convert.ToDecimal(row["CashAmount"]),
                    IsActive = Convert.ToBoolean(row["IsActive"])
                }
            }).OrderByDescending(x => x.Purchase.Id).ToList();

            return View(userTransactionList);
        }

        public ActionResult AddPreSale()
        {
            var model = new UserTransactionViewModel
            {
                Purchase = new PurchaseViewModel()
            };

            ViewBag.BankList = GetBankList();
            ViewBag.BikeList = GetBrands();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPreSale(UserTransactionViewModel model)
        {
            try
            {
                SqlParameter[] parameters = {
            new SqlParameter("@calltype", "InsertPreSales"),
            new SqlParameter("@userName", model.Purchase.Name ),
            new SqlParameter("@MobileNo", model.Purchase.MobileNo ),
            new SqlParameter("@Address", model.Purchase.Address1 ),
            new SqlParameter("@ModelId", model.Purchase.ModelId),
            new SqlParameter("@BikeId", model.Purchase.BikeId),
            new SqlParameter("@VehicleNo", (model.Purchase.RegNo ?? "").ToUpper()),
            new SqlParameter("@DateOfSale", model.Purchase.dateofsale ),
            new SqlParameter("@ChassisNo", model.Purchase.ChassisNo ),
            new SqlParameter("@EngineNo", model.Purchase.EngineNo ),
            new SqlParameter("@CashAmount", Convert.ToDecimal(model.Purchase.CashAmount)),
            new SqlParameter("@InsuranceValid", model.Purchase.InsuranceValid ),
            new SqlParameter("@BankName", model.Purchase.FinancedBy ),
            new SqlParameter("@NocValid", model.Purchase.NocValid ),
        };

                _sqlHelper.ExecuteStoredProcedureNonQuery("sp_Users", parameters);

                TempData["Success"] = "Pre-sale added successfully!";
                return RedirectToAction("PreSales");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to add Pre-sale. Error: " + ex.Message;

                ViewBag.BikeList = GetBrands();
                ViewBag.ModelList = GetBikeModels();
                ViewBag.BankList = GetBankList();

                return View(model);
            }
        }


        public ActionResult PreSaleDetails(int id)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "GetPreSaleById"),
        new SqlParameter("@id", id)
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Users", parameters);

            var model = new UserTransactionViewModel { Purchase = new PurchaseViewModel() };

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                model.Purchase.Id = Convert.ToInt32(row["TransactionId"]);
                model.Purchase.Name = row["Name"].ToString();
                model.Purchase.MobileNo = row["Mobile"].ToString();
                model.Purchase.Address1 = row["Address1"].ToString();
                model.Purchase.dateofsale = Convert.ToDateTime(row["DateOfSale"]);
                model.Purchase.RegNo = row["RegNo"].ToString();
                model.Purchase.ChassisNo = row["ChassisNo"].ToString();
                model.Purchase.EngineNo = row["EngineNo"].ToString();
                model.Purchase.CashAmount = Convert.ToDecimal(row["CashAmount"]);
                model.Purchase.ModelName = row["ModelName"].ToString();
                model.Purchase.FinancedBy = row["BankName"].ToString();
            }

            return View("PreSaleDetails", model);
        }


        /////// Sales /////////////////////////////////////////////////////////////////////
        public ActionResult Sales()
        {
            SqlParameter[] parameters = { new SqlParameter("@calltype", "GetAllSales") };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Sales", parameters);

            var salesList = dt.AsEnumerable()
                .Select(row => new UserTransactionViewModel
                {
                    Transaction = new TransactionViewModel
                    {
                        Name = row["Name"].ToString(),
                        MobileNo = row["Mobile"].ToString(),
                        Id = Convert.ToInt32(row["TransactionId"]),
                        ModelName = row["ModelName"].ToString(),
                        dateofsale = Convert.ToDateTime(row["DateOfSale"]),
                        Downpayment = Convert.ToDecimal(row["DownPayment"]),
                        CashAmount = Convert.ToDecimal(row["EMIAmount"]),
                        DateOfSaleReminder = Convert.ToDateTime(row["ServicingReminderDate"])
                    }
                }).ToList();

            return View(salesList);
        }


        public ActionResult AddSales()
        {
            var model = new UserTransactionViewModel
            {
                Transaction = new TransactionViewModel()
            };

            ViewBag.BankList = GetBankList();
            ViewBag.BikeList = GetBrands();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSales(UserTransactionViewModel model)
        {
            try
            {

                SqlParameter[] parameters = {
        new SqlParameter("@calltype", "InsertSales"),
        new SqlParameter("@Name", model.Transaction.Name),
        new SqlParameter("@MobileNo", model.Transaction.MobileNo),
        new SqlParameter("@Address1", model.Transaction.Address1 ),
        new SqlParameter("@Address2", model.Transaction.Address2 ),
        new SqlParameter("@DateOfBirth", model.Transaction.DateOfBirth),
        new SqlParameter("@Email", model.Transaction.Email ),
        new SqlParameter("@Location", model.Transaction.Location ),
        new SqlParameter("@Pincode", model.Transaction.Pincode ),
        new SqlParameter("@State", model.Transaction.State ),
        new SqlParameter("@City", model.Transaction.City ),
        new SqlParameter("@BikeId", model.Transaction.BikeId),
        new SqlParameter("@ChassisNo", model.Transaction.ChassisNo ),
        new SqlParameter("@DRM", model.Transaction.DRM),
        new SqlParameter("@DateOfSale", model.Transaction.dateofsale),
        new SqlParameter("@DownPayment", model.Transaction.Downpayment),
        new SqlParameter("@EMIAmount", model.Transaction.CashAmount),
        new SqlParameter("@EngineNo", model.Transaction.EngineNo ),
        new SqlParameter("@FinancedBy", model.Transaction.FinancedBy ),
        new SqlParameter("@InsuranceDueDate", model.Transaction.InsuranceDuDate),
        new SqlParameter("@MR", model.Transaction.MeterReading),
        new SqlParameter("@ManufacturingYear", model.Transaction.ManufacturingYear ),
        new SqlParameter("@ModelId", model.Transaction.ModelId),
        new SqlParameter("@NoOfInstallments", model.Transaction.NoOfInstallments),
        new SqlParameter("@Notes", model.Transaction.Notes ),
        new SqlParameter("@RegNo", model.Transaction.RegNo ),
        new SqlParameter("@VehicleDetails", model.Transaction.VehicleDetails ),
        new SqlParameter("@IsRegularService", model.Transaction.IsRegularService),
        new SqlParameter("@ServicingReminderDate", model.Transaction.dateofsale) // 🚨 careful: you had ServicingReminderDate = dateofsale
    };

                _sqlHelper.ExecuteStoredProcedureNonQuery("sp_Sales", parameters);

                TempData["Success"] = "Sales added successfully!";
                return RedirectToAction("Sales");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to add Pre-sale. Error: " + ex.Message;

                ViewBag.BikeList = GetBrands();
                ViewBag.ModelList = GetBikeModels();
                ViewBag.BankList = GetBankList();
                 

                return View(model);
            }
        }



        private UserTransactionViewModel GetSaleById(int id)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@calltype", "GetSaleById"),
        new SqlParameter("@id", id)
    };

            var dt = _sqlHelper.ExecuteStoredProcedure("sp_Sales", parameters);

            var model = new UserTransactionViewModel { Transaction = new TransactionViewModel() };

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                model.Transaction.Id = Convert.ToInt32(row["TransactionId"]);
                model.Transaction.Name = row["Name"].ToString();
                model.Transaction.MobileNo = row["Mobile"].ToString();
                model.Transaction.ModelName = row["ModelName"].ToString();
                model.Transaction.Address1 = row["Address1"].ToString();
                model.Transaction.dateofsale = Convert.ToDateTime(row["DateOfSale"]);
                model.Transaction.RegNo = row["RegNo"].ToString();
                model.Transaction.ChassisNo = row["ChassisNo"].ToString();
                model.Transaction.EngineNo = row["EngineNo"].ToString();
                model.Transaction.DRM = Convert.ToInt32(row["DRM"]);
                model.Transaction.FinancedBy = row["FinancedBy"].ToString();
                model.Transaction.NoOfInstallments = Convert.ToInt32(row["NoOfInstallments"]);
                model.Transaction.CashAmount = Convert.ToDecimal(row["EMIAmount"]);
                model.Transaction.VehicleDetails = row["VehicleDetails"].ToString();
                model.Transaction.Notes = row["Notes"].ToString();
                model.Transaction.DateOfSaleReminder = Convert.ToDateTime(row["ServicingReminderDate"]);
            }

            return model;
        }


        public ActionResult SaleDetails(int id)
        {
            var model = GetSaleById(id);
            return View("SaleDetails", model);
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

    }
}