let page = 1,
  pagesize = 5,
  searchString = "",
  sortColumn = "",
  isAsc = true,
  status = "",
  fromDate = "",
  toDate = "";
  
function getOrders() {
  $.ajax({
    type: "GET",
    url: "/Orders/GetPagedOrders",
    data: {
      page,
      pagesize,
      searchString,
      sortColumn,
      isAsc,
      status,
      fromDate,
      toDate,
    },
    success: function (response) {
      $("#order-partial").html(response);
    },
  });
}

$(document).ready(function () {
  getOrders();

  $("#fromDate").change(function () {
    var date = $(this).val();
    $("#toDate").attr("min", date);
  });

  $("#toDate").change(function () {
    var date = $(this).val();
    $("#fromDate").attr("max", date);
  });
  $("#search-orders").keyup(
    debounce(function () {
      searchString = $(this).val();
      page = 1;
      getOrders();
    }, 300)
  );

  $("#order-status").change(function () {
    status = $(this).val();
    getOrders();
  });

  $("#order-time").change(function () {
    var pastDay = $(this).val();
    var date = new Date();
    if (pastDay === "all") {
      fromDate = "";
      toDate = "";
      getOrders();
      return;
    } else if (pastDay === "month") {
      fromDate = new Date(date.getFullYear(), date.getMonth(), 2);
      toDate = new Date(date.getFullYear(), date.getMonth() + 1, 1);
    } else {
      fromDate = new Date(date); // Clone current date
      fromDate.setDate(fromDate.getDate() - parseInt(pastDay) + 1); // Subtract the selected number of days
      toDate = new Date(date);
    }
    fromDate = fromDate.toISOString().split("T")[0];
    toDate = toDate.toISOString().split("T")[0];

    getOrders();
  });

  $("#date-search").submit(function (e) {
    e.preventDefault();
    fromDate = $("#fromDate").val();
    toDate = $("#toDate").val();
    if (fromDate=='' || toDate =='' || fromDate > toDate) {
      toastr.error("Please Select Valid Dates!");
      return;
    }

    page = 1;
    getOrders();
  });

  // Sorting

  $(document).on("click", ".sort", function () {
    var newSortColumn = $(this).data("sortcolumn");
    if (newSortColumn == sortColumn) {
      isAsc = !isAsc;
    } else {
      isAsc = true;
    }
    sortColumn = newSortColumn;
    getOrders();
  });

  // Pagination

  $(document).on("click", "#orders-pagination #next-page-btn", function () {
    page++;
    getOrders();
  });

  $(document).on("click", "#orders-pagination #prev-page-btn", function () {
    page--;
    getOrders();
  });

  $(document).on("change", "#orders-pagination #pagesizelist", function () {
    page = 1;
    pagesize = $(this).val();
    getOrders();
  });

  $("#clear-btn").click(function () {
    searchString = "";
    status = "";
    fromDate = "";
    toDate = "";
    $("#search-orders").val("");
    $("#order-status").val("");
    $("#order-time").val("all");
    $("#fromDate").removeAttr("max");
    $("#toDate").removeAttr("min");
    getOrders();
  });
});
