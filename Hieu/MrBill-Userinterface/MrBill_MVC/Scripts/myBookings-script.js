$(document).ready(function () {


	$(".sort-added").each(function() {
		$(this).click(function() {
			$(this).parents().parents().parents().find(".sort2").fadeOut( function() {
				$(".sort1").fadeIn();
			});
		});
	});

	$(".sort-traveldate").each(function () {
		$(this).click(function () {
			$(this).parents().parents().parents().find(".sort1").fadeOut(function () {
				$(".sort2").fadeIn();
			});
		});
	});


	$(".year-holder li").each(function () {
		$(this).click(function () {
			var month = $(this).text().replace(/^\s+|\s+$/g, '');

			$(".month-header-text").each(function() {
				var bigmonth = $(this).text().replace((new Date).getFullYear(), "").replace(" ", "").toLowerCase();
				if (bigmonth === (month.trim())) {
					$(this).click();
				}
			});
		});
	});


	$(".month-bookings-holder").each(function() {
		$(this).click(function () {
			if ($(this).find(".more-info-booking").is(":visible")) {
				$(this).css("background-color", "lightgray");
				$(this).find(".more-info-booking").slideUp();
				$(this).find(".visible-indicator").removeClass("glyphicon-chevron-down").addClass("glyphicon-chevron-up");

			} else {
				$(this).css("background-color", "#7cc5df");
				$(this).find(".more-info-booking").slideDown();
				$(this).find(".visible-indicator").removeClass("glyphicon-chevron-up").addClass("glyphicon-chevron-down");
			}
			
		});
	})
	//"more-info-booking"

	//$("tr td").each(function () {
	//	var itemText = $(this).text();

	//	if (itemText.length > 20) {
	//		$(this).text(itemText.substring(0, 12) + "...");
	//		$(this).prop('title', itemText);
	//	}
	//	if (itemText.indexOf(", ") >= 0) {
	//		var shortItem = itemText.split(",");
	//		$(this).text(shortItem[0] + "...");
	//		$(this).prop('title', itemText);

	//	}

	//});

	//$(".edit-booking a").each(function() {
	//    $(this).addClass("fui-new");
	//});
	$(this).siblings(".info-box").slideUp("400", function () {
	});
	//$(this).find(".open-close-btn").addClass("fui-radio-unchecked").removeClass("fui-radio-checked");
	
	$(".top-book").each(function () {
		$(this).find(".month-bookings-holder:even").addClass("even");
	});


	$(".month-header").click(function (event) {
		//$(this).siblings(".top-book").slideUp("400", function () {
		//});
		if ($(this).hasClass("disabled-item")) {
			return false;
		}
		//$(this).find(".month-trigger").addClass("fui-radio-unchecked").removeClass("fui-radio-checked");
		$(this).siblings(".bookings").slideUp("100");

		if ($(this).siblings(".bookings").is(":hidden")) {
			//$(this).siblings(".top-book").slideDown("400", function () {
			//});
			$(this).siblings(".bookings").slideDown("100");
			//$(this).find(".month-trigger").addClass("fui-radio-checked").removeClass("fui-radio-unchecked");
		}
	});

	//$(".print").click(function (event) {
	//    event.stopPropagation();
	//    alert("child");
	//});








	//$(".close-book").click(function () {
	//    $(this).siblings(".info-box").slideUp("400", function () {
	//    });
	//    $(this).find(".open-close-btn").addClass("fui-radio-unchecked").removeClass("fui-radio-checked");

	//    if ($(this).siblings(".info-box").is(":hidden")) {
	//        $(this).siblings(".info-box").slideDown("400", function () {
	//        });
	//        $(this).find(".open-close-btn").addClass("fui-radio-checked").removeClass("fui-radio-unchecked");
	//    }
	//});

	$(".month-header").each(function () {
		if ($(this).siblings().find(".top-book").length == 0) {
			$(this).addClass("disabled-item");

		}
	});

	var monthNames = ["januari " + (new Date).getFullYear(), "februari " + (new Date).getFullYear(), "mars " + (new Date).getFullYear(), "april " + (new Date).getFullYear(), "maj " + (new Date).getFullYear(), "juni " + (new Date).getFullYear(), "juli " + (new Date).getFullYear(), "augusti " + (new Date).getFullYear(), "september " + (new Date).getFullYear(), "oktober " + (new Date).getFullYear(), "november " + (new Date).getFullYear(), "december " + (new Date).getFullYear()];
	var d = new Date();

	var currMonth = monthNames[d.getMonth()];
	$(".month-row").each(function () {
		var thisMonth = $(this).find(".month-header-text").text();
		if (currMonth === thisMonth && $(this).siblings(".top-book") && !$(this).find(".month-header").hasClass("disabled-item")) {
			//alert($(this).find(".month-header").find(".month-trigger").length);
			//if ($(this).parents("month-header").hasClass("disabled-item")) {
			$(this).find(".month-header").siblings(".bookings").slideDown("100");
			//$(this).find(".month-header").find(".month-trigger").addClass("fui-radio-checked").removeClass("fui-radio-unchecked");

			//} 
		}
		var receipts = $(this).find(".month-header").siblings().find(".top-book").length;
		if (receipts > 1 || receipts == 0)
			$(this).find(".recepit-number").text(receipts.toString() + " TRANSAKTIONER");

		else
			$(this).find(".recepit-number").text(receipts.toString() + " TRANSAKTION");

		//var cc = $(this).find(".country").text();
		//alert(cc + " == " + country );
		//var country = getCountryName(cc);
		//$(this).find(".country").text(country);
	});

	//$(".add-info").click(function () {
	//    if ($(".add-info-box").is(":visible")) {
	//        $(".add-info-box").hide();
	//        $(".add-info-btn").hide();
	//    }
	//    else {
	//        $(".add-info-box").show();
	//        $(".add-info-btn").show();
	//    }
	//});

	jQuery('a[target^="_new"]').click(function () {
		var width = window.innerWidth * 0.66;
		// define the height in
		var height = width * window.innerHeight / window.innerWidth;
		// Ratio the hight to the width as the user screen ratio
		window.open(this.href, 'newwindow', 'width=' + width + ', height=' + height + ', top=' + ((window.innerHeight - height) / 2) + ', left=' + ((window.innerWidth - width) / 2));

	});

	//$(".print").click(function (e) {
	//    if ($(this).parents(".month-header").hasClass("disabled-item")) {
	//        return false;
	//    }


	//    $(this).parents(".month-header").siblings(".bookings").slideDown("100");
	//    //$(this).siblings(".bookings").parent().find(".open-close-btn").removeClass("fui-radio-unchecked").addClass("fui-radio-checked");
	//    setTimeout(function () {
	//        receiptPrint($(this).parent(".top-book"));

	//    }, 500);
	//    return false;
	//});


	//$(".edit-button").click(function(e) {
	//    var monthHeader = $(this).parents(".month-header");
	//    if (monthHeader.hasClass("disabled-item"))
	//        return false;

	//    $.featherlight($("#edit-box"));
	//    $(".featherlight-close-icon").addClass("fui-cross");

	//    //gets the ID of the transaction
	//    $(this).siblings(".info-box").find(".transpid");


	//});

	$(".add-info").click(function (e) {

		//popitup("/Travel/MyBookings");
		$.featherlight($("#add-box"));
		$(".featherlight-close-icon").addClass("fui-cross");
		e.preventDefault();
		return false;
	});




});
