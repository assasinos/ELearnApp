
const courses = $("#courses");
let previousbtn = $("#1");
previousbtn.parent().toggleClass("active", true);

const maxPages = $("#Previous").attr("data-max")-0;

if (maxPages === 1){
    $("#Next").toggleClass("disabled", true);
}


const nums = $(".num");


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
    previousbtn.parent().addClass("active", true);
    await DisplayPage(page);
});

document.getElementById("Next").addEventListener("click",async (e) => {

    if (e.target.classList.contains("disabled")) return;
    const page = previousbtn.attr('id') -0 +1;
    
    e = $(e.target);
    $("#Previous").toggleClass("disabled", false);
    console.log(page, maxPages)
    if (page === maxPages) 
    {
        $("#Next").toggleClass("disabled", true);
    }

    previousbtn.parent().toggleClass("active", false);
    previousbtn = $("#"+page);
    previousbtn.parent().addClass("active", true);
    await DisplayPage(page);
});


async function DisplayPage(page){
    courses.empty();
    let result = await GetPage(page);

    result.forEach((el)=>{
        courses.append("            <div class=\"col-lg-4 mb-3 d-flex align-items-stretch\">\n" +
            "              <div class=\"card\">\n" +
            `                <img src=\"${el.imgsrc}\" class=\"card-img-top img-fluid\" alt=\"Course image\">\n` +
            "                <div class=\"card-body\">\n" +
            `                  <h5 class=\"card-title\">${el.title}</h5>\n` +
            `<a class=\"btn btn-primary\" href=\"/Course/${el.course_uid}\">Learn more</a>` +
            "                </div>\n" +
            "              </div>\n" +
            "            </div>");
    });
}



for (const el of nums) {
    el.addEventListener("click", async (e) => {


        const page = e.target.id-0;

        e = $(e.target);
        
    
        if (page === 1) {
            $("#Previous").toggleClass("disabled", true);
        } else {
            $("#Previous").toggleClass("disabled", false);
        }
        
        if (page === maxPages) 
        {
            $("#Next").toggleClass("disabled", true);
        }
        else{
            $("#Next").toggleClass("disabled", false);
        }
        
        previousbtn.parent().toggleClass("active", false);
        previousbtn = e;
        e.parent().addClass("active", true)
        await DisplayPage(page);

    });
}




function GetPage(pagenumber)
{
    return $.ajax({
        url: '/api/Course/GetUserCourses',
        type: 'GET',
        data: {
            pageNumber: pagenumber
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





















