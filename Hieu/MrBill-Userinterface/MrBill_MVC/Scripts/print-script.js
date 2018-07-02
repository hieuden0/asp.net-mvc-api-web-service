
$(document).ready(function () {
	if ($(this).parents(".month-header").hasClass("disabled-item")) {
		        return false;
		    }
	$(".print").click(function (e) {
		window.print();
		return false;
	});



})