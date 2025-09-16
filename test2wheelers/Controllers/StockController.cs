using _2whealers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using test2wheelers.Helpers;

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
        public IActionResult AddStock()
        {
            var model = new ManageStockViewModel();
             
            ViewBag.BikeList = GetBrands();
            return View(model); 
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
