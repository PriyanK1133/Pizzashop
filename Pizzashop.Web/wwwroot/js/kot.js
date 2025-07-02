const itemStatus = Object.freeze({
  InProgress: "InProgress",
  Ready: "Ready",
  All: "",
});

let status = itemStatus.InProgress,
  categoryId;

function getKOTOrders() {
  var id = $(".carousel-indicator-btn.active").attr("id");
  $.ajax({
    type: "GET",
    url: "/OrderApp/GetKOTOrders",
    data: { categoryId, status },
    success: function (response) {
      var kotPartial = $("#kot-list-partial");
      kotPartial.html(response);
      updateTime();
      $(`#${id}`).click();
    },
  });
}

function openKOTOrder(order) {
  if (status == itemStatus.All) {
    return;
  }
  var kotModal = $("#kot-order-modal");
  kotModal
    .find("#order-id")
    .text(order.id.slice(-5).toUpperCase())
    .data("id", order.id);
  var body = "";

  for (var orderItem of order.orderItems) {
    var modifiers = "";
    for (var modifier of orderItem.orderModifiers) {
      modifiers += `<li class="text-secondary"><small>${modifier.name}</small> </li>`;
    }

    body += `<div class="d-flex justify-content-between p-2 kot-item" >
                <div>
                    <div class="form-check">
                        <input class="form-check-input kot-item-checkbox border-dark border-opacity-50" role="button" type="checkbox"  id="${orderItem.id}-item-checkbox" >
                        <label class="form-check-label text-start" for="checkChecked">
                            ${orderItem.name}
                        </label>
                    </div>
                    <ul class="m-1 text-start">
                    ${modifiers}
                    </ul>
                </div>
                <div class="quantity-btn border-primary rounded-1 border" style>
                    <div class="input-group d-flex align-items-center justify-content-between">
                        <span class="input-group-btn">
                            <button type="button" class="quantity-left-minus btn btn-number" data-id="${orderItem.id}"
                                data-type="minus" >
                                <i class="bi bi-dash-lg"></i>
                            </button>
                        </span>
                        <div>
                            <span type="text" id="${orderItem.id}-quantity" name="quantity"
                                class="form-control input-number border-0" >${orderItem.quantity}</span>
                        </div>
                        <button type="button" class="quantity-right-plus btn btn-number" data-id="${orderItem.id}"
                            data-type="plus" data-max="${orderItem.quantity}" >
                            <i class="bi bi-plus-lg "></i>
                        </button>
                    </div>
                </div>
            </div>`;
  }

  $("#kot-order-items").html(body);
  var submitBtn = kotModal.find("#submit-btn");
  if (status == itemStatus.InProgress) {
    submitBtn.text("Mark As Prepared");
  } else {
    submitBtn.text("Mark As In Progress");
  }
  kotModal.modal("show");
}

$(document).ready(function () {
  getKOTOrders();

  setInterval(updateTime, 1000);

  $(".category-tab").click(function () {
    var id = $(this).attr("id");
    var name = $(this).data("name");
    if (id == "All") {
      categoryId = null;
    } else {
      categoryId = id;
    }
    $("#tab-header").html(name);
    getKOTOrders();
  });

  $(".item-status").click(function () {
    if ($(this).hasClass("active-btn")) {
      $(".item-status").removeClass("active-btn");
      status = itemStatus.All;
    } else {
      $(".item-status").removeClass("active-btn");
      $(this).addClass("active-btn");
      status = $(this).data("status");
    }
    getKOTOrders();
  });

  //#region Quantity

  $(document).on("click", ".quantity-right-plus", function (e) {
    e.preventDefault();
    var id = $(this).data("id");
    var quantity = parseInt($(`#${id}-quantity`).text());

    var max = parseInt($(this).data("max"));
    console.log("max", max, id);
    if (quantity < max) {
      quantity += 1;
      $(`#${id}-quantity`).text(quantity);
    }
  });

  var clickTimer;
  $(document).on("mousedown", ".quantity-right-plus", function (e) {
    e.preventDefault();
    clickTimer = setInterval(() => {
      $(this).click();
    }, 100);
  });

  // minus button
  $(document).on("click", ".quantity-left-minus", function (e) {
    e.preventDefault();
    var id = $(this).data("id");
    var quantity = parseInt($(`#${id}-quantity`).text());

    if (quantity > 1) {
      quantity -= 1;
      $(`#${id}-quantity`).text(quantity);
    }
  });

  $(document).on("mousedown", ".quantity-left-minus", function (e) {
    e.preventDefault();
    clickTimer = setInterval(() => {
      $(this).click();
    }, 100);
  });

  // clear timer
  window.onmouseup = function () {
    clearInterval(clickTimer);
  };
  //#endregion Quantity

  // Mark As Prepared
  var kotModal = $("#kot-order-modal");
  kotModal.find("#submit-btn").click(function () {
    if (status == itemStatus.All) {
      return;
    }

    var data = {
      orderId: kotModal.find("#order-id").data("id"),
      itemStatus: status,
      orderItems: [],
    };

    if (status == itemStatus.InProgress) {
      data.itemStatus = itemStatus.Ready;
    } else if (status == itemStatus.Ready) {
      data.itemStatus = itemStatus.InProgress;
    }

    $(".kot-item-checkbox:checked").each(function () {
      console.log($(this));
      var orderItemId = $(this).attr("id").replace("-item-checkbox", "");
      var quantity = $(this)
        .parents(".kot-item")
        .find(`#${orderItemId}-quantity`)
        .text();

      data.orderItems.push({ id: orderItemId, quantity });
    });

    if (data.orderItems.length == 0) {
      toastr.error("No Items Selected to update");
      return;
    }
    
    $.ajax({
      type: "POST",
      url: "/OrderApp/UpdateOrderItemsStatus",
      data: data,
      success: function (response) {
        if (response.success == true) {
          toastr.success(response.message);
        } else {
          toastr.error(response.message);
        }
        getKOTOrders();
        kotModal.modal("hide");
      },
    });
  });
});
