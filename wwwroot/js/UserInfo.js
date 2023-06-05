
$.ajax(
    {
        type:"GET",
        url:"/api/User/GetUser",
        success:function(data)
        {
            const nickname = $("#nickname");
            nickname.append('<img src="'+data.imgsrc+' "class="img-fluid rounded-circle me-2" alt="Profile Image" width="30" height="30">')
            
            nickname.append(data.username);
            if (data.role != "Student")
            {
                nickname.append('<span class="badge rounded-pill bg-info text-dark ms-1">'+data.role+'</span>')
            }
        },
        error:function(e){
            console.log(e);
        }
    });