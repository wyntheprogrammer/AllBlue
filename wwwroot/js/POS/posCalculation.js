document.addEventListener('DOMContentLoaded', function () {
    const allInputs = document.querySelectorAll('input[data-item-name]');
    const deliveryModeSelect = document.querySelector('.delivery-mode');
    const summaryTableBody = document.querySelector('#summaryTable tbody');
    const subtotalDisplay = document.querySelector('#subtotal');

    function updatePricesByMode() {
        const mode = deliveryModeSelect.value;
        allInputs.forEach(input => {
            const newPrice = mode === 'Pickup' ? input.dataset.pickup : input.dataset.deliver;
            input.dataset.price = newPrice;

            const row = input.closest('tr');
            const priceCell = row.querySelector('.price-cell');
            if(priceCell) {
                priceCell.textContent = `P ${parseFloat(newPrice).toFixed(2)}`;
            }
        });
    }


    function updateSummary() {
        let summaryMap = {};
        let totalQty = 0;
        let totalPrice = 0;

        // Loop through each table row
        document.querySelectorAll('#gallonContainer tbody tr, #bottleContainer tbody tr').forEach(row => {
            const price = parseFloat(row.querySelector('input[data-item-name]').dataset.price) || 0;

            const client = parseFloat(row.querySelector('.client-gal')?.value) || 0;
            const wrs = parseFloat(row.querySelector('.wrs-gal')?.value) || 0;
            const freeGal = parseFloat(row.querySelector('.free-gal')?.value) || 0;

            const itemName = row.querySelector('input[data-item-name]')?.dataset.itemName;
            const totalCell = row.querySelector('.total-cell');

            // PAID gallons = client + wrs only
            const paidGallons = client + wrs;

            // PER ROW total
            const rowTotal = paidGallons * price;

            // Update row total cell
            totalCell.textContent = rowTotal > 0 ? `${rowTotal.toFixed(2)}` : "";

            // Build summary
            if (paidGallons + freeGal > 0) {
                if (!summaryMap[itemName]) {
                    summaryMap[itemName] = { qty: 0, itemTotal: 0 };
                }

                // TOTAL quantity (paid + free)
                summaryMap[itemName].qty += client + wrs + freeGal;

                // Paid only added to money total
                summaryMap[itemName].itemTotal += rowTotal;

                totalQty += client + wrs + freeGal;
                totalPrice += rowTotal;
            }
        });

        // Clear summary
        summaryTableBody.innerHTML = "";

        // Render summary rows
        Object.keys(summaryMap).forEach(itemName => {
            const data = summaryMap[itemName];
            const row = document.createElement("tr");
            row.innerHTML = `
                <td class="font-extra-small text-center">${itemName}</td>
                <td class="font-extra-small text-center">${data.qty}</td>
                <td class="font-extra-small text-center">${data.itemTotal.toFixed(2)}</td>
            `;
            summaryTableBody.appendChild(row);
        });

        // Total row
        const totalRow = document.createElement('tr');
        totalRow.innerHTML = `
            <th class="font-extra-small text-center">TOTAL</th>
            <td class="font-extra-small text-center" id="total-qty">${totalQty}</td>
            <td class="font-extra-small text-center" id="total-price">${totalPrice.toFixed(2)}</td>
        `;
        summaryTableBody.appendChild(totalRow);

        subtotalDisplay.textContent = `${totalPrice.toFixed(2)}`;
    }


    // Initial price setup
    updatePricesByMode();
    updateSummary();

    allInputs.forEach(input => {
        input.addEventListener('input', updateSummary);
    });

    deliveryModeSelect.addEventListener('change', function () {
        updatePricesByMode();
        updateSummary();
    });
});