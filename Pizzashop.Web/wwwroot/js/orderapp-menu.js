let taxTypes = {
  Percentage: "Percentage",
  FlatAmount: "Flat Amount",
};
let orderComment;
let initialStateOfOrderItems;

function updateAmount() {
  let menuTableSidebar = $("#menu-table-sidebar");
  let orderItemsDiv = menuTableSidebar.find(".order-item");
  let subtotal = menuTableSidebar.find("#order-subtotal");
  let orderTaxes = menuTableSidebar.find(".order-tax");

  let subtotalAmount = 0;
  let totalItemTaxAmount = 0;
  let totalTaxAmount = 0;

  orderItemsDiv.each(function () {
    let itemRate = parseFloat($(this).data("rate"));
    let modifiersRatePerItem = 0;

    $(this)
      .find(".item-modifier-rate")
      .each(function () {
        modifiersRatePerItem += parseFloat($(this).text());
      });

    let quantity = parseInt($(this).find(".quantity").text());
    let itemTax = parseFloat($(this).data("item-tax"));

    let itemTaxAmount = itemTax * 0.01 * itemRate * quantity;

    $(this)
      .find(".item-total")
      .text((itemRate * quantity + itemTaxAmount).toFixed(2));
    $(this)
      .find(".item-modifiers-total")
      .text((modifiersRatePerItem * quantity).toFixed(2));

    totalItemTaxAmount += itemTaxAmount;

    subtotalAmount +=
      (itemRate + modifiersRatePerItem) * quantity + itemTaxAmount;
  });
  subtotal.text(subtotalAmount.toFixed(2));

  if (subtotalAmount == 0) {
    menuTableSidebar.find(".tax-amount").text(0);
    menuTableSidebar.find("#order-total").text(0);

    updateOrderItemCount();
    if (
      menuTableSidebar.find("#order-items").html() == initialStateOfOrderItems
    ) {
      menuTableSidebar.find("#save-order-btn").prop("disabled", true);
    } else {
      menuTableSidebar.find("#save-order-btn").prop("disabled", false);
    }
    return;
  }

  orderTaxes.each(function () {
    let type = $(this).data("type");
    let rate = parseFloat($(this).data("rate"));
    let taxAmount = 0;

    if (type == taxTypes.FlatAmount) {
      taxAmount = rate;
    } else {
      taxAmount = rate * 0.01 * subtotalAmount;
    }

    $(this).find(".tax-amount").text(taxAmount.toFixed(2));
    totalTaxAmount += taxAmount;
  });

  menuTableSidebar
    .find("#order-total")
    .text(parseFloat(subtotalAmount + totalTaxAmount).toFixed(2));

  updateOrderItemCount();
  if (
    menuTableSidebar.find("#order-items").html() == initialStateOfOrderItems
  ) {
    menuTableSidebar.find("#save-order-btn").prop("disabled", true);
  } else {
    menuTableSidebar.find("#save-order-btn").prop("disabled", false);
  }
}

function loadMenuItems() {
  let categoryId = $(".category-tab.active").data("id");
  let action;
  if (categoryId == "favourite") {
    action = "GetFavoriteItems";
  } else {
    action = "GetItemsByCategoryId";
  }

  $.ajax({
    type: "GET",
    url: `/OrderApp/${action}`,
    data: { categoryId },
    success: function (response) {
      $("#menu-items-partial").html(response);
    },
  });
}

function loadCategories() {
  let categoryList = $("#category-list");
  $.ajax({
    type: "GET",
    url: "/OrderApp/GetAllCategories",
    success: function (response) {
      if (response.success) {
        $.each(response.data, function (i, category) {
          categoryList.append(
            `<li role="button" class="category-tab fw-semibold px-3 py-2 " data-id="${category.id}">${category.name}</li>`
          );
        });
      }
    },
  });
}

function checkIfItemAlreadyPrepared(orderItemId, quantity) {
  let res;
  $.ajax({
    type: "GET",
    url: "/OrderApp/IsItemQuantityPrepared",
    data: { orderItemId, quantity },
    async: false,
    success: function (response) {
      res = response;
    },
  });
  return res;
}

function IsOrderServed() {
  let completeOrderBtn = $("#menu-table-sidebar #complete-order-btn");
  let generateInvoiceBtn = $("#menu-table-sidebar #generate-invoice-btn");
  let cancelOrderBtn = $("#menu-table-sidebar #cancel-order-btn");
  let orderId = completeOrderBtn.parent().data("order-id");

  setInterval(() => {
    $.ajax({
      type: "GET",
      url: "/OrderApp/IsOrderServed",
      data: { orderId },
      global: false,
      success: function (response) {
        if (response.data) {
          completeOrderBtn.prop("disabled", false);
          cancelOrderBtn.prop("disabled", true);
          generateInvoiceBtn.removeClass("disabled");
        } else {
          completeOrderBtn.prop("disabled", true);
          cancelOrderBtn.prop("disabled", false);
          generateInvoiceBtn.addClass("disabled");
        }
      },
    });
  }, 1000);
}

function updateOrderItemCount() {
  let countDisplay = $("#order-item-count");
  let count = $("#menu-table-sidebar .order-item").length;

  countDisplay.text(count);
}

function addItemInMenuTable(item) {
  let accordionItems = $("#menu-table-sidebar #order-items");
  let modifiers = "";
  let modifiersRate = 0;
  let uniqueModifierIdentifier = "";

  for (let modifier of item.modifiersForItem.sort((a,b) => a.id - b.id)) {
    modifiers += `<li class="item-modifier" data-id="${modifier.id}"><small>${modifier.name} &emsp; Rs. <span class="item-modifier-rate">${modifier.rate}</span></small></li>`;
    modifiersRate += parseFloat(modifier.rate);
    uniqueModifierIdentifier += modifier.id.slice(-5);
  }
  let uniqueId = item.id.slice(-5) + "-" + uniqueModifierIdentifier;

  let orderItemDiv = $(`#${uniqueId}-item`);

  if (orderItemDiv.length > 0) {
    orderItemDiv.find(".quantity-right-plus").click();
    return;
  }

  let body = `<div class="accordion-item order-item border-top mb-2" id="${uniqueId}-item" data-item-id="${item.id}" data-item-tax="${item.itemTax}" data-rate="${item.rate}" data-modifiers-total="${modifiersRate}">
                <div class="accordion-header p-1 shadow-sm">
                    <div class="row align-items-center">
                        <div class="col-5 ">
                            <div class="d-flex gap-2 w-100 p-1" data-bs-toggle="collapse"
                                data-bs-target="#${uniqueId}-collapse" aria-expanded="true" type="button">
                                <div class="align-content-center">
                                    <button class="accordion-button collapsed p-0 bg-white border-0"
                                        type="button" data-bs-toggle="collapse"
                                        data-bs-target="#${uniqueId}-collapse" type="button">

                                    </button>
                                </div>
                                <div class="fw-semibold col">
                                    ${item.name}
                                </div>
                            </div>
                            <div id="${uniqueId}-collapse" class="accordion-collapse collapse" >
                                <div class="accordion-body py-0">
                                    <ul class="ps-1 m-0 text-secondary" id="item-modifiers">
                                        ${modifiers}
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div class="col-3">
                            <div class="quantity-btn border-secondary rounded-1 border" style>
                                <div class="input-group d-flex align-items-center justify-content-between ">
                                    <span class="input-group-btn">
                                        <button type="button"
                                            class="quantity-left-minus btn btn-number p-1 h-100" data-id="${uniqueId}"
                                            data-type="minus">
                                            <i class="bi bi-dash-lg"></i>
                                        </button>
                                    </span>
                                    <div>
                                        <span type="text" id="${uniqueId}-quantity" class="quantity"
                                            class="form-control input-number border-0 p-0">1</span>
                                    </div>
                                    <span class="input-group-btn">
                                        <button type="button"
                                            class="quantity-right-plus btn btn-number h-100 p-1" data-id="${uniqueId}"
                                            data-type="plus">
                                            <i class="bi bi-plus-lg"></i>
                                        </button>
                                    </span>
                                </div>
                            </div>
                        </div>
                        <div class="col-2">Rs. <span class="item-total">${item.rate}</span> <br> <small class="text-secondary">Rs. <span class="item-modifiers-total">${modifiersRate}</span></small></div>
                        <div class="col-2">
                            <button class="btn remove-order-item-btn">
                                <i class="bi bi-trash3"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>`;

  accordionItems.append(body);
  updateAmount();
}

$(document).ready(function () {
  loadCategories();
  loadMenuItems();
  IsOrderServed();
  updateOrderItemCount();

  //#region star rating feedback
  $(".star").on("click", function () {
    let rating = $(this).data("rating");
    $(this).parent().find("input").val(rating);
    $(this)
      .parent()
      .find(".star")
      .each(function () {
        if ($(this).data("rating") <= rating) {
          $(this).addClass("active");
          $(this).find("i").removeClass("far text-header").addClass("fas");
        } else {
          $(this).removeClass("active");
          $(this).find("i").removeClass("fas").addClass("far text-header");
        }
      });
  });
  //#endregion star rating feedback

  orderComment = $("#order-comment-form textarea").val();
  initialStateOfOrderItems = $("#menu-table-sidebar #order-items").html();

  $("#complete-order-confirmation-modal")
    .find(".modal-title")
    .text("Complete Confirmation");
  $("#cancel-order-confirmation-modal")
    .find(".modal-title")
    .text("Cancel Confirmation");

  //#region QR Code
  const qrCode = new QRCode(
    document.getElementById("qr-code"),
    window.location.toString()
  );
  //#endregion QR Code

  $(document).on("click", ".category-tab", function () {
    $(".category-tab").removeClass("active");
    $(this).addClass("active");
    loadMenuItems();
    $("#item-search").val("");
  });

  $(document).on("click", ".favourite-icon", function (e) {
    e.stopPropagation();

    $(this).find("i").toggleClass("bi-heart-fill bi-heart");

    let itemId = $(this).data("id");
    let message;
    
    if ($(this).find("i").hasClass("bi-heart-fill")) {
      message = "Item marked as favourite!";
    } else {
      message = "Item removed from favourite!";
    }

    $.ajax({
      type: "PATCH",
      url: "/OrderApp/ToggleFavoriteItem",
      data: { itemId },
      success: function (response) {
        if (response.success) {
          toastr.success(message);
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  $("#item-search").keyup(
    debounce(function () {
      let searchString = $(this).val().toLowerCase();
      let itemCount = 0;
      $(".menu-card").each(function () {
        let name = String($(this).data("name"));
        name = name.toLowerCase();
        if (name.includes(searchString)) {
          $(this).removeClass("d-none");
          itemCount += 1;
        } else {
          $(this).addClass("d-none");
        }

        if (itemCount == 0) {
          $(".menu-items-not-found").removeClass("d-none");
        } else {
          $(".menu-items-not-found").addClass("d-none");
        }
      });
    }, 300)
  );

  // Customer Details Pop up
  $(document).on("click", ".reset-menu-customer-details", function () {
    $(this)
      .parents("form")
      .find("span.text-danger")
      .text("")
      .parent()
      .find("input, textarea, select")
      .removeClass("input-validation-error");
  });

  $(document).on("submit", "#menu-customer-details-form", function (e) {
    e.preventDefault();
    let data = $(this).serialize();

    $.ajax({
      type: "POST",
      url: "/OrderApp/EditCustomerDetails",
      data: data,
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          $("#menu-customer-details-modal").modal("hide");
          $("#menu-customer-details-partial").load(
            "/OrderApp/GetCustomerDetailsPartial",
            response.data
          );
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  // Order Instruction Pop Up
  $(document).on("submit", "#order-comment-form", function (e) {
    e.preventDefault();
    let data = $(this).serialize();

    $.ajax({
      type: "PATCH",
      url: "/OrderApp/EditOrderComment",
      data: data,
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          orderComment = response.data;
        } else {
          toastr.error(response.message);
        }
      },
    });
  });

  //#region Special Instruction
  $(document).on("click", ".item-instruction-btn", function () {
    let itemInstructionModal = $("#item-instruction-modal");
    let orderItemId = $(this).parents(".order-item").data("id");
    itemInstructionModal.find("input[name='id']").val(orderItemId);

    $.ajax({
      type: "GET",
      async: false,
      url: "/OrderApp/GetOrderItemInstruction",
      data: { id: orderItemId },
      success: function (response) {
        itemInstructionModal.find("#special-instruction").val(response.data);
      },
    });

    itemInstructionModal.modal("show");
  });

  $(document).on("submit", "#item-instruction-form", function (e) {
    e.preventDefault();
    let data = $(this).serialize();

    $.ajax({
      type: "PATCH",
      url: "/OrderApp/SaveOrderItemInstruction",
      data: data,
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
        } else {
          toastr.error(response.message);
        }
      },
    });
  });
  //#endregion Special Instruction

  $(document).on("reset", "#order-comment-form", function (e) {
    e.preventDefault();
    $(this).find("textarea").val(orderComment);
  });

  //#region Add Modifier Modal
  $(document).on("click", ".menu-card", function () {
    let itemId = $(this).data("id");
    let itemName = $(this).data("name");
    let itemTypeClass = $(this).data("class");
    let itemRate = $(this).data("rate");
    let itemTaxPercentage = $(this).data("item-tax");

    $.ajax({
      type: "GET",
      url: "/OrderApp/GetModifierGroupsForItem",
      data: { itemId },
      success: function (response) {
        let addModifiersModal = $("#add-modifiers-modal");
        addModifiersModal.find(".modal-body").html(response);
        addModifiersModal.find(".modal-title").text(itemName);
        addModifiersModal.find(".modifier-card").addClass(itemTypeClass);
        addModifiersModal
          .find("#add-btn")
          .data("item-id", itemId)
          .data("itemName", itemName)
          .data("rate", itemRate)
          .data("item-tax", itemTaxPercentage);

        let modifiersLength = addModifiersModal.find(".modifier-card").length;
        if (
          modifiersLength == 0 &&
          addModifiersModal.find("#add-btn").length > 0
        ) {
          addModifiersModal.find("#add-btn").click();
          return;
        }
        addModifiersModal.modal("show");
      },
    });
  });

  $(document).on("click", "#add-modifiers-partial .modifier-card", function () {
    let selectedModifiersLength = $(this)
      .parents(".modifier-group")
      .find(".modifier-card.selected").length;
    let maxSelection = parseInt($(this).parents(".modifier-group").data("max"));
    let modifierId = $(this).data("id");

    if (
      $(`.modifier-card.selected[data-id='${modifierId}']`).length &&
      $(this).hasClass("selected") == 0
    ) {
      toastr.error("Modifier Already Added!");
      return;
    }

    if (
      selectedModifiersLength < maxSelection ||
      $(this).hasClass("selected")
    ) {
      $(this).toggleClass("selected border-primary border-secondary");
      $(this).find(".modifier-card-body").toggleClass("bg-primary-subtle");
    }
  });

  //#endregion Add Modifier Modal

  //#region Add Modifiers to Item
  $(document).on("click", "#add-modifiers-modal #add-btn", function () {
    let applicableModifierGroups = $(this)
      .parents(".modal-content")
      .find(".modifier-group");
    let modifiersForItem = [];
    let isValid = true;

    applicableModifierGroups.each(function () {
      let minSelection = $(this).data("min");
      let maxSelection = $(this).data("max");
      let selectedModifiers = $(this).find(".modifier-card.selected");

      if (
        selectedModifiers.length > maxSelection ||
        selectedModifiers.length < minSelection
      ) {
        toastr.error(
          `Select Modifiers between ${minSelection} to ${maxSelection} for ${$(
            this
          ).data("name")}`
        );
        isValid = false;
      }

      selectedModifiers.each(function () {
        modifiersForItem.push({
          id: $(this).data("id"),
          name: $(this).data("name"),
          rate: $(this).data("rate"),
        });
      });
    });

    if (isValid) {
      let item = {
        id: $(this).data("item-id"),
        name: $(this).data("itemName"),
        rate: $(this).data("rate"),
        itemTax: $(this).data("item-tax"),
        modifiersForItem: modifiersForItem.sort((a, b) => a.id - b.id),
      };

      addItemInMenuTable(item);
      $("#add-modifiers-modal").modal("hide");
    }
  });
  //#endregion Add Modifiers to Item

  //#region Remove Order Item From Menu Table
  $(document).on("click", ".remove-order-item-btn", function () {
    let orderItemId = $(this).parents(".order-item").data("id");

    if (orderItemId) {
      let response = checkIfItemAlreadyPrepared(orderItemId, 1);
      if (response.data) {
        toastr.error(response.message, "Invalid Operation: ");
        return;
      }
    }

    $(this).parents(".order-item").remove();
    updateAmount();
  });
  //#endregion Remove Order Item From Menu Table

  //#region Save Order
  $(document).on("click", "#menu-table-sidebar #save-order-btn", function () {
    let orderData = {
      id: $(this).parent().data("order-id"),
      orderItems: [],
      orderTaxIds: [],
    };
    let items = $("#menu-table-sidebar .order-item");
    let taxes = $("#menu-table-sidebar .order-tax");

    items.each(function () {
      let orderItemData = {
        id: $(this).data("id"),
        itemId: $(this).data("item-id"),
        quantity: parseInt($(this).find(".quantity").text()),
        modifierIds: [],
      };

      $(this)
        .find(".item-modifier")
        .each(function () {
          let modifierId = $(this).data("id");
          orderItemData.modifierIds.push(modifierId);
        });

      orderData.orderItems.push(orderItemData);
    });

    taxes.each(function () {
      orderData.orderTaxIds.push($(this).data("id"));
    });

    $.ajax({
      type: "POST",
      url: "/OrderApp/SaveOrder",
      async: false,
      data: orderData,
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          startLoading();

          setTimeout(() => {
            window.location.reload();
          }, 1000);
        } else {
          toastr.error(response.message);
        }
      },
    });
  });
  //#endregion Save Order

  //#region Complete Order
  $(document).on(
    "click",
    "#menu-table-sidebar #complete-order-btn",
    function () {
      let orderId = $(this).parent().data("order-id");
      $("#complete-order-confirmation-modal .modal")
        .data("order-id", orderId)
        .modal("show");
    }
  );

  $(document).on(
    "click",
    "#complete-order-confirmation-modal #delete-btn",
    function () {
      let orderId = $(this).parents(".modal").data("order-id");
      let paymentMethod = $("input[name=payment-method]:checked").val();

      $.ajax({
        type: "PATCH",
        url: "/OrderApp/CompleteOrder",
        data: { orderId, paymentMethod },
        success: function (response) {
          if (response.success) {
            toastr.success(response.message);
            $("#customer-review-modal").modal("show");
          } else {
            toastr.error(response.message);
          }
        },
      });
    }
  );
  //#endregion Complete Order

  //#region Cancel Order
  $(document).on("click", "#menu-table-sidebar #cancel-order-btn", function () {
    let orderId = $(this).parent().data("order-id");
    $("#cancel-order-confirmation-modal .modal").modal("show");
    $("#cancel-order-confirmation-modal")
      .find("#delete-btn")
      .attr("href", "/OrderApp/CancelOrder/" + orderId)
      .removeAttr("data-bs-dismiss");
  });

  //#endregion Cancel Order

  //#region Quantity
  $(document).on("click", ".quantity-right-plus", function (e) {
    e.preventDefault();
    var id = $(this).data("id");
    var quantity = parseInt($(`#${id}-quantity`).text());

    quantity += 1;
    $(`#${id}-quantity`).text(quantity);
    updateAmount();
  });

  var clickTimer;
  $(document).on("mousedown", ".quantity-right-plus", function (e) {
    e.preventDefault();
    clickTimer = setInterval(() => {
      $(this).click();
    }, 200);
  });

  // minus button
  $(document).on("click", ".quantity-left-minus", function (e) {
    e.preventDefault();
    var id = $(this).data("id");
    var orderItemId = $(this).parents(".order-item").data("id");
    var quantity = parseInt($(`#${id}-quantity`).text());

    if (quantity > 1) {
      var response = checkIfItemAlreadyPrepared(orderItemId, quantity);
      if (response.data) {
        toastr.error(response.message, "Invalid Operation: ");
        return;
      }
      quantity -= 1;
      $(`#${id}-quantity`).text(quantity);
      updateAmount();
    }
  });

  $(document).on("mousedown", ".quantity-left-minus", function (e) {
    e.preventDefault();
    clickTimer = setInterval(() => {
      $(this).click();
    }, 200);
  });

  // clear timer
  window.onmouseup = function () {
    clearInterval(clickTimer);
  };
  //#endregion Quantity
});
