﻿


$("#Logout").click(()=>{$.ajax({type:"POST",url:"/api/Account/Logout",success:function(e){location.reload();}});});