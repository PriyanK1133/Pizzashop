let existingModifiersPage = 1;
let modifierList = [];
let tempList = [];
let modifiersPage = 1;
var modifiersToDelete = [];
let validator = null;

$.validator.addMethod("notContain", function (val, element, params) {
  return !val.includes(params);
});

function removeModifier(array, idToDelete) {
  const index = array.indexOf(idToDelete);

  if (index !== -1) {
    array.splice(index, 1);
  }
}

function submitData(action, formdata) {
  $.ajax({
    type: "POST",
    url: `/Menu/${action}`,
    data: formdata,
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        $("#category").load("/Menu/GetCategories", function () {
          $(`#${response.data.id}`).click();
        });
      } else {
        toastr.error(response.message);
      }
    },
    error: function () {
      toastr.error("Category not added!!");
    },
  });
}

function loadData(action, element, id) {
  $.ajax({
    type: "GET",
    url: `/Menu/${action}`,
    data: { id: id },
    success: function (data) {
      element.html(data);
    },
  });
}

function setModalData(id, name, description) {
  var modal = $("#addCategoryModal");
  modal.find("#categoryId").val(id);
  modal.find("#categoryName").val(name);
  modal.find("#description").val(description);
  modal.find("#exampleModalLabel").text("Update Category");
  modal.modal("show");
}

function submitGroupData(action, formdata) {
  return $.ajax({
    type: "POST",
    url: `/Menu/${action}`,
    data: formdata,
    success: function (data) {
      if (data.success) {
        toastr.success(data.message);
      } else {
        toastr.error(data.message);
      }
    },
    error: function () {
      toastr.error("Something went wrong!!");
    },
  });
}

function loadGroupData(element, id) {
  $.ajax({
    type: "GET",
    url: `/Menu/GetPagedModifiers`,
    data: { modifierGroupId: id },
    success: function (data) {
      element.html(data);
    },
  });
}

function clearGroupModalData() {
  let modal = $("#addGroupModal");
  modal.find("form")[0].reset();
  modifierList = [];
  validator.resetForm();
  $("#display-selected-modifier").empty();
  modal.find("#group-title").text("Add Modifier Group");
}

function clearAddCategoryModal() {
  let form = $("#add-category-form");
  form[0].reset();
  categoryValidator.resetForm();
}

function setGroupModalData(id, name, description) {
  let displaySelectedModifier = $("#display-selected-modifier");

  clearGroupModalData();
  let modal = $("#addGroupModal");
  modal.find("#groupId").val(id);
  modal.find("#groupName").val(name);
  modal.find("#group-description").val(description);
  modal.find("#group-title").text("Update Modifier Group");

  $.ajax({
    type: "GET",
    url: "/Menu/GetModifierDetailsByModifierGroup",
    data: { id },
    success: function (response) {
      if (response.success) {
        data = response.data;
        let body = "";
        modifierList = [];

        $.each(data, function (i, val) {
          modifierList.push({ id: val.id, name: val.name });
          body +=
            '<span data-id="' +
            val.id +
            '" class="rounded-pill text-nowrap bg-secondary-subtle px-2 py-1 fs-6 m-2">' +
            val.name +
            '<button type="button" class="btn remove-selected-modifier-btn">X</button></span>';
        });
        displaySelectedModifier.html(body);

        $(".remove-selected-modifier-btn")
          .off("click")
          .click(function () {
            let id = $(this).parent().data("id");
            modifierList = modifierList.filter((obj) => obj.id != id);
            $(this).parent().remove();
          });
      } else {
        toastr.error("Something went wrong!");
        return;
      }
    },
  });

  modal.modal("show");
}

function checkExistingModifiers() {
  $.each(tempList, function (i, val) {
    $(`#${val.id}`).attr("checked", true);
  });
  let isAllChecked =
    $(".select-modifier-check:checked").length ==
    $(".select-modifier-check").length;
  $(".all-select-modifier-check").prop("checked", isAllChecked);
}

function loadAllPagedModifiers() {
  let searchString = $("#all-modifier-search").val();
  let pageSize = $("#select-modifier-pagination #pagesizelist").val();

  $.ajax({
    type: "GET",
    url: `/Menu/GetAllPagedModifiers`,
    data: { searchString, page: existingModifiersPage, pageSize },
    success: function (data) {
      $("#existing-modifiers-partial").html(data);
      checkExistingModifiers();
    },
  });
}

function closeExistingModifierModal() {
  $(".select-modifier-check").prop("checked", false);
  $.each(modifierList, function (i, val) {
    $(`#${val.id}`).prop("checked", true);
    let isAllChecked =
      $(".select-modifier-check:checked").length ==
      $(".select-modifier-check").length;
    $(".all-select-modifier-check").prop("checked", isAllChecked);
  });
  tempList = modifierList.slice();
}

function loadPagedItems() {
  let categoryId = $(".item-btn.active").attr("id");
  let searchString = $("#search").val();
  let pageSize = $("#pagesizelist").val();

  $.ajax({
    type: "GET",
    url: `/Menu/GetPagedItems`,
    data: { categoryId, searchString, page, pageSize },
    success: function (data) {
      dataTable.html(data);
    },
  });
}

function loadData(action, element) {
  $.ajax({
    type: "GET",
    url: `/Menu/${action}`,
    success: function (data) {
      element.html(data);
    },
  });
}

function loadPagedModifiers() {
  let modifierGroupId = $(".modifier-btn.active").attr("id");
  let searchString = $("#modifier-search").val();
  let pageSize = $("#modifier-pagination #pagesizelist").val();

  $.ajax({
    type: "GET",
    url: `/Menu/GetPagedModifiers`,
    data: { modifierGroupId, searchString, page: modifiersPage, pageSize },
    success: function (data) {
      $("#data-table-modifier").html(data);
    },
  });
}

function getModifiers(id, name) {
  $.ajax({
    type: "GET",
    url: "/Menu/GetModifierDetailsByModifierGroup",
    async: false,
    data: { id },
    success: function (response) {
      if (response.success) {
        data = response.data;
        var options = "";
        for (var i = 0; i <= data.length; i++) {
          options += `<option value=${i}>${i}</option>`;
        }

        var modifierList = "";
        $.each(data, function (i, val) {
          modifierList += `<li>${val.name}</li>`;
        });

        var modifierPriceList = "";
        $.each(data, function (i, val) {
          modifierPriceList += `<li>${val.rate}</li>`;
        });

        var body = `<div class="row selected-modifier-group" data-id="${id}">
                                <div class="d-flex align-items-center justify-content-between">
                                    <h4>${name}</h4>
                                    <div class="remove-selected-modifier-group" data-id="${id}"><img  src="./images/icons/trash.svg" ></div>
                                </div>
                                <div class="col">
                                    <select name="min-${name.replaceAll(
                                      " ",
                                      "_"
                                    )}" data-name="${name.replaceAll(
          " ",
          "_"
        )}" class="form-select rounded-pill w-100 min-selection">
                                        ${options}
                                    </select>
                                    <ul>
                                        ${modifierList}
                                    </ul>
                                </div>
                                <div class="col">
                                    <select name="max-${name.replaceAll(
                                      " ",
                                      "_"
                                    )}" data-name="${name.replaceAll(
          " ",
          "_"
        )}" class="form-select rounded-pill w-100 max-selection">
                                        ${options}
                                    </select>
                                    <ul class="list-unstyled text-end">
                                        ${modifierPriceList}
                                    </ul>
                                </div>

                        </div>`;
        $("#modifier-group-selection").append(body);
      } else {
        toastr.error("Something went wrong!");
        return;
      }
    },
  });
}
$(document).ready(function () {
  validator = $("#add-group-form").validate({
    rules: {
      name: {
        required: true,
        maxlength: 20,
      },
    },
    messages: {
      name: {
        required: "Name is required!",
        maxlength: "Modifier Group Name must be less than 20 character!",
      },
    },
    highlight: function (element) {
      $(element).parent().addClass("error");
    },
    unhighlight: function (element) {
      $(element).parent().removeClass("error");
    },
  });

  $("#category").load("/Menu/GetCategories");

  $("#modifier-group").load("/Menu/GetModifierGroups", function () {
    $(".modifier").first().addClass("active").click();
    makeSortable(
      $("#modifier-groups-sortable"),
      "/Menu/ChangeModifierGroupsOrder"
    );
  });

  $(document).on("click", ".remove-selected-modifier-group", function () {
    let id = $(this).data("id");
    $(`#${id}-btn`).removeAttr("disabled");
    $(this).parent().parent().remove();
  });

  //#region ModifierGroup
  $(document).on("click", ".modifier-btn", function () {
    $(".modifier").removeClass("active");
    $(this).addClass("active");
    modifiersToDelete = [];
    modifiersPage = 1;
    $("#modifier-search").val("");
    loadPagedModifiers();
  });

  $(document).on("keyup", "#add-group-form", function () {
    $(this).valid();
  });

  $(document).on("submit", "#add-group-form", function (e) {
    e.preventDefault();
    if (!$(this).valid()) {
      return;
    }
    let formData = {
      model: {
        name: $("#groupName").val(),
        description: $("#group-description").val(),
      },
      modifierIds: modifierList.map((modifier) => modifier.id),
    };

    let id = $("#groupId").val();
    if (id.length != 0) {
      formData.model.id = id;
      submitGroupData("EditModifierGroup", formData).done(() => {
        $("#modifier-group").empty();
        $("#modifier-group").load("/Menu/GetModifierGroups", function () {
          $(`#${id}`).click();
        });
      });
    } else {
      submitGroupData("AddModifierGroup", formData).done(() => {
        $("#modifier-group").empty();
        $("#modifier-group").load("/Menu/GetModifierGroups", function () {
          $(".modifier").first().click();
        });
      });
    }
    $("#addGroupModal").modal("hide");
  });

  $(document).on("click", "#delete-group-btn", function () {
    let id = $(this).data("id");
    submitGroupData("DeleteModifierGroup", { id: id }).done(() => {
      $("#modifier-group").empty();
      $("#modifier-group").load("/Menu/GetModifierGroups", function () {
        $(".modifier-btn").first().click();
      });
    });
  });

  $(document).on("click", ".delete-modifier-group-btn", function () {
    let id = $(this).data("id");
    $("#delete-group-btn").data("id", id);
  });

  $(document).on("click", "#add-modifier-btn", function (e) {
    e.preventDefault();
    let addModifierModal = $("#add-modifier-modal");
    $.ajax({
      type: "GET",
      url: `/Menu/GetModifierById`,
      success: function (data) {
        addModifierModal.html(data);
        addModifierModal.modal("show");
      },
    });
  });

  $(document).on("click", "#add-existing-modifier", function () {
    tempList = modifierList.slice();
    loadAllPagedModifiers();
    $("#existing-modifiers-modal").modal("show");

    $("#addGroupModal").modal("hide");
  });
  //#endregion ModifierGroup

  //#region Add Existing Modifiers
  $(document).on("change", ".select-modifier-check", function () {
    let isChecked = $(this).is(":checked");
    let obj = $(this).data("obj");
    let id = obj.id;
    if (isChecked) {
      tempList = tempList.filter((obj) => obj.id != id);
      tempList.push(obj);
    } else {
      tempList = tempList.filter((obj) => obj.id != id);
    }
    let isAllChecked =
      $(".select-modifier-check:checked").length ==
      $(".select-modifier-check").length;
    $(".all-select-modifier-check").prop("checked", isAllChecked);
  });

  $(document).on("change", ".all-select-modifier-check", function () {
    let checked = $(this).is(":checked");
    $(".select-modifier-check").prop("checked", checked);
    $(".select-modifier-check").change();
  });

  $(document).on("click", "#add-selected-modifier-btn", function () {
    let displaySelectedModifier = $("#display-selected-modifier");
    modifierList = tempList.slice();
    let body = "";
    $.each(tempList, function (i, val) {
      body +=
        '<span data-id="' +
        val.id +
        '" class="rounded-pill text-nowrap bg-secondary-subtle px-2 py-1 fs-6 m-2">' +
        val.name +
        '<button type="button" class="btn remove-selected-modifier-btn" >X</button></span>';
    });

    displaySelectedModifier.html(body);

    $(".modal").modal("hide");
    $("#addGroupModal").modal("show");

    $(".remove-selected-modifier-btn").click(function () {
      let id = $(this).parent().data("id");
      modifierList = modifierList.filter((obj) => obj.id != id);
      $(this).parent().remove();
    });
  });

  // Pagination
  $(document).on(
    "keyup",
    "#all-modifier-search",
    debounce(function () {
      existingModifiersPage = 1;
      loadAllPagedModifiers();
    }, 200)
  );

  $(document).on(
    "click",
    "#select-modifier-pagination #next-page-btn",
    function () {
      existingModifiersPage = existingModifiersPage + 1;
      loadAllPagedModifiers();
    }
  );

  $(document).on(
    "click",
    "#select-modifier-pagination #prev-page-btn",
    function () {
      existingModifiersPage = existingModifiersPage - 1;
      loadAllPagedModifiers();
    }
  );

  $(document).on(
    "change",
    "#select-modifier-pagination #pagesizelist",
    function () {
      existingModifiersPage = 1;
      loadAllPagedModifiers();
    }
  );

  //#endregion Add Existing Modifiers

  //#region Modifiers
  $(document).on("click", ".edit-modifier-btn", function () {
    let id = $(this).data("id");
    let addModifiermodal = $("#add-modifier-modal");

    $.ajax({
      method: "GET",
      url: "/Menu/GetModifierById",
      data: { id: id },
      success: function (data) {
        addModifiermodal.html(data);
        addModifiermodal.modal("show");
      },
    });
  });

  $(document).on("click", ".delete-modifier-btn", function () {
    var id = $(this).data("id");
    var deleteModal = $("#delete-modifier-modal").find("#delete-modal");
    deleteModal.modal("show");
    deleteModal.find("#delete-btn").data("id", id);
  });

  $(document).on("click", "#delete-modifier-modal #delete-btn", function () {
    var id = $(this).data("id");
    $.ajax({
      type: "POST",
      url: "/Menu/DeleteModifier",
      data: { id },
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          $(".modifier.active").trigger("click");
        } else {
          toastr.error(response.message);
        }
        deleteModal.modal("hide");
      },
    });
  });

  $(document).on("change", ".modifier-check", function () {
    var checked = $(this).is(":checked");
    var id = $(this).data("id");
    if (checked) {
      modifiersToDelete.push(id);
    } else {
      removeModifier(modifiersToDelete, id);
    }

    var isAllChecked =
      $(".modifier-check:checked").length == $(".modifier-check").length;
    $(".all-modifier-check").prop("checked", isAllChecked);
  });

  $(document).on("change", ".all-modifier-check", function () {
    var checked = $(this).is(":checked");
    $(".modifier-check").prop("checked", checked);
    modifiersToDelete = [];
    $(".modifier-check").change();
  });

  $(document).on("click", "#delete-multiple-modifiers", function () {
    if (modifiersToDelete.length == 0) {
      toastr.error("No Modifiers Selected!");
      return;
    }
    $("#multiple-modifier-delete-modal").find("#delete-modal").modal("show");
  });

  $(document).on(
    "click",
    "#multiple-modifier-delete-modal #delete-btn",
    function () {
      $.ajax({
        type: "POST",
        url: "/Menu/DeleteManyModifier",
        data: { ids: modifiersToDelete },
        success: function (response) {
          if (response.success) {
            toastr.success(response.message);
            $(".modifier.active").click();
          } else {
            toastr.error(response.message);
          }
        },
      });
    }
  );

  $(document).on(
    "keyup",
    "#modifier-search",
    debounce(function () {
      modifiersPage = 1;
      loadPagedModifiers();
    }, 200)
  );

  $(document).on("click", "#modifier-pagination #next-page-btn", function () {
    modifiersPage = modifiersPage + 1;
    loadPagedModifiers();
  });

  $(document).on("click", "#modifier-pagination #prev-page-btn", function () {
    modifiersPage = modifiersPage - 1;
    loadPagedModifiers();
  });

  $(document).on("change", "#modifier-pagination #pagesizelist", function () {
    modifiersPage = 1;
    loadPagedModifiers();
  });
  //#endregion Modifiers
});
