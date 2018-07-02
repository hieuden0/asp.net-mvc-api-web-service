//$(".supplier-info").click(function() {
//    $(this).find(".supplier-edit-info").slideUp("400");
//    $(this).find(".account-icon").removeClass("fui-radio-checked").addClass("fui-radio-unchecked");
    
//    if ($(this).find(".supplier-edit-info").is(":hidden")) {
//        $(this).find(".supplier-edit-info").slideDown("400");
//        $(this).find(".account-icon").removeClass("fui-radio-unchecked").addClass("fui-radio-checked");

//    }
//    $(".supplier-info input, .supplier-info  select").click(function () {
//        return false;
//    });

//});


$(document).ready(function() {
    if ($(".supplier-info").length > 3) {
        $(".account-holder").height("750px");
    } else {
        $(".account-holder").height("750px");
    }

});