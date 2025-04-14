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
using dotnet_project_ecommerce.ViewModels.Payment;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using dotnet_project_ecommerce.Models.ViewModels;
using System.Net.Http.Headers;

namespace dotnet_project_ecommerce.Controllers
{
    public class CustomerController : Controller
    {
        private readonly myContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomerController> _logger; // Correct type

        public CustomerController(
            myContext context,
            IWebHostEnvironment env,
            IConfiguration configuration,
            ILogger<CustomerController> logger) // Correct parameter type
        {
            _context = context;
            _env = env;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Index()
        {
            var products = _context.tbl_product.ToList(); // Fetch products
            var categories = _context.tbl_category.ToList(); // Fetch categories
            ViewData["categories"] = categories;
            ViewBag.CheckSession = SessionHelper.GetCustomerSession(); // Use SessionHelper
            return View(products);
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
            var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_email == email && c.customer_password == password);

            if (customer != null)
            {
                SessionHelper.SetCustomerSession(customer.customer_id.ToString());
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
                customer_password = password
            };

            _context.tbl_customer.Add(newCustomer);
            _context.SaveChanges();

            return RedirectToAction("CustomerLogin");
        }

        // Customer Logout
        public IActionResult CustomerLogOut()
        {
            SessionHelper.RemoveCustomerSession();
            return RedirectToAction("Index");
        }

        // Customer Profile
        [HttpGet]
        public IActionResult CustomerProfile()
        {
            var session = SessionHelper.GetCustomerSession();

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
            var session = SessionHelper.GetCustomerSession();

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
            var session = SessionHelper.GetCustomerSession();

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

        // Feedback
        [HttpGet]
        public IActionResult Feedback()
        {
            var session = SessionHelper.GetCustomerSession();

            if (session == null)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            else
            {
                return View();
            }
        }

        // Feedback 
        [HttpPost]
        public IActionResult Feedback(Feedback feedback)
        {
            var session = SessionHelper.GetCustomerSession();

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
        //// add to cart [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return BadRequest("Invalid quantity.");
                }

                var session = SessionHelper.GetCustomerSession();
                if (string.IsNullOrEmpty(session))
                {
                    TempData["Error"] = "Session expired. Please login again.";
                    return RedirectToAction("CustomerLogin");
                }

                int customerId = int.Parse(session);

                var product = _context.tbl_product.FirstOrDefault(p => p.product_id == productId);
                if (product == null)
                {
                    return NotFound("Product not found.");
                }

                var existingCartItem = _context.tbl_cart
                    .FirstOrDefault(c => c.prod_id == productId && c.cust_id == customerId && c.cart_status == 1);

                if (existingCartItem != null)
                {
                    existingCartItem.product_quantity += quantity;
                }
                else
                {
                    var cartItem = new Cart
                    {
                        prod_id = productId,
                        cust_id = customerId,
                        product_quantity = quantity,
                        cart_status = 1
                    };
                    _context.tbl_cart.Add(cartItem);
                }

                _context.SaveChanges();
                return RedirectToAction("ViewCart", new { customerId = customerId });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error adding to cart: {ex.Message}");
                TempData["Error"] = "An error occurred while adding the item to the cart.";
                return RedirectToAction("Index");
            }
        }


        // vuew 
        public IActionResult ViewCart(int customerId)
        {
            var cartItems = _context.tbl_cart
                .Include(c => c.Product)
                .Where(c => c.cust_id == customerId && c.cart_status == 1)
                .ToList();
            return View(cartItems);
        }
        // checkout 
        public IActionResult Checkout(int customerId)
        {
            // Fetch cart items for the customer
            var cartItems = _context.tbl_cart
                .Include(c => c.Product)
                .Where(c => c.cust_id == customerId && c.cart_status == 1)
                .ToList();

            // Check if the cart is empty
            if (!cartItems.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("ViewCart", new { customerId = customerId });
            }

            // Calculate total amount
            decimal totalAmount = cartItems.Sum(c => c.TotalPrice);

            // Pass data to the view
            ViewBag.TotalAmount = totalAmount;
            ViewBag.CustomerId = customerId;
            return View(cartItems);
        }



        //remove from the cart
        [HttpPost]
        public IActionResult RemoveFromCart(int cartId)
        {
            var cartItem = _context.tbl_cart.FirstOrDefault(c => c.cart_id == cartId);
            if (cartItem != null)
            {
                _context.tbl_cart.Remove(cartItem);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewCart", new { customerId = cartItem.cust_id });
        }




        //product details 
        public IActionResult ProductDetail(int product_id)
        {
            var product = _context.tbl_product.FirstOrDefault(p => p.product_id == product_id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }



        // /Customer/Payment/{id}
        //public IActionResult Payment(int id)
        //{
        //    var product = _context.tbl_product.FirstOrDefault(p => p.product_id == id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        public IActionResult Payment(int id)
        {
            try
            {
                var product = _context.tbl_product.FirstOrDefault(p => p.product_id == id);

                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Error");
                }

                return View(product);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in Payment action: {ex.Message}");
                TempData["Error"] = "An error occurred while processing your request.";
                return RedirectToAction("Error");
            }
        }


        // Payment process for Khalti (POST)
        [HttpPost]

        public async Task<IActionResult> InitiateKhaltiPayment(int productId, int quantity)
        {
            try
            {
                // Debug logging
                Console.WriteLine($"Starting payment for product {productId}, quantity {quantity}");

                var session = SessionHelper.GetCustomerSession();
                if (string.IsNullOrEmpty(session))
                {
                    TempData["Error"] = "Session expired. Please login again.";
                    return RedirectToAction("CustomerLogin");
                }

                var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_id.ToString() == session);
                if (customer == null)
                {
                    TempData["Error"] = "Customer not found.";
                    return RedirectToAction("CustomerLogin");
                }

                var product = _context.tbl_product.FirstOrDefault(p => p.product_id == productId);
                if (product == null)
                {
                    TempData["Error"] = "Product not available.";
                    return RedirectToAction("Index");
                }

                decimal totalPriceInPaisa = product.product_price * quantity * 100;
                Console.WriteLine($"Calculated amount: {totalPriceInPaisa} paisa");

                var payload = new
                {
                    return_url = $"{Request.Scheme}://{Request.Host}/Customer/PaymentCallback",
                    website_url = $"{Request.Scheme}://{Request.Host}",
                    amount = totalPriceInPaisa,
                    purchase_order_id = Guid.NewGuid().ToString(),
                    purchase_order_name = product.product_name,
                    customer_info = new
                    {
                        name = customer.customer_name,
                        email = customer.customer_email,
                        phone = customer.customer_phone
                    }
                };

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "key live_secret_key_68791341fdd94846a146f0457ff7b455");

                var response = await client.PostAsync(
                   "https://a.khalti.com/api/v2/epayment/initiate/"
,
                    new StringContent(JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"));

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Khalti API response: {response.StatusCode} - {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Payment gateway error. Please try again."; 
                    return RedirectToAction("Payment", new { id = productId });
                }

                var khaltiResponse = JsonConvert.DeserializeObject<KhaltiResponse>(responseContent);
                return Redirect(khaltiResponse.payment_url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment error: {ex}");
                TempData["Error"] = "An error occurred during payment processing.";
                return RedirectToAction("PaymentFailed");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallback(string pidx, string status, string transaction_id, string amount, string mobile, string purchase_order_id, string purchase_order_name)
        {
            if (status == "Completed")
            {
                var lookupResponse = await VerifyKhaltiPayment(pidx);
                if (lookupResponse != null && lookupResponse.status == "Completed")
                {
                    var session = SessionHelper.GetCustomerSession();
                    if (session == null)
                    {
                        return RedirectToAction("CustomerLogin");
                    }

                    var customer = _context.tbl_customer.FirstOrDefault(c => c.customer_id.ToString() == session);
                    if (customer == null)
                    {
                        return RedirectToAction("CustomerLogin");
                    }

                    var product = _context.tbl_product.FirstOrDefault(p => p.product_name == purchase_order_name);
                    if (product == null)
                    {
                        return NotFound();
                    }

                    decimal amountInRupees = decimal.Parse(amount) / 100;


                    //decimal productPriceInNPR = Convert.ToDecimal(product.product_price);

                    decimal totalAmount = amountInRupees + Convert.ToDecimal(5.65);

                    return RedirectToAction("PaymentSuccess", new
                    {
                        transactionId = transaction_id,
                        amount = amountInRupees,
                        totalAmount = totalAmount,
                        productName = purchase_order_name,
                        customerName = customer.customer_name,
                        customerEmail = customer.customer_email,
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
        public IActionResult PaymentSuccess(string transactionId, decimal amount, decimal totalAmount, string productName, string customerName, string customerEmail)
        {
            var receipt = new KhaltiPaymentReceiptViewModel
            {
                TransactionId = transactionId,
                Amount = amount,
                TotalAmount = totalAmount,
                PaymentDate = DateTime.Now,
                ProductName = productName,
                CustomerName = customerName,
                CustomerEmail = customerEmail
            };

            return View(receipt);
        }

        public IActionResult PaymentFailed()
        {
            return View();
        }




            // esewa 
            [HttpPost]
            public async Task<IActionResult> InitiateEsewaPayment(int productId, int quantity)
            {
                try
                {
                    var session = SessionHelper.GetCustomerSession();
                    if (string.IsNullOrEmpty(session))
                    {
                        TempData["Error"] = "Session expired. Please login again.";
                        return RedirectToAction("CustomerLogin");
                    }
                    // Constants for eSewa configuration
                    const string productCode = "EPAYTEST";
                    const string secretKey = "8gBm/:&EnhH.1/q"; 
                    const string initiateUrl = "https://rc-epay.esewa.com.np/api/epay/main/v2/form";

                    // Retrieve customer and product information
                    var customer = await _context.tbl_customer
                        .FirstOrDefaultAsync(c => c.customer_id.ToString() == session);

                    var product = await _context.tbl_product
                        .FirstOrDefaultAsync(p => p.product_id == productId);

                    // Validate inputs
                    if (product == null || customer == null || quantity <= 0)
                    {
                        _logger.LogWarning("Invalid product, customer, or quantity. Product ID: {productId}, Quantity: {quantity}, Session: {session}", productId, quantity, session);
                        return RedirectToAction("PaymentFailed");
                    }

                    // Calculate amounts (amount, taxAmount, totalAmount)
                    var (amount, taxAmount, totalAmount) = CalculateAmounts(product.product_price, quantity);

                    // Generate transaction UUID before saving the transaction record
                   var transactionUuid = Guid.NewGuid().ToString("N");
                HttpContext.Session.SetString("EsewaTxnUuid", transactionUuid);

                // Create transaction record with the generated UUID
                var transaction = new PaymentTransaction
                    {
                        TransactionId = transactionUuid,
                        TransactionCode = "TXN" + DateTime.UtcNow.Ticks.ToString(),
                        CustomerId = customer.customer_id,
                        ProductId = productId,
                        Quantity = quantity,
                        Amount = amount,
                        TaxAmount = taxAmount,
                        TotalAmount = totalAmount,
                        PaymentMethod = "eSewa",
                        Status = "Initiated",
                        CreatedAt = DateTime.Now,
                    };

                    _context.PaymentTransactions.Add(transaction);
                    await _context.SaveChangesAsync();
                    // Correct order: total_amount,transaction_uuid,product_code
                    var dataToSign = $"total_amount={totalAmount:F2},transaction_uuid={transactionUuid},product_code={productCode}";

                    var signature = GenerateEsewaSignature(dataToSign, secretKey);

                    // Prepare eSewa payment request with signing
                    var esewaRequest = new EsewaPaymentRequest
                    {
                        amount = amount.ToString("0.00"),
                        tax_amount = taxAmount.ToString("0.00"),
                        total_amount = totalAmount.ToString("0.00"),
                        transaction_uuid = transactionUuid,
                        product_code = productCode,
                        product_service_charge = "0.00",
                        product_delivery_charge = "0.00",
                        success_url = $"{Request.Scheme}://{Request.Host}/Customer/EsewaPaymentSuccess?transaction_uuid={transactionUuid}",
                        failure_url = $"{Request.Scheme}://{Request.Host}/Customer/PaymentFailed",
                        signed_field_names = "total_amount,transaction_uuid,product_code",
                        signature = GenerateEsewaSignature(
                            $"total_amount={totalAmount:F2},transaction_uuid={transactionUuid},product_code={productCode}",
                            secretKey
                        )
                    };

                // Build auto-submit form to post to eSewa
                return Content(BuildEsewaForm(esewaRequest, initiateUrl), "text/html");
                //var html = BuildEsewaForm(esewaRequest, initiateUrl);  // Build the form
                //return Content(html); // Return it as raw HTML (just for debugging)

            }
            catch (Exception ex)
                {
                _logger.LogInformation($"QueryString received: {Request.QueryString}");

                _logger.LogError(ex, "eSewa payment initiation failed for product {productId}, quantity {quantity}", productId, quantity);
                    return RedirectToAction("PaymentFailed");
                }
            }

            private (decimal amount, decimal taxAmount, decimal totalAmount) CalculateAmounts(decimal price, int quantity)
            {
                decimal amount = Math.Round(price * quantity, 2);
                decimal taxAmount = Math.Round(amount * 0.13m, 2); // 13% VAT
                decimal totalAmount = Math.Round(amount + taxAmount, 2);
                return (amount, taxAmount, totalAmount);
            }

            private string BuildEsewaForm(EsewaPaymentRequest request, string initiateUrl)
            {
                        return $@"
                         <form id='esewaForm' action='{initiateUrl}' method='POST' style='display:none;'>
                    {GenerateFormField("amount", request.amount)}
                    {GenerateFormField("tax_amount", request.tax_amount)}
                    {GenerateFormField("total_amount", request.total_amount)}
                    {GenerateFormField("transaction_uuid", request.transaction_uuid)}
                    {GenerateFormField("product_code", request.product_code)}
                    {GenerateFormField("product_service_charge", request.product_service_charge)}
                    {GenerateFormField("product_delivery_charge", request.product_delivery_charge)}
                    {GenerateFormField("success_url", request.success_url)}
                    {GenerateFormField("failure_url", request.failure_url)}
                    {GenerateFormField("signed_field_names", request.signed_field_names)}
                    {GenerateFormField("signature", request.signature)}
                    </form>
                    <script>document.getElementById('esewaForm').submit();</script>";
            }

            private string GenerateFormField(string name, string value)
            {
                return $"<input type='hidden' name='{name}' value='{value}' />";
            }

       
        private string GenerateEsewaSignature(string data, string secretKey)
            {
                try
                {
                    _logger.LogInformation($"Raw data to sign: {data}");

                    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
                    {
                        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                        string signature = Convert.ToBase64String(hashBytes);

                        _logger.LogInformation($"Generated signature: {signature}");
                    
                        return signature;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating eSewa signature");
                    throw;
                }
        
            }

        private async Task<EsewaPaymentResponse> VerifyEsewaPayment(string rawTransactionUuid)
        {
            try
            {
                // ✅ Sanitize the transaction UUID (remove ?data=... or anything after ?)
                var transactionUuid = rawTransactionUuid?.Split('?')[0]?.Trim();

                if (string.IsNullOrEmpty(transactionUuid))
                {
                    _logger.LogWarning("Empty or invalid transaction UUID during verification.");
                    return null;
                }

                var verifyUrl = _configuration["PaymentGateways:eSewa:VerifyUrl"];
                var url = $"{verifyUrl}?transaction_uuid={Uri.EscapeDataString(transactionUuid)}";

                var merchantId = _configuration["PaymentGateways:eSewa:MerchantId"];
                var secretKey = _configuration["PaymentGateways:eSewa:SecretKey"];
                var authString = $"{merchantId}:{secretKey}";
                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                _logger.LogInformation($"Verifying payment for UUID: {transactionUuid}");
                _logger.LogInformation($"Verification URL: {url}");
                _logger.LogInformation($"Auth token: {authToken}");

                var response = await client.GetAsync(url);
                _logger.LogInformation($"Verification status code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Verification response: {content}");

                    var result = JsonConvert.DeserializeObject<EsewaPaymentResponse>(content);
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Verification failed with status code: {response.StatusCode}");
                    _logger.LogError($"Verification response body: {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment verification error");
                return null;
            }
        }



        [HttpGet]
        public async Task<IActionResult> EsewaPaymentSuccess([FromQuery] string transaction_uuid)
        {
            try
            {
                _logger.LogInformation($"Received Esewa callback with UUID: {transaction_uuid}");

                if (string.IsNullOrEmpty(transaction_uuid))
                {
                    _logger.LogWarning("Missing transaction UUID in callback");
                    return RedirectToAction("PaymentFailed");
                }

                var paymentResponse = await VerifyEsewaPayment(transaction_uuid);

                if (paymentResponse?.Status?.ToUpper() != "COMPLETED")
                {
                    _logger.LogWarning($"Payment not completed. Status: {paymentResponse?.Status}");
                    return RedirectToAction("PaymentFailed");
                }

                var transaction = await _context.PaymentTransactions
                    .Include(t => t.Product)
                    .Include(t => t.Customer)
                    .FirstOrDefaultAsync(t => t.TransactionId == transaction_uuid);

                if (transaction == null)
                {
                    _logger.LogError($"Transaction not found: {transaction_uuid}");
                    return RedirectToAction("PaymentFailed");
                }

                // Update transaction
                transaction.Status = "Completed";
                transaction.TransactionCode = paymentResponse.TransactionCode;
                transaction.CompletedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // Prepare receipt
                return View(new EsewaPaymentReceiptViewModel
                {
                    TransactionId = transaction.TransactionId,
                    TransactionCode = transaction.TransactionCode,
                    Amount = transaction.Amount,
                    TaxAmount = transaction.TaxAmount,
                    TotalAmount = transaction.TotalAmount,
                    ProductName = transaction.Product?.product_name ?? "Unknown Product",
                    CustomerName = transaction.Customer?.customer_name ?? "Unknown Customer",
                    CustomerEmail = transaction.Customer?.customer_email ?? "N/A",
                    PaymentDate = transaction.CompletedAt.Value,
                    PaymentMethod = transaction.PaymentMethod
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing successful payment");
                return RedirectToAction("PaymentFailed");
            }
        }



        public IActionResult EsewaPaymentSuccess()
            {
            if (TempData["Receipt"] is string receiptJson)
            {
                var receipt = JsonConvert.DeserializeObject<EsewaPaymentReceiptViewModel>(receiptJson);
                return View(receipt);
            }
            return RedirectToAction("PaymentFailed");

            }
        }
}