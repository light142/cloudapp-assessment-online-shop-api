
$(document).ready(function () {
    console.log("admin-products.js is running!");

    $(".edit-product").click(function () {
        var productId = $(this).data("id");
        $.get("/Admin/EditProduct/" + productId, function (data) {
            $("#editProductContent").html(data);
            $("#editProductModal").modal("show");
        });
    });

    $(".delete-product").click(function () {
        var productId = $(this).data("id");
        $.get("/Admin/DeleteProduct/" + productId, function (data) {
            $("#deleteProductContent").html(data);
            $("#deleteProductModal").modal("show");
        });
    });

    $("#btnCreateProduct").click(function () {
        $.get("/Admin/CreateProduct", function (data) {
            $("#createProductContent").html(data);
            $("#createProductModal").modal("show");
        });
    });

    $(document).on("submit", "#createProductForm", function (e) {
        e.preventDefault();
        $.post("/Admin/CreateProduct", $(this).serialize(), function (response) {
            if (response.success) {
                alert(response.message);
                $("#createProductModal").modal("hide");
                location.reload(); // Refresh user list
            } else {
                alert("Error:\n" + response.message);
            }
        });
    });

    $(document).on("submit", "#deleteProductForm", function (e) {
        e.preventDefault();
        $.post("/Admin/ConfirmDelete", $(this).serialize(), function (response) {
            if (response.success) {
                alert(response.message);
                $("#deleteProductModal").modal("hide");
                location.reload(); // Refresh user list
            } else {
                alert("Error:\n" + response.message);
            }
        });
    });

    $(document).on("submit", "#editProductForm", function (e) {
        e.preventDefault();
        $.post("/Admin/EditProduct", $(this).serialize(), function (response) {
            if (response.success) {
                alert(response.message);
                $("#editProductModal").modal("hide");
                location.reload(); // Refresh user list
            } else {
                alert("Error:\n" + response.message);
            }
        });
    });
});
