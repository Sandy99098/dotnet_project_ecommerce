// Get elements
const loginForm = document.getElementById("login-form");
const signupForm = document.getElementById("signup-form");
const formTitle = document.getElementById("form-title");

document.getElementById("show-signup").addEventListener("click", function() {
    loginForm.classList.add("hidden");
    signupForm.classList.remove("hidden");
    formTitle.innerText = "Sign Up";
});

document.getElementById("show-login").addEventListener("click", function() {
    signupForm.classList.add("hidden");
    loginForm.classList.remove("hidden");
    formTitle.innerText = "Login";
});
