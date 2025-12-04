document.addEventListener("DOMContentLoaded", function () {
    const button = document.getElementById("openSearchCustomerModal");
    const container = document.getElementById("modalContainer");

    button.addEventListener("click", function () {
        fetch("/POS/SearchCustomer")
            .then(response => response.text())
            .then(html => {
                container.innerHTML = html;
                const modal = document.getElementById("searchCustomerModal");
                if (modal) {
                    const modalInstance = new bootstrap.Modal(modal);
                    modalInstance.show();
                }
            });
    });

    container.addEventListener("click", function (e) {
        if (e.target.classList.contains("pagination-link")) {
            e.preventDefault();
            loadFilteredResults(e.target.getAttribute("href"));
        }
    });

    container.addEventListener("click", function (e) {
        if (e.target.id === "applyFilter") {
            e.preventDefault();
            loadFilteredResults("/POS/SearchCustomer");
        }
    });

    // Handle row click
    container.addEventListener("click", function (e) {
        const row = e.target.closest(".clickable-row");
        if (row) {
            const customerId = row.getAttribute("data-id");
            if (customerId) {
                window.location.href = `/POS/Index?id=${customerId}`;
            }
        }
    });

    function loadFilteredResults(baseUrl) {
        const keyword = document.getElementById("searchKeyword").value;
        const barangay = document.getElementById("filterBarangay").value;
        const city = document.getElementById("filterCity").value;

        const url = new URL(baseUrl, window.location.origin);
        url.searchParams.set("keyword", keyword);
        url.searchParams.set("selectedBarangayID", barangay);
        url.searchParams.set("selectedCityID", city);

        fetch(url, {
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .then(response => response.text())
        .then(html => {
            const modalBody = document.querySelector("#searchCustomerModal #modalBodyContent");
            if (modalBody) {
                modalBody.innerHTML = html;
            }
        });
    }
});