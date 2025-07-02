// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function debounce(func, delay) {
  let timeout;
  return function (...args) {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
      func.apply(this, args);
    }, delay);
  };
}

function isEmptyGuid(guid) {
  return guid === "00000000-0000-0000-0000-000000000000";
}

// #region  Loadding
function startLoading() {
  $("#loader-container").removeClass("d-none");
}

function stopLoading() {
  $("#loader-container").addClass("d-none");
}

function ajaxLoading(func) {
  startLoading();
  func()
    .done(function () {
      stopLoading();
    })
    .fail(function () {
      stopLoading();
    });
}
//#endregion Loading

function makeSortable(element, url) {
  element.sortable({
    handle: ".for-drag",
    axis: "y",
    containment: "parent",
    update: function (event, ui) {
      let sortedIds = [];
      element.find(".for-order:visible").each(function () {
        sortedIds.push($(this).data("id"));
      });

      $.ajax({
        url: url,
        type: "POST",
        data: { sortedIds },
        success: function (response) {
          console.log("Order updated successfully");
        },
        error: function () {
          console.error("Failed to update order");
        },
      });
    },
  });
  element.disableSelection();
}

// Update Live Time

function updateTime() {
  $(".live-time").each(function () {
    var dateString = "";
    var time = $(this).data("time");
    var isSecondInclude = $(this).data("sec");
    if (!time) {
      $(this).text("0 Min");
      return;
    }
    var date1 = new Date(time);
    var date2 = new Date();
    var diff = date2.getTime() - date1.getTime();

    var days = Math.floor(diff / (1000 * 60 * 60 * 24));
    diff -= days * (1000 * 60 * 60 * 24);

    let flag = false;

    if (days > 0) {
      dateString += days + " days ";
      flag = true;
    }

    var hours = Math.floor(diff / (1000 * 60 * 60));
    diff -= hours * (1000 * 60 * 60);

    if (flag || hours > 0) {
      dateString += hours + " hours ";
      flag = true;
    }

    var mins = Math.floor(diff / (1000 * 60));
    diff -= mins * (1000 * 60);

    dateString += mins + " min ";

    var seconds = Math.floor(diff / 1000);
    diff -= seconds * 1000;

    if (!(isSecondInclude == false)) {
      dateString += seconds + " sec";
    }

    $(this).text(dateString);
  });
}

$(document).ready(function () {
  window.addEventListener("beforeunload", function () {
    document.getElementById("screenLoader").style.display = "flex";
  });

  window.addEventListener("load", function () {
    const loader = document.getElementById("screenLoader");

    if (loader) {
      loader.style.display = "none";
    }

    $("#screenLoader").fadeOut(200);
  });

  //#region Image Handling

  $(document).on("change", "#formFile", function (e) {
    var preview = $("#preview");
    var file = e.target.files[0];
    var regex = /.+(.jpg|.jpeg|.png)$/;
    if (regex.test($(this).val().toLowerCase())) {
      if (typeof FileReader != "undefined") {
        var reader = new FileReader();
        reader.onload = function (e) {
          var img = preview.find("img");
          var btn = preview.find("button");

          img.attr("style", "height:150px;width: 150px");
          img.addClass("rounded-circle");
          img.attr("src", e.target.result);
          btn.removeClass("d-none");
          preview.removeClass("py-5").addClass("py-1");
          preview.find("span").remove();
        };
        reader.readAsDataURL(file);
      } else {
        toastr.error("This browser does not support FileReader.");
      }
    } else {
      $("#preview button").click();
      toastr.error("Please upload a valid image file.");
      $(this).val("");
    }
  });

  $(document).on("click", "#preview button", function () {
    $("#formFile").val("");
    $("#current-image").val("");
    var img = $(this).parent().find("img");
    img.attr("src", "/images/icons/upload-files.svg");
    img.removeClass("rounded-circle");
    img.removeAttr("style width height");
    $(this).addClass("d-none");
    $(this)
      .parent()
      .removeClass("py-1")
      .addClass("py-5")
      .find("span:first")
      .remove();
    $(this).parent().append("<span>Drag And Drop Or Browse Files</span>");
  });

  // Drag And Drop Image
  $(document).on("dragover drop", function (e) {
    e.preventDefault();
  });

  $(document).on("dragover", "#preview", function (e) {
    e.preventDefault();
    $(this).parent().addClass("border-primary");
  });

  $(document).on("dragleave drop", "#preview", function (e) {
    e.preventDefault();
    $(this).parent().removeClass("border-primary");
  });

  $(document).on("drop", "#preview", function (e) {
    var files = e.originalEvent.dataTransfer.files;
    if (files.length > 0) {
      $("#formFile").prop("files", files);
      $("#formFile").trigger("change");
    }
  });
  //#endregion Image Handling
});
