

const user_uid = $("#user_uid").val();

function UpdateUser() {
    const form = $("#UserForm").serialize();
    $.ajax({
        url: "/api/admin/InstructorEdit/UpdateInstructor",
        type: "PUT",
        data: form,
        success: function (data) {
            prepareToast("CheckCircle.svg", "Course Updated", "The Course has been updated successfully");
        },
        error: function (data) {
            prepareToast("xmark.svg", "Error", data.responseJSON.errorMessage);
        }
    });
}


const modaldiv =$('#ConfirmModal');
const modalbody = $(".modal-body");

onsubmit = function (e)
{
    e.preventDefault();
    switch (e.submitter.id) {
        case "UpdateUserBTN":
            UpdateUser();
            break;
    }
};



$("#RemoveRoleBTN").click(()=>{

    modalbody.html("<p>Are you sure you want to remove instructor role from this user?</p>");
    modaldiv.modal('show');
    $("#ConfirmButton").one('click',()=> {
        $.ajax({
            url: "/api/admin/InstructorEdit/RemoveInstructorRole",
            type: "DELETE",
            data: {user_uid: user_uid},
            success: function (data) {
                window.location.href = "/Admin/Instructors";
            },
            error: function (data) {
                prepareToast("xmark.svg", "Error", data.responseJSON.errorMessage);

            }
        });
        modaldiv.modal('hide');
    });
});