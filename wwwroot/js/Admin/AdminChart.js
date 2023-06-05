
async function GetChartData() {


    const data = await $.ajax(
        {
            type:"GET",
            url:"/api/admin/Orders/GetChartData",
            success:function(datas)
            {
                return datas;
            },
            error:function(e){
                console.log(e);
            }
        });
    let months = [];
    let total_amount = [];
    for (order of data)
    {
        months.push(order.month);
        total_amount.push(order.total_amount);
    }
    return {months,total_amount};
}


async function CreateChart() {
    const data = await GetChartData();


// Get the canvas element
    let ctx = document.getElementById('revenueChart').getContext('2d');

// Create the chart
    let revenueChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.months,
            datasets: [{
                label: 'Revenue',
                //Pass Here real data from database 
                data: data.total_amount,
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
    
    
    
    
}

$("document").ready( async () =>{
    await CreateChart();
})