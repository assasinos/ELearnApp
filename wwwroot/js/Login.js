
let toastdiv = $("#liveToast")


onsubmit = function (event) {
    event.preventDefault()
        
    switch (event.submitter.id)
    {
        case "Loginbtn":
            login();
            break;
        case "Registerbtn":
            registeruser();
            break;
    }
}
function registeruser() {

    $.ajax(
        {
            type:"POST",
            url:"/api/Account/Register",
            data: $("#RegisterForm").serialize(),
            success:function(data)
            {
                prepareToast("CheckCircle.svg", "Account Created Successfully", "")
                
                $("#LoginModal").modal('hide');
            },
            error:function(e){
            prepareToast("xmark.svg", "Something went wrong", e.responseText)
            }
        });
    
    
}

function prepareToast(img,title, text)
{
    let el = document.createElement("div")
    el.innerHTML = RetriveToast(img,title, text);

    el = el.firstChild;

    toastdiv.append(el);
    $('.toast').toast('show');
    el.addEventListener('hidden.bs.toast', function (e) {
        e.target.remove();
    })
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



function login()
{

    $.ajax(
        {
            type:"POST",
            url:"/api/Account/Login",
            data: $("#LoginForm").serialize(),
            success:function(data)
            {
                location.reload();
            },
            error:function(e){
                prepareToast("xmark.svg", "Provided credentials are wrong", "Username or Password is incorrect")
            }
        });
}

$("#ShowRegister").click(()=>{
    $("#LoginForm").toggleClass("d-none",true);
    $("#RegisterForm").toggleClass("d-none",false);
    $("#ModalLabel").text("Register")
});
$("#ShowLogin").click(()=>{
    $("#LoginForm").toggleClass("d-none",false);
    $("#RegisterForm").toggleClass("d-none",true);
    $("#ModalLabel").text("Login")
});

$("#loginbtn").click(login);