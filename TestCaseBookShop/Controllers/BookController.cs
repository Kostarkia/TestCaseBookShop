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
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;

        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context, ILogger<BookController> logger)
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
        public IActionResult GetBooks()
        {
            try
            {
                //Normalde bu şekilde kullanmayı tercih ediyorum. foreach dönmemek için.
                //var books = _context.Books.Include(a => a.Category)
                //                           .Where(a => a.RecordState == RecordState.Active).OrderByDescending(a => a.CreatedAt)
                //                           .ToList();

                var books = _context.Books.FromSqlRaw("EXEC GetAllBooks")
                                         .AsEnumerable()
                                         .OrderByDescending(a => a.CreatedAt)
                                         .ToList();

                foreach (var item in books)
                {
                    item.Category = _context.Categories.FirstOrDefault(a => a.ID == item.CategoryID);
                }

                return Json(new { success = true, message = "Kitaplar başarıyla getirildi!", books });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetBooks API'nda bir hata oluştu: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetBook(Guid ID)
        {
            try
            {
                var book = _context.Books
                               .FromSqlRaw("EXEC GetBookByID @ID", new SqlParameter("@ID", ID))
                               .AsEnumerable()
                               .FirstOrDefault();

                return Json(new { success = true, message = "Kitap başarıyla getirildi!", book });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetBook API'nda bir hata oluştu: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }

        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateBook([FromBody] Book book)
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value;
                var bookCheck = _context.Books.Any(a => a.Title == book.Title && a.RecordState == RecordState.Active);

                if (bookCheck)
                    return Json(new { success = false, message = "Kitap Mevcut!" });

                var paramaters = new SqlParameter[]
                {
                new SqlParameter("@ID", book.ID),
                new SqlParameter("@Title", book.Title),
                new SqlParameter("@Author", book.Author),
                new SqlParameter("@Price", book.Price),
                new SqlParameter("@Stock", book.Stock),
                new SqlParameter("@CreatedByID", book.CreatedByID),
                new SqlParameter("@CategoryID", book.CategoryID)
                };

                _context.Database.ExecuteSqlRaw("EXEC CreateBook @ID, @Title, @Author, @Price, @Stock, @CreatedByID, @CategoryID", paramaters);

                _logger.LogInformation($"{_context.Users.FirstOrDefault(a => a.ID == book.CreatedByID).UserName} tarafından {book.Title} kitabı oluşturuldu.");

                return Json(new { success = true, message = "Kitap başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateBook API'nda bir hata oluştu: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPut]
        public IActionResult UpdateBook([FromBody] Book book)
        {
            try
            {
                var paramaters = new SqlParameter[]
                {
                new SqlParameter("@ID", book.ID),
                new SqlParameter("@Title", book.Title),
                new SqlParameter("@Author", book.Author),
                new SqlParameter("@Price", book.Price),
                new SqlParameter("@Stock", book.Stock),
                new SqlParameter("@CategoryID", book.CategoryID),
                new SqlParameter("@UpdatedByID", book.UpdatedById),
                };

                _context.Database.ExecuteSqlRaw("EXEC UpdateBook @ID, @Title, @Author, @Price, @Stock, @CategoryID, @UpdatedByID", paramaters);

                _logger.LogInformation($"{_context.Users.FirstOrDefault(a => a.ID == book.UpdatedById).UserName} tarafından {book.Title} kitabı güncellendi.");

                return Json(new { success = true, message = "Kitap başarıyla güncellendi!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateBook API'nda bir hata oluştu: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteBook(Guid ID)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var paramaters = new SqlParameter[]{

                new SqlParameter("@ID", ID)
                };

                _context.Database.ExecuteSqlRaw("EXEC DeleteBook @ID", paramaters);

                _logger.LogInformation($"{_context.Users.FirstOrDefault(a => a.ID == Guid.Parse(userId)).UserName} tarafından {ID}'li kitap silindi.");

                return Json(new { success = true, message = "Kitap başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteBook API'nda bir hata oluştu: {ex.Message}");
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }
    }
}
