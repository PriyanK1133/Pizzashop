var page = 1,
  pagesize = 5,
  searchString = "",
  sortColumn = "",
  isAsc = true;
function getUsers() {
  $.ajax({
    type: "GET",
    url: "/Users/GetPagedUsers",
    data: { page, pagesize, searchString, sortColumn, isAsc },
    success: function (response) {
      $("#users-partial").html(response);
    },
  });
}

$(document).ready(function () {
  console.log("script");
  getUsers();

  $("#search-users").keyup(
    debounce(function () {
      page = 1;
      searchString = $(this).val();
      getUsers();
    }, 300)
  );

  $(document).on("click", ".sort", function () {
    var newSortColumn = $(this).data("sortcolumn");
    if (newSortColumn == sortColumn) {
      isAsc = !isAsc;
    } else {
      isAsc = true;
    }
    sortColumn = newSortColumn;
    getUsers();
  });

  // Pagination

  $(document).on("click", "#users-pagination #next-page-btn", function () {
    page++;
    getUsers();
  });

  $(document).on("click", "#users-pagination #prev-page-btn", function () {
    page--;
    getUsers();
  });

  $(document).on("change", "#users-pagination #pagesizelist", function () {
    page = 1;
    pagesize = $(this).val();
    getUsers();
  });
});
