using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestCaseBookShop.Models;
using TestCaseBookShop.Models.Data;
using TestCaseBookShop.Models.Data.Enum;

namespace TestCaseBookShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;

        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = _context.Categories.FromSqlRaw("EXEC GetAllCategories").ToList();

                return Json(new { success = true, message = "Kategoriler başarıyla getirildi!", categories });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCategories API'nda bir hata oluştu: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCategory(Guid ID)
        {
            try
            {
                var category = _context.Categories
                                   .FromSqlRaw("EXEC GetCategoryByID @ID", new SqlParameter("@ID", ID))
                                   .AsEnumerable()
                                   .FirstOrDefault();

                return Json(new { success = true, message = "Kategori başarıyla getirildi!", category });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCategory API'nda bir hata oluştu: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            try
            {
                var categoryCheck = _context.Categories.Any(a => a.Name == category.Name && a.RecordState == RecordState.Active);

                if (categoryCheck)
                    return Json(new { success = false, message = "Kategori Mevcut!" });

                var paramaters = new SqlParameter[]
                {
                new SqlParameter("@ID", category.ID),
                new SqlParameter("@Name", category.Name),
                new SqlParameter("@CreatedByID", category.CreatedByID)
                };

                _context.Database.ExecuteSqlRaw("EXEC CreateCategory @ID, @Name, @CreatedByID", paramaters);

                _logger.LogInformation($"{_context.Users.FirstOrDefault(a => a.ID == category.CreatedByID).UserName} tarafından {category.Name} adlı kategori eklendi.");

                return Json(new { success = true, message = "Kategori başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateCategory API'nda bir hata oluştu: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPut]
        public IActionResult UpdateCategory(Category category)
        {
            try
            {
                var paramaters = new SqlParameter[]
                {
                new SqlParameter("@ID", category.ID),
                new SqlParameter("@Name", category.Name),
                new SqlParameter("@UpdatedByID", category.UpdatedById)
                };

                _context.Database.ExecuteSqlRaw("EXEC UpdateCategory @ID, @Name, @UpdatedByID", paramaters);

                _logger.LogInformation($"{_context.Users.FirstOrDefault(a => a.ID == category.UpdatedById).UserName} tarafından {category.Name} adlı kategori güncellendi.");

                return Json(new { success = true, message = "Kategori başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateCategory API'nda bir hata oluştu: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteCategory(Guid ID)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var bookCheck = _context.Books.Any(a => a.CategoryID == ID && a.RecordState == RecordState.Active);

                if (bookCheck)
                    return Json(new { success = false, message = "Kategoriye ait kitaplar mevcut!" });

                var paramaters = new SqlParameter[]
                {
                new SqlParameter("@ID", ID)
                };

                _context.Database.ExecuteSqlRaw("EXEC DeleteCategory @ID", paramaters);

                _logger.LogInformation($"{_context.Users.FirstOrDefault(a => a.ID == Guid.Parse(userId)).UserName} tarafından {ID}'li kategori silindi.");

                return Json(new { success = true, message = "Kategori başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteCategory API'nda bir hata oluştu: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }
    }
}
