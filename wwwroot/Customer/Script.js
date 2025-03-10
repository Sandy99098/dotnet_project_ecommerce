// Smooth scroll for anchor links
document.addEventListener("DOMContentLoaded", function () {
    // Add event listeners to all anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener("click", function (event) {
            event.preventDefault(); // Prevent default anchor behavior
            const targetId = this.getAttribute("href"); // Get the target section ID
            const targetElement = document.querySelector(targetId); // Find the target element

            if (targetElement) {
                // Smoothly scroll to the target element
                targetElement.scrollIntoView({
                    behavior: "smooth",
                    block: "start"
                });

                // Update the URL hash
                window.location.hash = targetId;
            }
        });
    });

    // Close mobile menu on link click
    document.querySelectorAll(".menu-items a").forEach(link => {
        link.addEventListener("click", function () {
            document.getElementById("checkbox").checked = false; // Uncheck the checkbox
        });
    });

    // Event listeners for "Buy Now" buttons
    document.querySelectorAll(".buy-now-btn").forEach(button => {
        button.addEventListener("click", function () {
            // Extract product details from data attributes
            const productName = this.getAttribute("data-product-name");
            const productPrice = this.getAttribute("data-product-price");

            // Redirect to the payment page with product details
            window.location.href = `/Payment?productName=${encodeURIComponent(productName)}&productPrice=${encodeURIComponent(productPrice)}`;
        });
    });
});



// for the form

// Get elements
const loginForm = document.getElementById("login-form");
const signupForm = document.getElementById("signup-form");
const formTitle = document.getElementById("form-title");

document.getElementById("show-signup").addEventListener("click", function () {
    loginForm.classList.add("hidden");
    signupForm.classList.remove("hidden");
    formTitle.innerText = "Sign Up";
});

document.getElementById("show-login").addEventListener("click", function () {
    signupForm.classList.add("hidden");
    loginForm.classList.remove("hidden");
    formTitle.innerText = "Login";
});


// showing the product respective to its categories as per user press the shopnow button
    function filterProducts(category) {
        // Hide all product sections
        document.querySelectorAll('.seller.container').forEach(section => {
            section.style.display = 'none';
        });

    // Show the selected category section
    const selectedSection = document.getElementById(category);
    if (selectedSection) {
        selectedSection.style.display = 'block';
        }
    }

    // Add event listeners to the SHOP NOW buttons
    document.querySelectorAll('.img-content button a').forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault(); // Prevent default anchor behavior
            const category = this.getAttribute('href').substring(1); // Get category from href
            filterProducts(category); // Filter products
            document.getElementById(category).scrollIntoView({ behavior: 'smooth' }); // Scroll to section
        });
    });
