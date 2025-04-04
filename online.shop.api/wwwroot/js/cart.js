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

    $(".product-card").on("click", function (e) {
        // If the click was on the "Add to Cart" button, do nothing
        if ($(e.target).closest('.add-to-cart-btn').length > 0) {
            return;
        }

        // If the click was on the "Add to Wishlist" button, do nothing
        if ($(e.target).closest('.add-to-wishlist-btn').length > 0) {
            return;
        }

        // If the click was on the "Remove from Wishlist" button, do nothing
        if ($(e.target).closest('.remove-from-wishlist-btn').length > 0) {
            return;
        }

        // Otherwise, navigate to the product details page
        let productId = $(this).data("product-id");
        window.location.href = `/Products/Details/${productId}`;
    });

    $(".inner-quantity-btn").on("click", function () {
        let productId = $(this).data("id");
        let quantityElement = $("#inner-quantity-" + productId);
        let currentQuantity = parseInt(quantityElement.text());
        let newQuantity = currentQuantity;

        if ($(this).hasClass("increase")) {
            newQuantity++;
        } else if ($(this).hasClass("decrease") && currentQuantity > 1) {
            newQuantity--;
        }

        quantityElement.text(newQuantity);
    });

    $(".add-to-wishlist-btn").on("click", function () {
        var productId = $(this).data("product-id");
        var button = $(this);

        // Check if the product is in the wishlist
        var isInWishlist = button.text().includes("Remove");
        var url = isInWishlist ? `/Wishlist/RemoveFromWishlist/` : `/Wishlist/AddToWishlist/`;

        $.ajax({
            url: url,
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    // Toggle button text and style
                    if (isInWishlist) {
                        button.html('❤️ Add to Wishlist');
                        showPopup("✅ Product removed from wishlist!");
                    } else {
                        button.html('❌ Remove from Wishlist');
                        showPopup("✅ Product added to wishlist!");
                    }
                } else {
                    alert('Error updating wishlist.');
                }
            },
            error: function () {
                alert('Something went wrong!');
            }
        });
    });

    $(".remove-from-wishlist-btn").on("click", function () {
        var productId = $(this).data("product-id");
        var button = $(this);

        // Check if the product is in the wishlist
        var url = `/Wishlist/RemoveFromWishlist/`;

        $.ajax({
            url: url,
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    // Toggle button text and style
                    location.reload(); // Refresh the page to reflect the changes
                } else {
                    alert('Error updating wishlist.');
                }
            },
            error: function () {
                alert('Something went wrong!');
            }
        });
    });
});

// Function to add product to cart
function addToCart(button) {
    const encrypt = new JSEncrypt();
    encrypt.setPublicKey(PUBLIC_RSA_KEY);

    let productId = button.data("product-id");
    let quantity = $("#inner-quantity-" + productId).text(); // Get selected quantity

    const payload = {
        productId: productId,
        quantity: parseInt(quantity)
    };

    const encrypted = encrypt.encrypt(JSON.stringify(payload));

    $.ajax({
        url: "/Cart/AddToCart",
        type: "POST",
        contentType: "application/json",
        headers: { "X-Requested-With": "XMLHttpRequest" },
        data: JSON.stringify({ EncryptedData: encrypted }),
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
