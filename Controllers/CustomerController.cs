using dotnet_project_ecommerce.Models;
using dotnet_project_ecommerce.sessionhelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using dotnet_project_ecommerce.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace dotnet_project_ecommerce.Controllers
{
    public class CustomerController : Controller
    {
        private myContext _context;
        private IWebHostEnvironment _env;

        public CustomerController(myContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Index()
        {
            List<Product> products = _context.tbl_product.ToList(); // Fetch products
            List<Category> categories = _context.tbl_category.ToList(); // Fetch categories
            ViewData["categories"] = categories;
            ViewBag.CheckSession = SessionHelper.GetCustomerSession(); // Use SessionHelper
            return View(products); // Pass products to the view
        }

        // GET: /Customer/CustomerLogin
        public IActionResult CustomerLogin()
        {
            return View();
        }

        // POST: /Customer/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            Customer customer = _context.tbl_customer.FirstOrDefault(c => c.customer_email == email && c.customer_password == password);

            if (customer != null)
            {
                SessionHelper.SetCustomerSession(customer.customer_id.ToString()); // Use SessionHelper
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View("CustomerLogin");
            }
        }

        // POST: /Customer/CustomerRegistration
        [HttpPost]
        public IActionResult CustomerRegistration(string fullName, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View("CustomerLogin");
            }

            var existingCustomer = _context.tbl_customer.FirstOrDefault(c => c.customer_email == email);
            if (existingCustomer != null)
            {
                ModelState.AddModelError("", "Email is already registered.");
                return View("CustomerLogin");
            }

            var newCustomer = new Customer
            {
                customer_name = fullName,
                customer_email = email,
                customer_password = password // Hash the password in a real application
            };

            _context.tbl_customer.Add(newCustomer);
            _context.SaveChanges();

            return RedirectToAction("CustomerLogin");
        }

        // Customer Logout
        public IActionResult CustomerLogOut()
        {
            SessionHelper.RemoveCustomerSession(); // Use SessionHelper
            return RedirectToAction("Index");
        }

        // Customer Profile
        [HttpGet]
        public IActionResult CustomerProfile()
        {
            var session = SessionHelper.GetCustomerSession(); // Use SessionHelper

            if (session == null)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            else
            {
                var user = _context.tbl_customer.FirstOrDefault();
                return View(user);
            }
        }

        // Update Customer Profile (GET)
        [HttpGet]
        public IActionResult UpdateCustomerProfile(int id)
        {
            var session = SessionHelper.GetCustomerSession(); // Use SessionHelper

            if (session == null)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            else
            {
                var user = _context.tbl_customer.Find(id);
                return View(user);
            }
        }

        // Update Customer Profile (POST)
        [HttpPost]
        public IActionResult UpdateCustomerProfile(Customer customer, IFormFile customer_image)
        {
            var session = SessionHelper.GetCustomerSession(); // Use SessionHelper

            if (session == null)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            else
            {
                if (customer_image != null)
                {
                    string ImagePath = Path.Combine(_env.WebRootPath, "Uploads/Customer_Images", customer_image.FileName);
                    using (var stream = new FileStream(ImagePath, FileMode.Create))
                    {
                        customer_image.CopyTo(stream);
                    }
                    customer.customer_image = customer_image.FileName;
                }

                _context.tbl_customer.Update(customer);
                _context.SaveChanges();
                return RedirectToAction("CustomerProfile");
            }
        }

        // Feedback (GET)
        public IActionResult Feedback()
        {
            var session = SessionHelper.GetCustomerSession(); // Use SessionHelper

            if (session == null)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            else
            {
                return View();
            }
        }

        // Feedback (POST)
        [HttpPost]
        public IActionResult Feedback(Feedback feedback)
        {
            var session = SessionHelper.GetCustomerSession(); // Use SessionHelper

            if (session == null)
            {
                ViewBag.message = "Error Occurred. Please log in.";
                return RedirectToAction("CustomerLogin", "Customer");
            }
            else
            {
                _context.tbl_feedback.Add(feedback);
                _context.SaveChanges();
                ViewBag.message = "Thank You For Your Feedback!";
                return View();
            }
        }


        // /Customer/Payment/{id}
        [HttpGet]
        public IActionResult Payment()
        {
            var session = SessionHelper.GetCustomerSession();
            if (session == null)
            {
                return RedirectToAction("CustomerLogin");
            }

            int customerId = int.Parse(session);
            var cartItems = _context.tbl_cart
                .Include(c => c.Product)
                .Where(c => c.cust_id == customerId && c.cart_status == 1)
                .ToList();

            if (!cartItems.Any())
            {
                return RedirectToAction("ViewCart");
            }

            decimal totalAmount = cartItems.Sum(c => (decimal)c.Product.product_price * c.product_quantity);

            ViewBag.CartItems = cartItems;
            ViewBag.TotalAmount = totalAmount;

            return View();
        }


        // add to cart 

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var session = SessionHelper.GetCustomerSession();
            if (session == null)
            {
                return RedirectToAction("CustomerLogin");
            }

            var customerId = int.Parse(session);
            var product = _context.tbl_product.FirstOrDefault(p => p.product_id == productId);
            if (product == null)
            {
                return NotFound();
            }

            var cartItem = _context.tbl_cart
                .FirstOrDefault(c => c.prod_id == productId && c.cust_id == customerId);

            if (cartItem != null)
            {
                // Update quantity if the product is already in the cart
                cartItem.product_quantity += quantity;
            }
            else
            {
                // Add new item to the cart
                cartItem = new Cart
                {
                    prod_id = productId,
                    cust_id = customerId,
                    product_quantity = quantity,
                    cart_status = 1 // Active
                };
                _context.tbl_cart.Add(cartItem);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        // view cart

        [HttpGet]
        public IActionResult ViewCart()
        {
            var session = SessionHelper.GetCustomerSession();
            if (session == null)
            {
                return RedirectToAction("CustomerLogin");
            }

            var customerId = int.Parse(session);
            var cartItems = _context.tbl_cart
                .Where(c => c.cust_id == customerId && c.cart_status == 1).Include(c => c.Product) // Include product details
                .ToList();

            return View(cartItems);
        }
        //payment process for esewa


        // payment process Khalti 
        [HttpPost]
        public async Task<IActionResult> InitiateKhaltiPayment()
        {
            var session = SessionHelper.GetCustomerSession();
            if (session == null) return RedirectToAction("CustomerLogin");

            int customerId = int.Parse(session);
            var cartItems = _context.tbl_cart
                .Include(c => c.Product)
                .Where(c => c.cust_id == customerId && c.cart_status == 1)
                .ToList();

            if (!cartItems.Any()) return RedirectToAction("Error");

            decimal totalAmount = cartItems.Sum(c => (decimal)c.Product.product_price * c.product_quantity);

            var customer = _context.tbl_customer.Find(customerId);
            var productNames = string.Join(", ", cartItems.Select(c => c.Product.product_name));

            var payload = new
            {
                return_url = "https://localhost:7247/Customer/PaymentCallback",
                website_url = "https://localhost:7247",
                amount = totalAmount * 100, // Convert to paisa
                purchase_order_id = Guid.NewGuid().ToString(),
                purchase_order_name = $"Order_{DateTime.Now:yyyyMMddHHmmss}",
                customer_info = new
                {
                    name = customer.customer_name,
                    email = customer.customer_email,
                    phone = customer.customer_phone
                }
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "key live_secret_key_68791341fdd94846a146f0457ff7b455");

            using var response = await client.PostAsync("https://dev.khalti.com/api/v2/epayment/initiate/", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var khaltiResponse = JsonConvert.DeserializeObject<KhaltiResponse>(responseContent);
                return Redirect(khaltiResponse.payment_url);
            }
            return View("Error");
        }


        [HttpGet]
        public async Task<IActionResult> PaymentCallback(string pidx, string status, string transaction_id, string amount, string mobile, string purchase_order_id, string purchase_order_name)
        {
            if (status == "Completed")
            {
                var lookupResponse = await VerifyKhaltiPayment(pidx);
                if (lookupResponse?.status == "Completed")
                {
                    var session = SessionHelper.GetCustomerSession();
                    if (session == null) return RedirectToAction("CustomerLogin");

                    int customerId = int.Parse(session);
                    var cartItems = _context.tbl_cart
                        .Where(c => c.cust_id == customerId && c.cart_status == 1)
                        .ToList();

                    // Mark cart items as purchased
                    foreach (var item in cartItems)
                    {
                        item.cart_status = 0; // 0 = Inactive
                    }
                    _context.SaveChanges();

                    // Get customer details
                    var customer = _context.tbl_customer.Find(customerId);
                    decimal totalAmount = decimal.Parse(amount) / 100;

                    return RedirectToAction("PaymentSuccess", new
                    {
                        transactionId = transaction_id,
                        totalAmount = totalAmount,
                        paymentDate = DateTime.Now,
                        customerName = customer.customer_name,
                        customerEmail = customer.customer_email
                    });
                }
            }
            return RedirectToAction("PaymentFailed");
        }


        private async Task<KhaltiLookupResponse> VerifyKhaltiPayment(string pidx)
        {
            var url = "https://dev.khalti.com/api/v2/epayment/lookup/";
            var payload = new { pidx = pidx };
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "key live_secret_key_68791341fdd94846a146f0457ff7b455");

            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<KhaltiLookupResponse>(responseContent);
            }
            else
            {
                return null;
            }
        }
        [HttpGet]
        public IActionResult PaymentSuccess(string transactionId, decimal totalAmount, DateTime paymentDate, string customerName, string customerEmail)
        {
            var session = SessionHelper.GetCustomerSession();
            if (session == null) return RedirectToAction("CustomerLogin");

            int customerId = int.Parse(session);
            var cartItems = _context.tbl_cart
                .Include(c => c.Product)
                .Where(c => c.cust_id == customerId && c.cart_status == 0)
                .ToList();

            var receipt = new PaymentReceiptViewModel
            {
                TransactionId = transactionId,
                TotalAmount = totalAmount,
                PaymentDate = paymentDate,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                Items = cartItems.Select(c => new CartItemDetail
                {
                    ProductName = c.Product.product_name,
                    Quantity = c.product_quantity,
                    Price = c.Product.product_price
                }).ToList()
            };

            return View(receipt);
        }



        public IActionResult PaymentFailed()
        {
            return View();
        }


    }
}