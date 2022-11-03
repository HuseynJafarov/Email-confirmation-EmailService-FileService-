$(function () {

    let scrollSection = document.getElementById("scrol-area")

    window.onscroll = function () { scrollFunction() };

    function scrollFunction() {
        if (document.body.scrollTop > 150 || document.documentElement.scrollTop > 150) {
            scrollSection.style.top = "0";
        } else {
            scrollSection.style.top = "-77px";
            scrollSection.classList.remove("visibl");
        }
    }







           

    $(document).on("click", ".show-more button", function () {
        
        let parent = $("#parent-products")

        let skipCount = $("#parent-products").children().length;


        let productCount = $("#product-count").val();

       
        $.ajax({
            /* url: `/accessories/loadmore?skip=${skipCount}`,*/
            url: "/accessories/loadmore",
            type: "Get",
            data: {
                skip: skipCount
            },

            success: function (res) {

                $(parent).append(res);

                skipCount = $("#parent-products").children().length;

                if (skipCount >= productCount) {

                    $(".show-more button").addClass("d-none")

                }
            }
        })
    });




});