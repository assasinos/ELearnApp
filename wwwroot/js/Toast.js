
let toastdiv = $("#liveToast")


function prepareToast(img,title, text)
{
    let el = document.createElement("div")
    el.innerHTML = RetriveToast(img,title, text);

    el = el.firstChild;

    toastdiv.append(el);
    $('.toast').toast('show');
    el.addEventListener('hidden.bs.toast', function (e) {
        if (e.target !== null){
            e.target.remove();
        }
        
    });
    el.addEventListener("click",(e)=>{
        $(e.target).parent().parent().remove();
    });
}
function RetriveToast(img,title, text)
{
    return "<div class=\"toast\" role=\"alert\" aria-live=\"assertive\" aria-atomic=\"true\">\n" +
        "  <div class=\"toast-header\">\n" +
        `    <img src=\"/Assets/Toasts/${img}\" class=\"rounded me-2\" alt=\"Notification img\">\n ` +
        `    <strong class=\"me-auto\">${title}</strong>\n ` +
        "    <small class=\"text-muted\">Just now</small>\n" +
        "  </div>\n" +
        "  <div class=\"toast-body\">\n" +
        `${text}\n ` +
        "  </div>\n" +
        "</div>"
}