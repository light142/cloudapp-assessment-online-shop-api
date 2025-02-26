
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
});
