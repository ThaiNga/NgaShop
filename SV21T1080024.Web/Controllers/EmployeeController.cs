using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1080024.BusinessLayers;
using SV21T1080024.DomainModels;
using SV21T1080024.Web.Models;
using System.Reflection;

namespace SV21T1080024.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    public class EmployeeController : Controller
    {
        const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "employee_search"; //Tên biến dùng để lưu trong session

        public IActionResult Index()
        {
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SEARCH_CONDITION);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(model);
        }

        /*public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfEmployees(out rowCount, page, PAGE_SIZE, searchValue ?? "");

            Models.EmployeeSearchResult model = new Models.EmployeeSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model);
        }*/
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            Employee employee = new Employee()
            {
                EmployeeID = 0,
                BirthDate = new DateTime(1990, 1, 1),
                Photo = "nophoto.png",
                IsWorking = true
            };

            return View("Edit", employee);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            Employee? employee = CommonDataService.GetEmployee(id);
            if (employee == null)
                return RedirectToAction("Index");
            if (string.IsNullOrEmpty(employee.Photo))
                employee.Photo = "nophoto.png";
            return View(employee);
        }
        [HttpPost]
        public IActionResult Save(Employee data, string birthDateInput, IFormFile? uploadPhoto)
        {
            
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";
            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email");
            data.Phone = data.Phone ?? "";
            data.Address = data.Address ?? "";
            //Xử lý ngày sinh
            DateTime? birthDate = birthDateInput.ToDateTime();
            if (birthDate.HasValue)
                data.BirthDate = birthDate.Value;

            //Xử lý với ảnh upload (nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho employee)
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; //Tên file sẽ lưu
                string folder = Path.Combine(ApplicationContext.WebRootPath, @"images\employees"); //đường dẫn đến thư mục lưu file
                string filePath = Path.Combine(folder, fileName); //Đường dẫn đến file cần lưu D:\images\employees\photo.png

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }


            //Nếu tồn tại lỗi thì trả dữ liệu về lại cho View để người dùng nhập lại cho đúng
            if (!ModelState.IsValid)
            {

                return View("Edit", data);
            }
            //Gọi chức năng xử lý dưới tầng tác nghiệp nếu quá trình kiểm soát lỗi không phát hiện lỗi đầu vào
            if (data.EmployeeID == 0)
            {
                int id = CommonDataService.AddEmployee(data);
                if (id <= 0)
                {
                    ModelState.AddModelError("Email", "Email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateEmployee(data);
                if (!result)
                {
                    ModelState.AddModelError("Email", "Email bị trùng");                   
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }

            var employee = CommonDataService.GetEmployee(id);
            if (employee == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !CommonDataService.IsUsedEmployee(id);
            return View(employee);
        }
    }
}
