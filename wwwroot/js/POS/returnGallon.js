document.addEventListener("DOMContentLoaded", function () {
    const button = document.getElementById("openReturnGallonModal");
    const container = document.getElementById("modalContainer");

    button.addEventListener("click", function (){
        fetch("/POS/ReturnGallon")
            .then(response => response.text())
            .then(html => {
                container.innerHTML = html;
                const modal = document.getElementById("returnGallonModal");
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