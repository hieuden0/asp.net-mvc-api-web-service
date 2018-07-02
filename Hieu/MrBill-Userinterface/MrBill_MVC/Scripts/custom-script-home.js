$(document).ready(function () {

    $('.btn-dwn').click(function () {
        $('html,body').animate({ scrollTop: $(".page2").height()+250 }, 'slow');
    });
});


//$(document).ready(function () {
//   var images = InitializeSliderImages();
//    initializeSliderFunctions(images);


//});

//function InitializeSliderImages() {

//    var imageArray = ["Content/img/Alcatraz.jpg", "", "", ""];
//    return imageArray;
//}

//function initializeSliderFunctions(images) {

//    $("#next").click(function () {
       
//        $("body").css("background", "url("+ images[1]+")");
//    });

//    $("#prev").click(function () {
//        $("body").css("background", "url(www)");
//    });
//}