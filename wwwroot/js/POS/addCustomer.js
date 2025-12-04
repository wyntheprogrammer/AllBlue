document.addEventListener("DOMContentLoaded", function () {
    const button = document.getElementById("openAddCustomerModal");
    const container = document.getElementById("modalContainer");

    button.addEventListener("click", function (){
        fetch("/POS/AddCustomer")
            .then(response => response.text())
            .then(html => {
                container.innerHTML = html;
                const modal = document.getElementById("addCustomerModal");
                if(modal) {
                    const modalInstance = new bootstrap.Modal(modal);
                    modalInstance.show();
                }
            })
            .catch(error => {
                console.error("Error loading modal:", error);
            });
            });
        });