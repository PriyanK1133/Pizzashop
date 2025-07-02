var page = 1;
var pagesize = 5;
var searchString = "";

function getTaxesAndFees() {
  $.ajax({
    type: "GET",
    url: "/TaxesAndFees/GetPagedList",
    data: { page: page, pagesize: pagesize, searchString: searchString },
    success: function (response) {
      $("#taxes-and-fees-partial").html(response);
    },
  });
}

function resetForm() {
  $("#add-tax-form")
    .find("input[name='Name'], input[name='TaxAmount'], select")
    .val(null)
    .removeClass("input-validation-error");
  $("#add-tax-form")
    .find(".text-danger")
    .text("")
    .siblings("input")
    .removeClass("input-validation-error");
  $("#add-tax-form").find("input[type='checkbox']").prop("checked", false);
}

$(document).ready(function () {
  $.ajax({
    type: "GET",
    url: "/TaxesAndFees/GetPagedList",
    success: function (response) {
      $("#taxes-and-fees-partial").html(response);
    },
  });

  $("#add-tax-form").submit(function (e) {
    e.preventDefault();

    if (
      $("select#tax-type").val() == "Percentage" &&
      $("input#TaxAmount").val() > 100
    ) {
      $("span[data-valmsg-for='TaxAmount']").text(
        "Tax Amount must be less than 100"
      );
      return;
    } else if (
      $("select#tax-type").val() == "Flat Amount" &&
      $("input#TaxAmount").val() > 100000
    ) {
      $("span[data-valmsg-for='TaxAmount']").text(
        "Tax Value must be between 0 to 100000"
      );
      return;
    }

    if(!$(this).valid()){
      return;
    }

    var form = $(this);
    var data = form.serialize();
    console.log(data);
    $.ajax({
      type: "POST",
      url: "/TaxesAndFees/Add",
      data: data,
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          $("#add-tax-modal").modal("hide");
          $("#taxes-and-fees-partial").load("/TaxesAndFees/GetPagedList");
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  $(document).on("keyup blur change", "input#TaxAmount", function (e) {
    if (
      $("select#tax-type").val() == "Percentage" &&
      $("input#TaxAmount").val() > 100
    ) {
      $("span[data-valmsg-for='TaxAmount']").text(
        "Tax Amount must be less than 100"
      );
    } else if (
      $("select#tax-type").val() == "Flat Amount" &&
      $("input#TaxAmount").val() > 100000
    ) {
      $("span[data-valmsg-for='TaxAmount']").text(
        "Tax Value must be between 0 to 100000"
      );
    }
  });

  $(document).on("click", ".edit-tax-btn", function () {
    var id = $(this).data("id");

    $.ajax({
      type: "GET",
      url: "/TaxesAndFees/GetById",
      data: { id: id },
      success: function (response) {
        if (response.success == false) {
          toastr.error(response.message);
          return;
        }
        $("#edit-tax-modal").html(response);
        $("#edit-tax-modal #add-edit-tax-title").text("Update Tax");
        $("#edit-tax-modal").modal("show");
      },
    });
  });

  $("#edit-tax-form").submit(function (e) {
    e.preventDefault();

    if (
      $(this).find("select#tax-type").val() == "Percentage" &&
      $(this).find("input#TaxAmount").val() > 100
    ) {
      $(this).find("span[data-valmsg-for='TaxAmount']").text(
        "Tax Amount must be less than 100"
      );
      return;
    } else if (
      $(this).find("select#tax-type").val() == "Flat Amount" &&
      $(this).find("input#TaxAmount").val() > 100000
    ) {
      $(this).find("span[data-valmsg-for='TaxAmount']").text(
        "Tax Value must be between 0 to 100000"
      );
      return;
    }

    if(!$(this).valid()){
      return;
    }

    var data = $(this).serialize();

    $.ajax({
      type: "POST",
      url: "/TaxesAndFees/Edit",
      data: data,
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          $("#edit-tax-modal").modal("hide");
          $("#taxes-and-fees-partial").load("/TaxesAndFees/GetPagedList");
        } else if (response.success == null) {
          $("#edit-tax-modal").html(response);
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  $(document).on("click", ".delete-tax-btn", function () {
    var id = $(this).data("id");
    $("#delete-tax-modal #delete-btn").data("id", id);
    $("#delete-tax-modal").find(".modal").modal("show");
  });

  $(document).on("click", "#delete-tax-modal #delete-btn", function () {
    var id = $(this).data("id");
    console.log(id);
    $.ajax({
      type: "GET",
      url: "/TaxesAndFees/Delete",
      data: { id: id },
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          $("#taxes-and-fees-partial").load("/TaxesAndFees/GetPagedList");
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  // Pagination

  $(document).on("click", "#tax-pagination #next-page-btn", function () {
    page++;
    getTaxesAndFees();
  });

  $(document).on("click", "#tax-pagination #prev-page-btn", function () {
    page--;
    getTaxesAndFees();
  });

  $(document).on("change", "#tax-pagination #pagesizelist", function () {
    pagesize = $(this).val();
    page = 1;
    console.log(pagesize);
    getTaxesAndFees();
  });

  $("#search-tax").keyup(
    debounce(function () {
      searchString = $(this).val();
      getTaxesAndFees();
    }, 300)
  );
});
