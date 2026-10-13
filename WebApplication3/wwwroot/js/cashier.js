// Cashier page logic.
// Kept in an external file (instead of an inline <script>) so the
// Content-Security-Policy can block all inline scripts ('unsafe-inline' removed).

(function () {
    "use strict";

    function changeQuantity(button, change) {
        const row = button.closest(".sale-row");
        const input = row.querySelector(".quantity-input");
        const available = parseInt(
            row.querySelector(".available").textContent
        ) || 0;

        let quantity = parseInt(input.value) || 0;
        quantity += change;

        if (quantity < 0) {
            quantity = 0;
        }

        if (quantity > available) {
            quantity = available;
        }

        input.value = quantity;
        calculateTotals();
    }

    function calculateTotals() {
        let grandTotal = 0;

        document.querySelectorAll(".sale-row").forEach(row => {
            const price = parseFloat(
                row.querySelector(".item-price").value
            ) || 0;

            const quantity = parseInt(
                row.querySelector(".quantity-input").value
            ) || 0;

            const total = price * quantity;
            grandTotal += total;

            row.querySelector(".line-total").textContent =
                total.toFixed(2) + " JOD";
        });

        const grandTotalElement = document.getElementById("grandTotal");
        if (grandTotalElement) {
            grandTotalElement.textContent = grandTotal.toFixed(2);
        }
    }

    document.addEventListener("DOMContentLoaded", () => {
        // One listener for all +/- buttons (replaces inline onclick handlers).
        document.addEventListener("click", event => {
            const button = event.target.closest(".quantity-button");
            if (!button) {
                return;
            }

            const change = parseInt(button.dataset.change) || 0;
            changeQuantity(button, change);
        });

        calculateTotals();
    });
})();
