
const  username = $("#username-input")

const suggestionsdiv = $('#Suggestions');


username.on('input', async function() {
    const input = $(this).val();

    if (input.length < 3) {return;}
    

    const suggestions = await GetUsers(input);
    const suggestionsHtml = suggestions.map(suggestion => `<p class="btn btn-outline-info m-1 suggestion" >${suggestion}</p>`).join('');
    suggestionsdiv.html(suggestionsHtml);
});

username.on('focus', function() {
    suggestionsdiv.toggleClass("d-none",false);
});
username.on('blur', function() {
    setTimeout(function() {
        suggestionsdiv.toggleClass("d-none",true);
    }, 200);
    
    
});


suggestionsdiv.on('click', '.suggestion', async function() {
    const suggestion = $(this).text();
    username.val(suggestion);
    
    const user_uid = await GetUserUID(suggestion);
    
    $("#user_uid").val(user_uid);
});

onsubmit = function(e) {
    e.preventDefault();
    const user_uid = $("#user_uid").val();
    return $.ajax({
        url: "/api/admin/InstructorEdit/AddInstructor",
        type: "PUT",
        data: {user_uid: user_uid},
        success: function(e) {
            setTimeout(function() {prepareToast("CheckCircle.svg", "Success", "Instructor added successfully");}, 1000);
            window.location.href = `/Admin/Instructor/${user_uid}`;
        },
        error: function(data) {
            prepareToast("Error.svg", "Error", data.responseJSON.errorMessage);
        }
    });
};


function GetUsers(username) {
    return $.ajax({
        url: "/api/admin/InstructorEdit/GetUserSuggestion",
        type: "GET",
        data: {username: username}
    });
}

function GetUserUID(username){
    //Should always be user if called as it is called from the suggestion list
    return $.ajax({
        url: "/api/admin/User/GetUserUID",
        type: "GET",
        data: {username: username}
    });
}