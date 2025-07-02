let fromDate, toDate;

function loadDashboardData() {
  $.ajax({
    type: "GET",
    url: "/Dashboard/GetDashboardData",
    data: { fromDate, toDate },
    success: function (response) {
      $("#dashboard-partial").html(response);
      loadGraph();
    },
  });
}

function loadGraph() {
  $.ajax({
    type: "GET",
    url: "/Dashboard/GetGraphData",
    data: { fromDate, toDate },
    success: function (response) {
      let revenueData = response.data.revenueData;
      let revenueDates = revenueData.map((r) => r.date);
      let revenueValues = revenueData.map((r) => r.value);
      makeChart("Revenue", revenueDates, revenueValues);

      let customerGrowthData = response.data.customerGrowthData;
      let customerGrowthDates = customerGrowthData.map((cg) => cg.date);
      let customerGrowthValues = customerGrowthData.map((cg) => cg.value);
      makeChart("CustomerGrowth", customerGrowthDates, customerGrowthValues);
    },
  });
}
//Revenue-chart  Customer-chart
function makeChart(canvas, xValues, yValues) {
  new Chart(canvas, {
    type: "line",
    data: {
      labels: xValues,
      datasets: [
        {
          label: canvas,
          borderColor: "#2196F3",
          backgroundColor: "rgba(33, 150, 243, 0.1)",
          data: yValues,
          borderWidth: 2,
          pointRadius: 0,
          pointHoverRadius: 5,
          tension: 0.4,
          fill: true,
        },
      ],
    },
    options: {
      tension: 1,
      responsive: true,
      scales: {
        yAxes: [{ ticks: { min: 0 } }],
      },
      maintainAspectRatio: false,
      plugins: {
        tooltip: {
          mode: "index",
          intersect: false,
          backgroundColor: "rgba(255, 255, 255, 0.9)",
          titleColor: "#333",
          bodyColor: "#666",
          borderColor: "#e0e0e0",
          borderWidth: 1,
        },
      },
    },
  });
}

$(document).ready(function () {
  loadDashboardData();
  loadGraph();

  $("#dashboard-time").change(function () {
    var pastDay = $(this).val();
    var date = new Date();

    if (pastDay == "custom-date") {
      $("#custom-date-modal").modal("show");
      $(this).val("");
      return;
    }

    if (pastDay === "all") {
      fromDate = null;
      toDate = null;
      loadDashboardData();
      return;
    } else if (pastDay === "month") {
      fromDate = new Date(date.getFullYear(), date.getMonth(), 2);
      toDate = new Date(date.getFullYear(), date.getMonth() + 1, 1);
    } else {
      fromDate = new Date(); // Clone current date
      fromDate.setDate(fromDate.getDate() - parseInt(pastDay) + 1); // Subtract the selected number of days
      toDate = new Date();
    }

    fromDate = fromDate.toISOString().split("T")[0];
    toDate = toDate.toISOString().split("T")[0];

    loadDashboardData();
  });

  $("#fromDate").change(function () {
    var date = $(this).val();
    $("#toDate").attr("min", date);
  });

  $("#toDate").change(function () {
    var date = $(this).val();
    $("#fromDate").attr("max", date);
  });

  $("#custom-date-submit").click(function () {
    fromDate = $("#fromDate").val();
    toDate = $("#toDate").val();

    if (fromDate == "" || toDate == "" || fromDate > toDate) {
      toastr.error("Please Select Valid Dates!");
      return;
    }

    loadDashboardData();
    $("#custom-date-modal").modal("hide");
  });

  $("#custom-date-cancel").click(function () {
    $("#fromDate").val("").removeAttr("max");
    $("#toDate").val("").removeAttr("min");
  });
});
