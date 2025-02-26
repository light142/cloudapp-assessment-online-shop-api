$(document).ready(function () {
    // Open Create User Modal
    $("#btnCreateUser").click(function () {
        $.get("/Admin/CreateUser", function (data) {
            $("#createUserContent").html(data);
            $("#createUserModal").modal("show");
        });
    });

    // Open Edit User Modal
    $(".edit-user").click(function () {
        var userId = $(this).data("id");

        $.get("/Admin/EditUser/" + userId, function (data) {
            $("#editUserContent").html(data);
            $("#editUserModal").modal("show");
        });
    });

    // Open Delete User Modal
    $(".delete-user").click(function () {
        var id = $(this).data("id");
        $.get("/Admin/DeleteUser/" + id, function (data) {
            $("#deleteUserContent").html(data);
            $("#deleteUserModal").modal("show");
        });
    });

    // Submit Create User Form
    $(document).on("submit", "#createUserForm", function (e) {
        e.preventDefault();
        var formData = $(this).serialize();

        $.post("/Admin/CreateUser", formData, function (response) {
            if (response.success) {
                alert(response.message);
                $("#createUserModal").modal("hide");
                location.reload(); // Refresh user list
            } else {
                let errors = response.errors.join("\n");
                alert("Error:\n" + errors);
            }
        });
    });

    $(document).on("submit", "#deleteUserForm", function (e) {
        e.preventDefault();
        $.post("/Admin/ConfirmDeleteUser", $(this).serialize(), function (response) {
            if (response.success) {
                alert(response.message);
                $("#deleteUserModal").modal("hide");
                location.reload(); // Refresh user list
            } else {
                let errors = response.errors.join("\n");
                alert("Error:\n" + errors);
            }
        });
    });

    // Submit Edit User Form
    $(document).on("submit", "#editUserForm", function (e) {
        e.preventDefault();
        var formData = $(this).serialize();

        $.post("/Admin/EditUser", formData, function (response) {
            if (response.success) {
                alert(response.message);
                $("#editUserModal").modal("hide");
                location.reload(); // Refresh user list
            } else {
                let errors = response.errors.join("\n");
                alert("Error:\n" + errors);
            }
        });
    });
});
