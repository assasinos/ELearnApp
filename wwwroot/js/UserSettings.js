﻿function UpdateInfo() {
    const form = $("#PersonalForm").serializeArray();
    console.log(form);
    $.ajax({
        url: "/api/Account/UpdatePersonalInfo",
        type: "PUT",
        data: form,
        success: function (data) {
            console.log(data);
            prepareToast("CheckCircle.svg", "Information Updated", "Your personal information has been updated successfully.");
        },
        error: function (data) {
            console.log(data);
            prepareToast("xmark.svg", "Error", data.responseText);
        }
    });
    
}

function UpdatePassword() {
    const newPassword = $("#newpassword");
    if (newPassword.val() === "" || newPassword.val() === null){
        prepareToast("xmark.svg", "Error", "Password cannot be empty");
        return;
    }
    
    const repeat = $("#newpasswordrepeat");


    if (newPassword.val() !== repeat.val()){
        prepareToast("xmark.svg", "Error", "Passwords do not match.");
        return;
    }
    const CurrentPassword = $("#oldpassword");

    if (CurrentPassword.val() === "" || CurrentPassword.val() === null){
        prepareToast("xmark.svg", "Error", "Password cannot be empty");
        return;
    }
    
    $.ajax({
        url: "/api/Account/UpdatePassword",
        type: "PATCH",
        data: {OldPassword: CurrentPassword.val(), NewPassword:newPassword.val()  },
        success: function (data) {

            prepareToast("CheckCircle.svg", "Password Updated", "Your password has been updated successfully.");
            newPassword.val("")
            repeat.val("");
            newPassword.val("");
             setTimeout(()=>{$.ajax({type:"POST",url:"/api/Account/Logout",success:function(e){location.reload();}});}, 1000);
            
            
        },
        error: function (data) {
            prepareToast("xmark.svg", "Error", data.responseText);
        }
    });
    
    
}

const modaldiv =$('#ConfirmModal');
const modalbody = $(".modal-body");


function DeleteAccount() {
    const newPassword = $("#DeletePassword");
    if (newPassword.val() === "" || newPassword.val() === null){
        prepareToast("xmark.svg", "Error", "Password cannot be empty");
        return;
    }

    const repeat = $("#DeletePasswordRepeat");


    if (newPassword.val() !== repeat.val()){
        prepareToast("xmark.svg", "Error", "Passwords do not match.");
        return;
    }


    modalbody.html("<p>Are you sure you want to delete this account?</p>" +
        "<p>Deleting a account will delete all the data related to this account</p>");
    modaldiv.modal('show');
    $("#ConfirmButton").one('click',()=>{

        $.ajax({
            url: "/api/Account/DeleteAccount",
            type: "DELETE",
            data: { confirmpassword:newPassword.val()  },
            success: function (data) {

                prepareToast("CheckCircle.svg", "Account Deleted", "Your Account has been deleted successfully.");
                newPassword.val("")
                repeat.val("");
                newPassword.val("");
                setTimeout(()=>{$.ajax({type:"POST",url:"/api/Account/Logout",success:function(e){location.reload();}});}, 1000);


            },
            error: function (data) {
                prepareToast("xmark.svg", "Error", data.responseText);
            }
            });
        modaldiv.modal('hide');
        return;
    });
    
    
    
}

onsubmit = function (e) {
    e.preventDefault();
  switch (e.submitter.id)
  {
      case "PersonalInfoBTN":
          UpdateInfo();
          break;
      case "PasswordBTN":
          UpdatePassword();
          break;
      case "DeleteAccountBTN":
          DeleteAccount();
          break;
  }
};