using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1080024.BusinessLayers;
using SV21T1080024.DomainModels;
using SV21T1080024.Web.Models;
using System.Buffers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1080024.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class ProductController : Controller
    {
        const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "product_search";
        /*public IActionResult Index(int page = 1, string searchValue = "", int categoryID = 0, int supplierID = 0,
                                                decimal minPrice = 0, decimal maxPrice = 0)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, page, PAGE_SIZE, searchValue, categoryID, supplierID, minPrice, maxPrice);

            Models.ProductSearchResult model = new Models.ProductSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Data = data
            };
            return View(model);
        }*/
        public IActionResult Index()
        {
            ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(SEARCH_CONDITION);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0
                };
            }
            return View(input);
        }
        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "",
                                                        input.CategoryID, input.SupplierID, input.MinPrice, input.MaxPrice);
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = input.CategoryID,
                SupplierID = input.SupplierID,
                MinPrice = input.MinPrice,
                MaxPrice = input.MaxPrice,
                Data = data
            };
            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Mặt hàng";
            Product product = new Product()
            {
                ProductID = 0,
                Photo = "nophoto.png",
                IsSelling = true
            };
            return View("Edit", product);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin Mặt hàng";
            Product? product = ProductDataService.GetProduct(id);
            if (product == null)
                return RedirectToAction("Index");
            if (string.IsNullOrEmpty(product.Photo))
                product.Photo = "nophoto.png";
            return View(product);
        }
        [HttpPost]
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {

            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Vui lòng nhập đơn vị tính");
            
            data.ProductDescription = data.ProductDescription ?? "";

            //Xử lý với ảnh upload (nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho employee)
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; //Tên file sẽ lưu
                string folder = Path.Combine(ApplicationContext.WebRootPath, @"images\products"); //đường dẫn đến thư mục lưu file
                string filePath = Path.Combine(folder, fileName); //Đường dẫn đến file cần lưu D:\images\products\photo.png

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
            if (data.ProductID == 0)
            {
                ProductDataService.AddProduct(data);
            }
            else
            {
                ProductDataService.UpdateProduct(data);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }

            var product = ProductDataService.GetProduct(id);
            if (product == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !ProductDataService.IsUsedProduct(id);
            return View(product);
        }
        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    ProductPhoto productPhoto = new ProductPhoto()
                    {
                        ProductID = id,
                        PhotoID = 0,
                        Photo = "nophoto.png"
                    };
                    return View(productPhoto);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    ProductPhoto? photoEdit = ProductDataService.GetPhoto(photoId);
                    if (photoEdit == null)
                        return RedirectToAction("Edit", new { id = id });
                    if (string.IsNullOrEmpty(photoEdit.Photo))
                        photoEdit.Photo = "nophoto.png";
                    return View(photoEdit);
                case "delete":
                    //TODO: Xóa ảnh (xóa trực tiếp, không cần confirm)
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");

            }

        }
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {

            ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh cho mặt hàng" : "Thay đổi ảnh của mặt hàng";
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Vui lòng nhập mô tả ảnh");
            //Xử lý với ảnh upload (nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho employee)
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; //Tên file sẽ lưu
                string folder = Path.Combine(ApplicationContext.WebRootPath, @"images\products"); //đường dẫn đến thư mục lưu file
                string filePath = Path.Combine(folder, fileName); //Đường dẫn đến file cần lưu D:\images\products\photo.png

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }


            //Nếu tồn tại lỗi thì trả dữ liệu về lại cho View để người dùng nhập lại cho đúng
            if (!ModelState.IsValid)
            {

                return View("Photo", data);
            }
            //Gọi chức năng xử lý dưới tầng tác nghiệp nếu quá trình kiểm soát lỗi không phát hiện lỗi đầu vào
            if (data.PhotoID == 0)
            {
                ProductDataService.AddPhoto(data);
            }
            else
            {
                ProductDataService.UpdatePhoto(data);
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }
        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    ProductAttribute productAttribute = new ProductAttribute()
                    {
                        ProductID = id,
                        AttributeID = 0,
                    };
                    return View(productAttribute);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
                    ProductAttribute? attributeEdit = ProductDataService.GetAttribute(attributeId);
                    if (attributeId == null)
                        return RedirectToAction("Edit", new { id = id });
                    return View(attributeEdit);
                case "delete":
                    //TODO: Xóa ảnh (xóa trực tiếp, không cần confirm)
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");

            }

        }
        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính cho mặt hàng" : "Thay đổi thuộc tính của mặt hàng";
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Vui lòng nhập giá trị thuộc tính");

            //Nếu tồn tại lỗi thì trả dữ liệu về lại cho View để người dùng nhập lại cho đúng
            if (!ModelState.IsValid)
            {

                return View("Attribute", data);
            }
            //Gọi chức năng xử lý dưới tầng tác nghiệp nếu quá trình kiểm soát lỗi không phát hiện lỗi đầu vào
            if (data.AttributeID == 0)
            {
                ProductDataService.AddAttribute(data);
            }
            else
            {
                ProductDataService.UpdateAttribute(data);
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }
    }
}
