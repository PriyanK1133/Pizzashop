var page = 1,
  pagesize = 5,
  searchString = "",
  sortColumn = "",
  isAsc = true,
  fromDate = "",
  toDate = "";
function getCustomers() {
  $.ajax({
    type: "GET",
    url: "/Customers/GetPagedCustomers",
    data: { page, pagesize, searchString, sortColumn, isAsc, fromDate, toDate },
    success: function (response) {
      $("#customer-partial").html(response);
    },
  });
}

function getCustomerHistory(id) {
  $.ajax({
    type: "GET",
    url: "/Customers/GetById",
    data: { id },
    success: function (response) {
      if (response.success == false) {
        toastr.error(response.message);
      }
      var modal = $("#customer-history");
      modal.find(".modal-body").html(response);
      modal.modal("show");
    },
  });
}

function exportCustomers() {
  var url = `/Customers/ExportCustomers?searchString=${encodeURIComponent(
    searchString
  )}&fromDate=${encodeURIComponent(fromDate)}&toDate=${encodeURIComponent(
    toDate
  )}`;

  window.open(url, "_blank");
}

$(document).ready(function () {
  getCustomers();
  $("#search-customers").keyup(
    debounce(function () {
      searchString = $(this).val();
      page = 1;
      getCustomers();
    }, 300)
  );

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
    page = 1;
    getCustomers();
    $("#custom-date-modal").modal("hide");
  });

  $("#custom-date-cancel").click(function () {
    $("#fromDate").val("").removeAttr("max");
    $("#toDate").val("").removeAttr("min");
    page = 1;
    pagesize = 5;
    searchString = "";
    sortColumn = "";
    isAsc = true;
    fromDate = "";
    toDate = "";
    getCustomers();
  });

  $("#customer-time").change(function () {
    var pastDay = $(this).val();
    var date = new Date();

    if (pastDay == "custom-date") {
      $("#custom-date-modal").modal("show");
      $(this).val("");
      return;
    }

    if (pastDay === "all") {
      fromDate = "";
      toDate = "";
      getCustomers();
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

    page = 1;
    getCustomers();
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
    console.log(sortColumn + " " + isAsc);
    getCustomers();
  });

  // Pagination

  $(document).on("click", "#customers-pagination #next-page-btn", function () {
    page++;
    getCustomers();
  });

  $(document).on("click", "#customers-pagination #prev-page-btn", function () {
    page--;
    getCustomers();
  });

  $(document).on("change", "#customers-pagination #pagesizelist", function () {
    page = 1;
    pagesize = $(this).val();
    getCustomers();
  });
});
