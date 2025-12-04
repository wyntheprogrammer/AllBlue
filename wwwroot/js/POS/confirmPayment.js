document.addEventListener("DOMContentLoaded", function () {
    const button = document.getElementById("openConfirmPaymentModal");
    const container = document.getElementById("modalContainer");

    if (!button) {
        console.warn("No button with id 'openConfirmPaymentModal' found.");
        return;
    }

    if (!container) {
        console.warn("No container with id 'confirmPaymentContainer' found.");
        return;
    }

    button.addEventListener("click", function () {
        const customerID = button.dataset.customerId;

        const selectUserElem = document.getElementById("select-user");
        const selectedUserId = selectUserElem ? selectUserElem.value : null;
        if (!selectedUserId) {
            alert("Please select a user first.");
            return;
        }

        const selectServiceElem = document.getElementById("select-service");
        const selectedService = selectServiceElem ? selectServiceElem.value : null;
        if (!selectedService) {
            alert("Please select a service first.");
            return;
        }

        const dateElem = document.getElementById("date");
        const date = dateElem ? dateElem.value : null;

        // Collect all rows
        const rows = document.querySelectorAll("#gallonContainer tbody tr, #bottleContainer tbody tr");
        const items = [];

        rows.forEach(row => {
            const itemIdCell = row.querySelector("th[scope=row]");
            if (!itemIdCell) return;

            const itemID = parseInt(itemIdCell.innerText.trim(), 10);
            const clientGal = parseInt(row.querySelector(".client-gal")?.value || "0", 10);
            const wrsGal = parseInt(row.querySelector(".wrs-gal")?.value || "0", 10);
            const freeGal = parseInt(row.querySelector(".free-gal")?.value || "0", 10);
            const total = parseInt(parseFloat(row.querySelector(".total-cell")?.innerText.trim()) || 0, 10);

            const qty = clientGal + wrsGal + freeGal;

            if (qty > 0) {
                items.push({
                    ItemID: itemID,
                    ClientGal: clientGal,
                    WRSGal: wrsGal,
                    FreeGal: freeGal,
                    Qty: qty,
                    Total: total
                });
            }
        });

        if (items.length === 0) {
            alert("Please input gallons for at least one item.");
            return;
        }

        const totalQtyElem = document.querySelector("#summaryTable #total-qty");
        const totalPriceElem = document.querySelector("#summaryTable #total-price");

        const totalQty = totalQtyElem ? parseInt(totalQtyElem.innerText.trim() || "0", 10) : 0;
        const totalPrice = totalPriceElem ? parseInt(parseFloat(totalPriceElem.innerText.trim() || "0"), 10) : 0;
        const totalFree = items.reduce((sum, item) => sum + item.FreeGal, 0);

        const payload = {
            CustomerID: parseInt(customerID, 10),
            SelectedUserID: parseInt(selectedUserId, 10),
            SelectedService: selectedService,
            Items: items,
            TotalQty: totalQty,
            Free: totalFree,
            TotalPrice: totalPrice,
            Discount: "",
            Cash: 0,
            Changed: 0,
            Note: "",
            Status: "Unpaid",
            Date: date
        };

        console.log("Payload:", JSON.stringify(payload));

        fetch("/POS/ConfirmPayment", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        })
        .then(response => {
            if (!response.ok) throw new Error("Server returned " + response.status);
            return response.text();
        })
        .then(html => {
            container.innerHTML = html;
            const modal = document.getElementById("confirmPaymentModal");
            if (!modal) {
                console.warn("No modal element found in the response.");
                return;
            }

            const modalInstance = new bootstrap.Modal(modal);
            modalInstance.show();

            const totalDue = document.getElementById('totalDue');
            const cashInput = document.getElementById('cashInput');
            const changeInput = document.getElementById('changeInput');
            const statusInput = document.getElementById('status');
            const discountInput = document.getElementById('discountInput');
            const balancedInput = document.getElementById('balanced');

            const noteContainer = document.getElementById('noteContainer');

            if (!totalDue || !cashInput || !changeInput || !statusInput || !discountInput) {
                console.warn("Some modal input elements are missing.");
                return;
            }

            function updateStatus() {
                let total = parseFloat(totalDue.textContent) || 0;
                let cash = parseFloat(cashInput.value) || 0;
                let discountText = discountInput.value.trim();

                let balance = Math.max(total - cash, 0);

                balancedInput.value = balance;

                if (discountText !== "") {
                    statusInput.value = 'Discount';
                    changeInput.value = 0;
                } else if (cash >= total) {
                    statusInput.value = 'Paid';
                    changeInput.value = (cash - total);
                } else if (cash > 0 && cash < total) {
                    statusInput.value = `Balance: ${(balance)}`;
                    changeInput.value = 0;
                } else {
                    statusInput.value = 'Unpaid';
                    changeInput.value = 0;
                }


                //Show note when discount has value, hide when empty
                if(discountText != "")
                {
                    noteContainer.style.display = "flex";
                    noteContainer.style.flexDirection = "column";
                } else {
                    noteContainer.style.display = "none";
                }
            }

            cashInput.addEventListener('input', updateStatus);
            discountInput.addEventListener('input', updateStatus);
        })
        .catch(error => {
            console.error("Error loading modal:", error);
            alert("Failed to load payment modal. Check console for details.");
        });
    });
});
