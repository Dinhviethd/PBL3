// Form handling
document.getElementById("userForm").addEventListener("submit", function (e) {
    e.preventDefault();
    const formData = new FormData(this);
    const userData = Object.fromEntries(formData.entries());
    console.log("Form submitted:", userData);
    // Add your form submission logic here
});

// Search functionality
document.querySelector(".search-btn").addEventListener("click", function () {
    const searchInputs = document.querySelectorAll(".search-input");
    const searchData = Array.from(searchInputs).map((input) => ({
        field: input.placeholder,
        value: input.value,
    }));
    console.log("Search criteria:", searchData);
    // Add your search logic here
});

// Pagination
document.querySelectorAll(".page-btn").forEach((button) => {
    button.addEventListener("click", function () {
        if (this.classList.contains("prev")) {
            console.log("Previous page");
            // Add previous page logic
        } else if (this.classList.contains("next")) {
            console.log("Next page");
            // Add next page logic
        } else {
            const page = this.textContent;
            console.log("Go to page:", page);
            // Update active state
            document
                .querySelectorAll(".page-btn")
                .forEach((btn) => btn.classList.remove("active"));
            this.classList.add("active");
            // Add page navigation logic
        }
    });
});

// Responsive handling
function handleResponsive() {
    const width = window.innerWidth;
    const dashboard = document.querySelector(".dashboard-content");
    if (width <= 768) {
        dashboard.classList.add("mobile");
    } else {
        dashboard.classList.remove("mobile");
    }
}

window.addEventListener("resize", handleResponsive);
handleResponsive();