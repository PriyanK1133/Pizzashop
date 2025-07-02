function BindSectionList() {
  let ddSections = $(".dd-section");

  $.ajax({
    type: "GET",
    async: false,
    url: "/OrderApp/GetAllSections",
    success: function (response) {
      ddSections
        .empty()
        .append("<option selected disabled>Select Section</option>");

      $.each(response.sections, function (i, val) {
        ddSections.append(
          $("<option></option>").val(this.value).text(this.text)
        );
      });
    },
  });
}

function BindTables(sectionId) {
  let ddTables = $("#dd-tables");
  ddTables.empty().append('<li class="ms-3">Select Tables</li>');

  $.ajax({
    type: "GET",
    async: false,
    url: "/OrderApp/GetAvailableTablesBySectionId",
    data: { sectionId },
    success: function (response) {
      if (response.success) {
        if (response.data.length == 0) {
          ddTables.append(
            '<li class="ms-3">No Available Tables in Section</li>'
          );
        }
        $.each(response.data, function (i, table) {
          ddTables.append(`<li >
                              <div class="form-check ms-3">
                                  <input class="form-check-input table-checkbox " type="checkbox" id='${table.id}-checkbox' value="${table.id}" data-id="${table.id}" data-capacity='${table.capacity}'>
                                  <label class="form-check-label" for="${table.id}-checkbox">
                                      ${table.name} (${table.capacity})
                                  </label>
                              </div>
                          </li>`);
        });
      }
    },
  });
}

function loadWaitingList() {
  let sectionId = $(".section-tab.active").data("section-id");
  var waitingList = $("#waiting-list-partial");
  $.ajax({
    type: "GET",
    url: "/OrderApp/GetWaitingListPartial",
    data: { sectionId },
    success: function (response) {
      waitingList.html(response);
      updateTime();
    },
  });
}

function setAllCount() {
  let totalCount = 0;
  let allCountBadge = $(".all-count");
  $(".list-count").each(function () {
    totalCount += parseInt($(this).text());
  });
  allCountBadge.text(totalCount);
}

function fillCustomerDetils(form, customer) {
  form.find("input[name='Email']").val(customer.email).prop("disabled", true);
  form.find("input[name='Name']").val(customer.name);
  form.find("input[name='Id']").val(customer.id);
  form.find("input[name='CustomerId']").val(customer.customerId);
  form.find("input[name='Phone']").val(customer.phone);
  form.find("input[name='NumberOfPerson']").val(customer.numberOfPerson);
  if (!isEmptyGuid(customer.sectionId)) {
    form.find("select[name='SectionId']").val(customer.sectionId);
  }

  form
    .find("span.text-danger")
    .text("")
    .parent()
    .find("input, textarea, select")
    .removeClass("input-validation-error");
}

function setEditWaitingTokenData(customer) {
  let editWaitingTokenModal = $("#edit-waiting-token-modal");
  let form = editWaitingTokenModal.find("form");

  form.find("input[name='Email']").val(customer.email).prop("disabled", true);
  form.find("input[name='Name']").val(customer.name);
  form.find("input[name='Id']").val(customer.id);
  form.find("input[name='CustomerId']").val(customer.customerId);
  form.find("input[name='Phone']").val(customer.phone);
  form.find("input[name='NumberOfPerson']").val(customer.numberOfPerson);
  form
    .find("select[name='SectionId']")
    .val(customer.sectionId)
    .data("current-section", customer.sectionId);
  form
    .find("span.text-danger")
    .text("")
    .parent()
    .find("input, textarea, select")
    .removeClass("input-validation-error");

  editWaitingTokenModal.modal("show");
}

function increaseCount(sectionId) {
  let countBadge = $(`#${sectionId}-count`);
  let count = parseInt(countBadge.text());
  countBadge.text(count + 1);
  setAllCount();
}

function decreaseCount(sectionId) {
  let countBadge = $(`#${sectionId}-count`);
  let count = parseInt(countBadge.text());
  countBadge.text(count - 1);
  setAllCount();
}

$(document).ready(function () {
  BindSectionList();
  loadWaitingList();
  setAllCount();
  setInterval(updateTime, 60000);
  $("#edit-waiting-token-modal #submit-btn").text("Save");
  $("#delete-waiting-token-modal .modal-title").text("Cancel Confirmation");

  $(".section-tab").click(function () {
    loadWaitingList();
  });

  // Auto Fetch Data from Email

  $(document).on("blur", "input[name='Email']", function () {
    var email = $(this).val();
    var form = $(this).parents("form");
    $.ajax({
      type: "GET",
      url: "/OrderApp/GetCustomerDetailsByEmail",
      data: { email },
      success: function (response) {
        if (response.success === true) {
          fillCustomerDetils(form, response.data);
        }
      },
    });
  });

  // Waiting Token Form Submit

  $(document).on("submit", "#waiting-token-form form", function (e) {
    e.preventDefault();

    var disabled = $(this)
      .find("select:disabled, input:disabled")
      .prop("disabled", false);
    let sectionId = $(this).find("select[name='SectionId']").val();
    var data = $(this).serialize();
    disabled.prop("disabled", true);

    $.ajax({
      type: "POST",
      url: "/OrderApp/AddWaitingToken",
      data: data,
      success: function (response) {
        if (response.success == true) {
          $("#waiting-token-modal").modal("hide");
          $(".reset-customer-detail").click();
          toastr.success("Customer Added in Waiting List!");
          loadWaitingList();
          increaseCount(sectionId);
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  // reset customer details form
  $(".waiting-token-btn").click(function () {
    let waitingTokenModal = $("#waiting-token-modal");
    $.ajax({
      type: "GET",
      url: "/OrderApp/GetWaitingTokenForm",
      success: function (response) {
        waitingTokenModal.find("#waiting-token-form").html(response);
        BindSectionList();
        $("#waiting-token-form #submit-btn").text("Save");
      },
    });
    waitingTokenModal.modal("show");
  });

  // Remove Waiting Token Btn
  $(document).on("click", ".remove-waiting-token-btn", function () {
    let waitingTokenDeleteModal = $("#delete-waiting-token-modal").find(
      ".modal"
    );
    let id = $(this).parent().data("id");
    let sectionId = $(this).parent().data("section-id");
    waitingTokenDeleteModal.find("#delete-btn").data("id", id);
    waitingTokenDeleteModal.find("#delete-btn").data("section-id", sectionId);
    waitingTokenDeleteModal.modal("show");
  });

  // Remove Waiting Token
  $("#delete-waiting-token-modal #delete-btn").click(function () {
    let id = $(this).data("id");
    let sectionId = $(this).data("section-id");

    $.ajax({
      type: "DELETE",
      url: "/OrderApp/DeleteWaitingToken",
      data: { id },
      success: function (response) {
        if (response.success === true) {
          toastr.success(response.message);
          loadWaitingList();
          decreaseCount(sectionId);
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  // Edit Waiting Token
  $(document).on("submit", "#edit-waiting-token-form form", function (e) {
    e.preventDefault();

    let disabled = $(this).find(":disabled").prop("disabled", false);
    let data = $(this).serialize();
    let newSectionId = $(this).find("select[name='SectionId']").val();
    let currentSectionId = $(this)
      .find("select[name='SectionId']")
      .data("current-section");
    disabled.prop("disabled", true);

    $.ajax({
      type: "POST",
      url: "/OrderApp/EditWaitingToken",
      data: data,
      success: function (response) {
        if (response.success === true) {
          toastr.success(response.message);
          loadWaitingList();
          decreaseCount(currentSectionId);
          increaseCount(newSectionId);
          $(".modal").modal("hide");
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  // Assign Table

  $(document).on("click", ".assign-tables-btn", function () {
    let waitingTokenId = $(this).parent().data("id");
    let numberOfPerson = $(this).data("persons");
    let sectionId = $(this).parent().data("section-id");
    $("#assign-tables-form").data("waiting-token-id", waitingTokenId);
    $("#assign-tables-form").data("persons", numberOfPerson);
    $("#assign-tables-form").data("section-id", sectionId);
  });

  $("#assign-section").change(function () {
    let sectionId = $(this).val();
    $("#assign-section-error").text("");
    BindTables(sectionId);
  });

  $(".assign-table-cancel-btn").click(function () {
    $("#dd-tables").html("<li class='ms-3'> Select Table</li>");
  });

  $(document).on("change", "#dd-tables .table-checkbox", function () {
    let errorBox = $("#assign-tables-error");
    let checkedTables = $("#dd-tables .table-checkbox:checked").length;
    if (checkedTables == 0) {
      errorBox.text("Please Select Tables!");
    } else {
      errorBox.text("");
    }
  });

  $("#assign-tables-form").submit(function (e) {
    e.preventDefault();
    var form = $(this);
    let waitingTokenId = form.data("waiting-token-id");

    let sectionId = form.find("#assign-section").val();
    if (sectionId == null) {
      form.find("#assign-section-error").text("Section is Required!");
      return;
    }

    let tableIds = [];
    let totalCapacity = 0;
    let numberOfPerson = parseInt(form.data("persons"));
    form
      .find("#dd-tables")
      .find(".table-checkbox:checked")
      .each(function () {
        let tableId = $(this).data("id");
        let tableCapacity = parseInt($(this).data("capacity"));
        totalCapacity += tableCapacity;
        tableIds.push(tableId);
      });

    if (tableIds.length == 0) {
      form.find("#assign-tables-error").text("Please Select Tables!");
      return;
    }
    if (totalCapacity < numberOfPerson) {
      $(this)
        .find("#assign-tables-error")
        .text("Number of person must be less than table capacity!");
      return;
    }

    $.ajax({
      type: "POST",
      url: "/OrderApp/AssignTablesToWaitingCustomer",
      data: { waitingTokenId, sectionId, tableIds },
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          window.location.href = "/OrderApp/Menu/" + response.data;
        } else {
          toastr.error(response.message);
        }
      },
    });
  });
});
