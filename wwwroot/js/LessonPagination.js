
async function generatepage(LessonNumber)
 {
    Lesson.empty();
    let result = await GetLesson(LessonNumber);
    
    Lesson.append(`<h1>${result.lesson_name}</h1>`+result.lesson_content);
}

const Lesson = $("#Lesson");
let previousbtn = $("#1");
previousbtn.toggleClass("active", true);

const maxPages = $("#Previous").attr("data-max")-0;


const nums = $(".num");


const courseuid = $("#container").attr("data-uid");


if (maxPages === 1){
    $("#Next").toggleClass("disabled", true);
}



for (const el of nums) 
{
    el.addEventListener("click", async (e) => {
    const page = e.target.id-0;
        if (page === previousbtn.attr('id')-0) return;
    e = $(e.target);
    
    if (page === 1){
        $("#Previous").toggleClass("disabled", true);
    }
    else {
        $("#Previous").toggleClass("disabled", false);
    }
    if (page === maxPages){
        $("#Next").toggleClass("disabled", true);
    }
    else {
        $("#Next").toggleClass("disabled", false);
    }
    
    previousbtn.parent().toggleClass("active", false);

    previousbtn = e;
    e.parent().addClass("active", true);
    await generatepage(page);
    })};




document.getElementById("Previous").addEventListener("click",async (e) => {
   
    if (e.target.classList.contains("disabled")) return;
    
    const page = previousbtn.attr('id') -1;

    e = $(e.target);
    $("#Next").toggleClass("disabled", false);
    if (page === 1) 
    {
        $("#Previous").toggleClass("disabled", true);
    }
    
    previousbtn.parent().toggleClass("active", false);
    previousbtn = $("#"+page);
    previousbtn.parent().toggleClass("active", true);
    await generatepage(page);
});


document.getElementById("Next").addEventListener("click",async (e) => {
    if (e.target.classList.contains("disabled")) return;
    const page = previousbtn.attr('id') -0 +1;
    
    e = $(e.target);
    $("#Previous").toggleClass("disabled", false);
    if (page === maxPages) 
    {
        $("#Next").toggleClass("disabled", true);
    }

    previousbtn.parent().toggleClass("active", false);
    previousbtn = $("#"+page);
    previousbtn.parent().toggleClass("active", true);
    await generatepage(page);
});

function GetLesson(LessonNumber)
{
    return $.ajax({
        url: '/api/Lesson/GetLessonPageData',
        type: 'GET',
        data: {
            courseuid: courseuid,
            LessonNumber: LessonNumber
            
        },
        success: (data) => {
            return data;
        },
        error: function(xhr, status, error) {
            console.error(error);
            return null;
        }
    });
}

