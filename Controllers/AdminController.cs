using dotnet_project_ecommerce.Models;
using dotnet_project_ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;
using BCrypt.Net;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace dotnet_project_ecommerce.Controllers
{
    public class AdminController : Controller
    {
        private readonly myContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string _adminImagePath;


        public AdminController(myContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _adminImagePath = Path.Combine(env.WebRootPath, configuration["FileUploadSettings:AdminImagePath"]);
            //_customerImagePath = Path.Combine(env.WebRootPath, configuration["FileUploadSettings:CustomerImagePath"]);
        }

        // Helper method to check if admin is logged in
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("admin_session") != null;
        }

        // Helper method to get the current admin ID
        private int GetCurrentAdminId()
        {
            return int.Parse(HttpContext.Session.GetString("admin_session"));
        }
        private int GetCurrentCustomerId()
        {
            return int.Parse(HttpContext.Session.GetString("customer_session"));
        }

        // Admin Dashboard
        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // Admin Login Page
        public IActionResult Login()
        {
            return View();
        }

        // Admin Login Action
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.admin_email) || string.IsNullOrEmpty(model.admin_password))
            {
                TempData["Message"] = "Email and password are required.";
                return View(model);
            }

            var admin = await _context.tbl_admin.FirstOrDefaultAsync(a => a.admin_email == model.admin_email);

            if (admin != null && admin.VerifyPassword(model.admin_password))
            {
                HttpContext.Session.SetString("admin_session", admin.admin_id.ToString());
                return RedirectToAction("Index");
            }

            TempData["Message"] = "Incorrect email or password.";
            return View(model);
        }

        // Admin Registration Page
        public IActionResult Register()
        {
            return View();
        }

        // Admin Registration Action
        [HttpPost]
        public async Task<IActionResult> Register(AdminRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = new Admin
                {
                    admin_name = model.admin_name,
                    admin_email = model.admin_email,
                    admin_password = model.admin_password // Hash this password before saving
                };

                admin.HashPassword(); // Hash the password

                _context.tbl_admin.Add(admin);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // Admin Logout Action
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("admin_session");
            return RedirectToAction("Login");
        }

        // Admin Profile Page
        public async Task<IActionResult> Profile()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var admin = await _context.tbl_admin.FirstOrDefaultAsync(a => a.admin_id == GetCurrentAdminId());
            if (admin == null)
            {
                return RedirectToAction("Login");
            }

            var adminList = new List<Admin> { admin }; // Wrap the single Admin object in a list
            return View(adminList); // Return a list
        }

        // Admin Profile Update Action
        [HttpPost]
        public async Task<IActionResult> Profile(Admin admin)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Invalid data. Please check your inputs.";
                return View(admin);
            }

            var existingAdmin = await _context.tbl_admin.FirstOrDefaultAsync(a => a.admin_id == GetCurrentAdminId());
            if (existingAdmin == null)
            {
                return RedirectToAction("Login");
            }

            // Update profile fields
            existingAdmin.admin_email = admin.admin_email;
            existingAdmin.admin_name = admin.admin_name;

            _context.tbl_admin.Update(existingAdmin);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        // Change Profile Image Action


        [HttpPost]
        public async Task<IActionResult> ChangeProfileImage(IFormFile admin_image)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login");
            }

            if (admin_image == null || admin_image.Length == 0)
            {
                TempData["Message"] = "Please select a valid image file.";
                return RedirectToAction("Profile");
            }

            // Validate file type and size
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(admin_image.FileName)?.ToLower();

            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                TempData["Message"] = "Only image files (jpg, jpeg, png, gif) are allowed.";
                return RedirectToAction("Profile");
            }

            if (admin_image.Length > 5 * 1024 * 1024) // 5 MB
            {
                TempData["Message"] = "File size must be less than 5 MB.";
                return RedirectToAction("Profile");
            }

            try
            {
                // Ensure the upload folder exists
                if (!Directory.Exists(_adminImagePath))
                {
                    Directory.CreateDirectory(_adminImagePath);
                }

                // Generate a unique file name
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(admin_image.FileName);
                var filePath = Path.Combine(_adminImagePath, uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await admin_image.CopyToAsync(stream);
                }

                // Update the admin's profile image in the database
                var adminId = GetCurrentAdminId();
                var admin = await _context.tbl_admin.FirstOrDefaultAsync(a => a.admin_id == adminId);

                if (admin == null)
                {
                    TempData["Message"] = "Admin not found.";
                    return RedirectToAction("Profile");
                }

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(admin.admin_image))
                {
                    var oldFilePath = Path.Combine(_adminImagePath, admin.admin_image);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Update the admin's profile image
                admin.admin_image = uniqueFileName;

                _context.tbl_admin.Update(admin);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Profile image updated successfully.";
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error: {ex.Message}");
                TempData["Message"] = "An error occurred while updating the profile image.";
            }

            return RedirectToAction("Profile");
        }

// fetching the customers 
public IActionResult fetchCustomer()
        {
            return View( _context.tbl_customer.ToList());

        }
//viewing customer details 
 public IActionResult CustomerDetails(int id )

        {
            return View(
            _context.tbl_customer.FirstOrDefault(c=> c.customer_id == id ));   
        }

// updating the customer details 
 public IActionResult UpdateCustomerDetail(int id )
        {
            return View(_context.tbl_customer.Find(id));
        }
        [HttpPost]
      //update customer 
      public IActionResult UpdateCustomerDetail(Customer customer,IFormFile customer_image)
        {
            string ImagePath = Path.Combine(_env.WebRootPath, "Uploads/Customer_Images", customer_image.FileName);
            FileStream fs = new FileStream(ImagePath, FileMode.Create);
            customer_image.CopyTo(fs);
            customer.customer_image = customer_image.FileName;
            _context.tbl_customer.Update(customer);
            _context.SaveChanges();
            return RedirectToAction("fetchCustomer");

        }


        //deleting the customer 
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_id == id);
            _context.tbl_customer.Remove(customer);
            _context.SaveChanges();
            return RedirectToAction("fetchCustomer");
        }
        public IActionResult DeletePermission(int id)
        {
            
          var customer=  _context.tbl_customer.FirstOrDefault(a=>a.customer_id == id );

            return View(customer);
        }

        // fetching category 
        public IActionResult FetchCategory()
        {
            return View(_context.tbl_category.ToList());
        }
        public IActionResult AddCategory()
        {
            return View(    );
        }
        [HttpPost]
        public IActionResult AddCategory(Category c)
        {
            _context.tbl_category.Add(c);
            _context.SaveChanges();
            return RedirectToAction("FetchCategory");
        }
// Update Category 
public IActionResult UpdateCategory(int id)
        {
            var c = _context.tbl_category.Find(id);
            return View(c);
        }
        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            _context.tbl_category.Update(category);
            _context.SaveChanges();
            return RedirectToAction("FetchCategory");
            //return View();
        }
        //deleting the category 

        // confirm delete 
        public IActionResult DeleteCategoryPermission(int id)
        {
            var c = _context.tbl_category.FirstOrDefault(c=> c.category_id == id);
            return View(c);
        }

  
        public IActionResult DeleteCategory(int id)
        {
            var delete = _context.tbl_category.Find(id);
            _context.tbl_category.Remove(delete);
            
            _context.SaveChanges();
            return RedirectToAction("FetchCategory");
        }
        // For products 

        public IActionResult FetchProduct()
        {
            var f = _context.tbl_product.ToList();
            return View(f);
        }
        public IActionResult AddProduct()
        {
            List<Category> categories = _context.tbl_category.ToList();
            ViewData["Category"] = categories;
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product,IFormFile product_image)
        {
            string imageName = Path.GetFileName(product_image.FileName);
            string imagePath = Path.Combine(_env.WebRootPath,"Uploads/Product_Images/",imageName);
            FileStream fs = new FileStream(imagePath, FileMode.Create);
            product_image.CopyTo(fs);
            product.product_image= imageName;
            _context.tbl_product.Add(product);
            _context.SaveChanges();

            return RedirectToAction("FetchProduct");
        }

//Product Details page
        public IActionResult ProductDetail(int id)

        {
            return View(
            _context.tbl_product.Include(p=>p.Category).FirstOrDefault(p=>p.product_id==id));
        }


        //product Delete Page
        public IActionResult DeleteProductPermission(int id)
        {
            var c = _context.tbl_product.Include(p=>p.Category).FirstOrDefault(c => c.product_id == id);
            return View(c);
        }
        // confirm delete 


        public IActionResult DeleteProduct(int id)
        {
            var delete = _context.tbl_product.Find(id);
            _context.tbl_product.Remove(delete);

            _context.SaveChanges();
            return RedirectToAction("FetchProduct");
        }


        // Update Product Details  
        public IActionResult UpdateProduct(int id)
        {
            List<Category> categories = _context.tbl_category.ToList();
            ViewData["Category"] = categories;
            //_context.tbl_product.Include(a => a.Category).FirstOrDefault();
            var product = _context.tbl_product.Find(id);
            ViewBag.CategoryId = product.cat_id;
            return View(product);
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product product, IFormFile product_image)
        {
            string imageName = Path.GetFileName(product_image.FileName);
            string imagePath = Path.Combine(_env.WebRootPath, "Uploads/Product_Images/", imageName);
            FileStream fs = new FileStream(imagePath, FileMode.Create);
            product_image.CopyTo(fs);
            product.product_image = imageName;
            _context.tbl_product.Update(product);
            _context.SaveChanges();

            return RedirectToAction("FetchProduct");
        }

// Fetch FeedBAck
public IActionResult FetchFeedback()
        {
            //_context.tbl_feedback.ToList();

            return View(_context.tbl_feedback.ToList());
        }


        public IActionResult DeleteFeedback(int id)
        {
            var feedback = _context.tbl_feedback.Find(id);
            if (feedback == null)
            {
                return NotFound(); 
            }
            return View(feedback); 
        }
        [HttpPost]
        public IActionResult DeleteFeedback(Feedback feedback)
        {
            var f = _context.tbl_feedback.Remove(feedback);
            _context.SaveChanges();
            return RedirectToAction("fetchFeedback");
        }


        //[HttpPost, ActionName("DeleteFeedback")]
        //public IActionResult DeleteFeedbackConfirmed(int id)
        //{
        //    var feedback = _context.tbl_feedback.Find(id);
        //    if (feedback == null)
        //    {
        //        return NotFound(); // Return a 404 error if feedback is not found
        //    }

        //    _context.tbl_feedback.Remove(feedback); // Delete the feedback
        //    _context.SaveChanges(); // Save changes to the database

        //    return RedirectToAction("Feedback"); // Redirect to the feedback list
        //}


        // feedback details

        [HttpGet]
        public IActionResult ShowFeedback(int id)
        {
            var feedback = _context.tbl_feedback.Find(id); // Fetch feedback by ID
            if (feedback == null)
            {
                return NotFound(); // Return a 404 error if feedback is not found
            }
            return View(feedback); // Pass the feedback to the view
        }


        // cArt manipulation
        public IActionResult Cart()
        {
         var cart=   _context.tbl_cart.ToList();
            _context.SaveChanges();
            return View(cart);
        } 



[HttpPost]
        public IActionResult Cart( int id)
        {
            return View();

        }


    }
}