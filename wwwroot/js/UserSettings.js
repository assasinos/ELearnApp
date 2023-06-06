function UpdateInfo() {
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

onsubmit = function (e) {
    e.preventDefault();
  switch (e.submitter.id)
  {
      case "PersonalInfo":
          UpdateInfo();
          break;
  }
};