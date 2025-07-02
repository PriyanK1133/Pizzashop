function loadTables(id) {
  $(`#${id} .accordion-body`).html("Loading.......");
  $.ajax({
    type: "GET",
    url: "/OrderApp/GetTableListPartial",
    data: { id },
    success: function (response) {
      $(`#${id} .accordion-body`).html(response);
      updateTime();
    },
  });
}

function fillCustomerDetails(form, customer) {
  console.log(form);
  console.log(customer);
  form.find("input[name='Email']").val(customer.email).prop("disabled", true);
  form.find("input[name='Name']").val(customer.name).prop("disabled", true);
  form.find("input[name='Id']").val(customer.id);
  form.find("input[name='CustomerId']").val(customer.customerId);
  form.find("input[name='Phone']").val(customer.phone).prop("disabled", true);
  form.find("input[name='NumberOfPerson']").val(customer.numberOfPerson);
  if (!isEmptyGuid(customer.sectionId)) {
    form
      .find("select[name='SectionId']")
      .val(customer.sectionId)
      .prop("disabled", true);
  }

  form
    .find("span.text-danger")
    .text("")
    .parent()
    .find("input, textarea, select")
    .removeClass("input-validation-error");
}

function loadWaitingList(sectionId) {
  var waitingList = $("#waiting-list");
  $.ajax({
    type: "GET",
    url: "/OrderApp/GetWaitingList",
    data: { sectionId },
    success: function (response) {
      if (response.success === true && response.data.length != 0) {
        waitingList.empty();
        $.each(response.data, function (i, customer) {
          var tr = `<tr>
                                <td>
                                    <div class="form-check" >
                                        <input class="form-check-input" type="radio" name="waiting-list-radio" id="${
                                          customer.id
                                        }-radio" value="${
            customer.id
          }" onclick='fillCustomerDetails($("#customer-details-form form"),${JSON.stringify(
            customer
          )})'>
                                    </div>
                                </td>
                                <td>#${customer.id.slice(-5)}</td>
                                <td>${customer.name}</td>
                                <td>${customer.numberOfPerson}</td>
                            </tr>`;
          waitingList.append(tr);
        });
        return;
      }
      waitingList.html(
        "<tr><td colspan='4' class='text-center'>No Waiting Customer in this Section!</td></tr>"
      );
    },
  });
}

function BindSectionList() {
  var ddSections = $(".dd-section");

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

$(document).ready(function () {
  // update Time
  setInterval(() => {
    updateTime();
  }, 1000);
  BindSectionList();

  $(document).on(
    "click",
    ".section-card",
    debounce(function () {
      if ($(this).hasClass("active-section-card")) {
        $(this).removeClass("active-section-card");
        return;
      }
      $(".section-card").removeClass("active-section-card");
      $(this).addClass("active-section-card");
      var id = $(this).data("id");
      loadTables(id);
      $(".assign-tables-btn").prop("disabled", true);
    }, 100)
  );

  // Selected table
  $(document).on("click", ".available-table", function () {
    if ($(this).hasClass("selected-table")) {
      $(this).removeClass("selected-table");
    } else {
      $(this).addClass("selected-table");
    }
    var selectedTablesLength = $(".selected-table").length;
    var assignBtn = $(this)
      .parents(".accordion-collapse")
      .find(".assign-tables-btn");
    if (selectedTablesLength > 0) {
      assignBtn.prop("disabled", false);
    } else {
      assignBtn.prop("disabled", true);
    }
  });

  $(document).on("click", ".table-box:not(.available-table)", function () {
    window.location.href = "/OrderApp/Menu/" + $(this).data("order-id");
  });

  // reset customer details form
  $(document).on("click", ".reset-customer-detail", function () {
    var form = $(this).parents("form");
    form.find("input").prop("disabled", false).val("");
    form
      .find(".text-danger")
      .text("")
      .parent()
      .find("input, textarea, select")
      .removeClass("input-validation-error");
    $("#waiting-token-modal").modal("hide");
  });

  // Assign tables btn
  $(document).on("click", ".assign-tables-btn", function () {
    var sectionId = $(this).data("section");
    console.log("click", sectionId);
    var form = $("#customer-details-form");
    form.find("select[name='SectionId']").val(sectionId).prop("disabled", true);
    loadWaitingList(sectionId);
  });

  // waiting token btn
  $(document).on("click", ".waiting-token-btn", function () {
    var sectionId = $(this).data("section");
    var form = $("#waiting-token-form");

    $.ajax({
      type: "GET",
      url: "/OrderApp/GetWaitingTokenForm",
      success: function (response) {
        form.html(response);
        BindSectionList();
        form.find("#submit-btn").text("Save");
        form
          .find("select[name='SectionId']")
          .val(sectionId)
          .prop("disabled", true);
      },
    });
  });

  // Waiting Token Form Submit
  $(document).on("submit", "#waiting-token-form form", function (e) {
    e.preventDefault();

    var disabled = $(this)
      .find("select:disabled, input:disabled")
      .prop("disabled", false);
    var data = $(this).serialize();
    disabled.prop("disabled", true);

    console.log(data);
    $.ajax({
      type: "POST",
      url: "/OrderApp/AddWaitingToken",
      data: data,
      success: function (response) {
        if (response.success == true) {
          $("#waiting-token-modal").modal("hide");
          toastr.success("Customer Added in Waiting List!");
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  // Assign Table Form Submit
  $(document).on("submit", "#customer-details-form form", function (e) {
    e.preventDefault();

    // Check table Capacity to limit number of person
    let tables = [];
    let totalCapacity = 0;
    let numberOfPerson = parseInt(
      $(this).find("input[name='NumberOfPerson']").val()
    );

    $(".selected-table").each(function () {
      let id = $(this).data("id");
      let capacity = $(this).data("capacity");

      tables.push(id);
      totalCapacity += parseInt(capacity);
    });

    if (totalCapacity < numberOfPerson) {
      console.log("Exdcced");
      $(this)
        .find('span[data-valmsg-for="NumberOfPerson"]')
        .text("Number of person must be less than table capacity!");
      return;
    }

    var disabled = $(this)
      .find("select:disabled, input:disabled")
      .prop("disabled", false);
    var formData = new FormData($(this)[0]);
    disabled.prop("disabled", true);
    // formData.append("tables", tables);

    for (var i = 0; i < tables.length; i++) {
      formData.append("tables[" + i + "]", tables[i]);
    }

    $.ajax({
      type: "POST",
      url: "/OrderApp/AssignTables",
      data: formData,
      contentType: false,
      processData: false,
      success: function (response) {
        if (response.success == true) {
          toastr.success(response.message);
          window.location.href = "/OrderApp/Menu/" + response.data;
        } else {
          toastr.error(response.message);
        }
      },
    });
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
          console.log(response);
          fillCustomerDetails(form, response.data);
        }
      },
    });
  });
});
