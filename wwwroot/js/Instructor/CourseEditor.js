

const courseuid = $("#course_uid").val();




$("#LessonSelect").change(function () {
    const lessonuid = $(this).val();
    
    $("#Lessonuid").val(lessonuid);
    if (lessonuid === "New") {
        $("#LessonContent").val("");
        $("#LessonTitle").val("");
        $("#DeleteLessonBTN").toggleClass("disabled", true);
        return;
    }
    $("#DeleteLessonBTN").toggleClass("disabled", false);
    


    $.ajax({
        url: "/api/Lesson/GetLessonData",
        type: "GET",
        data: { lessonuid: lessonuid, courseuid: courseuid },
        success: function (data) {
            $("#LessonContent").val(data.lesson_content);
            $("#LessonTitle").val(data.lesson_name);
        }
    });
});


function CreateLesson(lessoncontent, lessontitle, courseuid) {
    $.ajax({
        url: "/api/instructor/CourseEdit/CreateLesson",
        type: "PUT",
        data: {course_uid: courseuid, lesson_content: lessoncontent, lesson_name:  lessontitle},
        success: function (data) {
            prepareToast("CheckCircle.svg", "Lesson Updated", "The Lesson has been created successfully");
            $("#Lessonuid").val(data)
            const lessonselect = $("#LessonSelect");
            lessonselect.prepend("<option value=\"" + data + "\">" + lessontitle + "</option>");
            lessonselect.val(data);
            $("#DeleteLessonBTN").toggleClass("disabled", false);
        },
        error: function (data) {
            prepareToast("xmark.svg", "Error", data.responseJSON.errorMessage );

        }
    });
}

function UpdateLesson() {
    const lessonuid = $("#Lessonuid").val();
    if (lessonuid === ""  ) {
        prepareToast("xmark.svg", "Error", "Please select a lesson to update");
        return;}
    
    const lessoncontent = $("#LessonContent").val();
    const lessontitle = $("#LessonTitle").val();
    if (lessonuid === "New"){
        CreateLesson(lessoncontent, lessontitle,courseuid);
        return;
    }
    $.ajax({
        url: "/api/instructor/CourseEdit/UpdateLesson",
        type: "PUT",
        data: { lesson_uid: lessonuid, course_uid: courseuid, lesson_content: lessoncontent, lesson_name:  lessontitle},
        success: function (data) {
            prepareToast("CheckCircle.svg", "Lesson Updated", "The Lesson has been updated successfully");
        },
        error: function (data) {
        
            
            prepareToast("xmark.svg", "Error", data.responseJSON.errorMessage);
            
        }
    });
}

function UpdateCourse() {
    const form = $("#CourseFrom").serialize();
    $.ajax({
        url: "/api/instructor/CourseEdit/UpdateCourse",
        type: "PUT",
        data: form,
        success: function (data) {
            prepareToast("CheckCircle.svg", "Course Updated", "The Course has been updated successfully");
        },
        error: function (data) {
            console.log(data);
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
        case "UpdateLessonBTN":
            UpdateLesson();
            break;
        case "UpdateCourseBTN":
            UpdateCourse();
            break;
    }
};


$("#DeleteCourseBTN").click((e)=>{

    modalbody.html("<p>Are you sure you want to delete this course?</p>" +
        "<p>Deleting a course will delete all lessons and all the data related to this course</p>");
    modaldiv.modal('show');
    $("#ConfirmButton").one('click',()=>{
        $.ajax({
            url: "/api/instructor/CourseEdit/DeleteCourse",
            type: "DELETE",
            data: {course_uid: courseuid},
            success: function (data) {
                window.location.href = "/Instructor/Courses";
            },
            error: function (data) {
                prepareToast("xmark.svg", "Error", data.responseJSON.errorMessage );

            }
        });
        modaldiv.modal('hide');
        return;
    });





});


$("#DeleteLessonBTN").click((e)=>{
    
    modalbody.html("<p>Are you sure you want to delete this lesson?</p>");    
    modaldiv.modal('show');
    let lessonuid = $("#Lessonuid").val();
    if (lessonuid === "" || lessonuid === "New") {
        prepareToast("xmark.svg", "Error", "Please select a lesson to delete");
        return;}
    $("#ConfirmButton").one('click',()=>{


        lessonuid = $("#Lessonuid").val();
        $.ajax({
            url: "/api/instructor/CourseEdit/DeleteLesson",
            type: "DELETE",
            data: { lesson_uid: lessonuid, course_uid: courseuid},
            success: function (data) {
                prepareToast("CheckCircle.svg", "Lesson Updated", "The Lesson has been Deleted successfully");

                const lessonselect = $("#LessonSelect");
                lessonselect.find(`option[value='${lessonuid}']`).remove();
                lessonselect.val("New");
                lessonselect.trigger('change')
            },
            error: function (data) {
                prepareToast("xmark.svg", "Error", data.responseJSON.errorMessage);

            }
        });
        modaldiv.modal('hide');
        return;
    });
    
    

});