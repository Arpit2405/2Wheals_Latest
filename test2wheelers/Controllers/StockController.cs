using _2whealers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using _2whealers.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace _2whealers.Controllers
{
    public class StockController : Controller
    {
        private readonly SqlHelper _sqlHelper;
        private readonly IWebHostEnvironment _env;

        public StockController(SqlHelper sqlHelper, IWebHostEnvironment env)
        {
            _env = env;
            _sqlHelper = sqlHelper;
        }

        public  IActionResult Stocks()
        {
            var regionIdClaim = User.Claims.FirstOrDefault(x => x.Type == "RegionId");
            string RegionId = regionIdClaim?.Value;
            if (!string.IsNullOrEmpty(RegionId))
            {

                SqlParameter[] parameters = {
                 new SqlParameter("@calltype", "GetAllStocks")
                  };

                var dt = _sqlHelper.ExecuteStoredProcedure("sp_stock", parameters);

                var stockList = dt.AsEnumerable()
                   .Select(row => new ManageStockViewModel
                   {
                       Id = Convert.ToInt32(row["Id"]),
                       BrandName = row["BrandName"].ToString(),
                       ModelName = row["ModelName"].ToString(),
                       Quantity = row["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["Quantity"]),
                       LastUpdated = row["LastUpdated"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["LastUpdated"]),
                       Status = row["Isactive"] == DBNull.Value ? false : Convert.ToBoolean(row["Isactive"])
                   }).ToList();

                return View(stockList);
            }
            else
            {
                 //HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Login");
            }
        }

        public IActionResult AddStock()
        {
            var model = new ManageStockViewModel();

            ViewBag.BikeList = GetBrands();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStock(ManageStockViewModel model)
        {
            if (ModelState.IsValid)
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@calltype", "Insert"),
                        new SqlParameter("@BrandsId", model.BrandsId),
                        new SqlParameter("@ModelId", model.ModelId),
                        new SqlParameter("@Quantity", model.Quantity),
                        new SqlParameter("@LastUpdated", DateTime.Now)
                    };

                var dt = _sqlHelper.ExecuteStoredProcedure("sp_stock", parameters);
                TempData["SuccessMessage"] = "Stock updated successfully!";

                return RedirectToAction("Stocks");
            }

            TempData["ErrorMessage"] = "Something went wrong, please try again.";
            return RedirectToAction("Stocks");
        }

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
    }

}
