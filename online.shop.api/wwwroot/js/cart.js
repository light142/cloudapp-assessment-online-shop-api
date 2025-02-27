$(document).ready(function () {
    console.log("Cart.js is running!");

    // Attach click event to all "Add to Cart" buttons
    $(".add-to-cart-btn").on("click", function () {
        addToCart($(this));
    });

    // Handle quantity increase & decrease
    $(".quantity-btn").on("click", function () {
        let productId = $(this).data("id");
        let quantityElement = $("#quantity-" + productId);
        let currentQuantity = parseInt(quantityElement.text());
        let newQuantity = currentQuantity;

        if ($(this).hasClass("increase")) {
            newQuantity++;
        } else if ($(this).hasClass("decrease") && currentQuantity > 1) {
            newQuantity--;
        }

        updateCart(productId, newQuantity);
    });
});

// Function to add product to cart
function addToCart(button) {
    let productId = button.data("product-id");

    $.ajax({
        url: `/Cart/AddToCart/${productId}`,
        type: "POST",
        contentType: "application/json",
        headers: { "X-Requested-With": "XMLHttpRequest" },
        success: function (data) {
            if (data.success) {
                showPopup("✅ Product added to cart!");
                updateCartCounter(); // Update cart counter
            } else {
                showPopup("❌ Failed to add product.");
            }
        },
        error: function () {
            showPopup("⚠️ Something went wrong!");
        }
    });
}

// Function to update cart item quantity
function updateCart(productId, newQuantity) {
    $.ajax({
        url: "/Cart/UpdateCart",
        type: "POST",
        data: { productId: productId, quantity: newQuantity },
        success: function (response) {
            if (response.success) {
                // ✅ Update quantity on the page
                $("#quantity-" + productId).text(newQuantity);

                // ✅ Update item subtotal price dynamically
                $("#subtotal-" + productId).text("$" + response.itemSubtotal.toFixed(2));

                // ✅ Update total cart price dynamically
                $("#cart-total").text("$" + response.cartTotal.toFixed(2));

                // ✅ Update cart counter
                $("#cart-count").text(response.cartCount);
            } else {
                alert("Error updating cart!");
            }
        },
        error: function () {
            alert("Something went wrong!");
        }
    });
}

// Function to show pop-up message
function showPopup(message) {
    let popup = $("<div>").addClass("cart-popup").text(message);
    $("body").append(popup);

    setTimeout(() => {
        popup.addClass("fade-out");
        setTimeout(() => popup.remove(), 500);
    }, 1500);
}

// Function to update cart counter
function updateCartCounter() {
    $.ajax({
        url: '/Cart/GetCartCount',
        type: "GET",
        success: function (data) {
            $("#cart-count").text(data.count);
        },
        error: function () {
            console.error("Error updating cart count");
        }
    });
}
