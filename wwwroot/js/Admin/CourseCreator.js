



onsubmit = (e) => {
    e.preventDefault();
    CreateCourse();
};
function CreateCourse(){
    const form = $("#CourseFrom").serialize();
    $.ajax({
        url: "/api/admin/CourseEdit/CreateCourse",
        type: "PUT",
        data: form,
        success: function (data) {
            prepareToast("CheckCircle.svg", "Success", "Course created successfully You will be moved shortly");
            setTimeout(() => {
                window.location.href = "/Admin/Course/" + data;
            }, 2000);
        },
        error: function (data) {
            prepareToast("xmark.svg", "Error", data.responseJSON.errorMessage);
        }
    });
}
