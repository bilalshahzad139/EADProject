var MyApp = {};

MyApp = (function () {


    function Clear() {
        $("#txtProductID").val(0);
        $("#txtPictureName").val("");
        $("#txtName").val("");
        $("#txtPrice").val("");
        $("#prodimg").hide();
    }

    function SaveProduct() {

        const data = new FormData();

        var id = $("#txtProductID").val();
        var name = $("#txtName").val();
        var price = $("#txtPrice").val();
        const oldPicName = $("#txtPictureName").val();

        data.append("ProductID", id);
        data.append("Name", name);
        data.append("Price", price);
        data.append("PictureName", oldPicName);



        var files = $("#myfile").get(0).files;

        if (files.length > 0) {
            data.append("Image", files[0]);
        }

        const settings = {
            type: "POST",
            url: window.BasePath + "Product2/Save",
            contentType: false,
            processData: false,
            data: data,
            success: function (r) {
                console.log(r);

                const obj = {};
                obj.data = [];
                obj.data.push({ ProductID: r.ProductID, Name: name, Price: price, PictureName: r.PictureName });

                const source = $("#listtemplate").html();
                const template = Handlebars.compile(source);

                const html = template(obj);


                if (id > 0) {
                    $(`#productsDiv tr[pid=${id}]`).replaceWith(html);
                } else {
                    $("#productsDiv").prepend(html);
                }

                BindEvents();

                Clear();

                alert("record is saved");
            },
            error: function () {
                alert("error has occurred");

            }
        };

        $.ajax(settings);
    }

    function LoadProducts(from, to) {

        $("#productsDiv").empty();
        //debugger;
        var action = null;
        if (to == null && from == null) // in case of all products, range will be null.
            action = "Product2/GetAllProducts";
        else {
            action = `Product2/GetPriceRangedProducts?from=${from}&to=${to}`;
            $("#productsDiv").empty(); // remove previous products before refreshing product list.
        }

        MyAppGlobal.MakeAjaxCall("GET",
            action,
            {},
            function (resp) {

                if (resp.data) {
                    //debugger;
                    for (let k in resp.data) {
                        const obj = resp.data[k];
                        obj.CreatedOn = moment(obj.CreatedOn).format("DD/MM/YYYY HH:mm:ss");

                        for (let k2 in obj.Comments) {
                            const comm = obj.Comments[k2];
                            comm.CommentOn = moment(comm.CommentOn).format("DD/MM/YYYY HH:mm:ss");
                        }
                    }


                    const source = $("#listtemplate").html();
                    const template = Handlebars.compile(source);

                    const html = template(resp);
                    $("#productsDiv").append(html);




                    BindEvents();

                }
            });

    }

    function BindEvents() {

        $(".editprod").unbind("click").bind("click",
            function () {
                const $tr = $(this).closest("tr");
                const pid = $tr.attr("pid");

                const d = { "pid": pid };

                MyAppGlobal.MakeAjaxCall("GET",
                    "Product2/GetProductById",
                    d,
                    function (resp) {
                        $("#txtProductID").val(resp.data.ProductID);
                        $("#txtPictureName").val(resp.data.PictureName);
                        $("#txtName").val(resp.data.Name);
                        $("#txtPrice").val(resp.data.Price);
                        $("#prodimg").show().attr("src", window.BasePath + "UploadedFiles/" + resp.data.PictureName);

                    });

                return false;
            });

        $(".deleteprod").unbind("click").bind("click",
            function () {

                if (!confirm("Do you want to continue?")) {
                    return;
                }
                var $tr = $(this).closest("tr");
                const pid = $tr.attr("pid");

                const d = { "pid": pid };

                MyAppGlobal.MakeAjaxCall("POST",
                    "Product2/DeleteProduct",
                    d,
                    function (resp) {

                        $tr.remove();
                    });


                return false;
            });

        $(".emailprod").unbind("click").bind("click",
            function () {
                const $tr = $(this).closest("tr");
                const pid = $tr.attr("pid");

                const d = { "pid": pid };

                MyAppGlobal.MakeAjaxCall("GET",
                    "Product2/GetProductById",
                    d,
                    function (resp) {

                        $("#popupname").text(resp.data.Name);

                        $("#overlay").show();

                        $("#emailpopup").show();

                    });

                return false;
            });
        $(".addToWishlist").click(function () {
            var mainProdContainer = $(this).closest(".product");
            var pid = mainProdContainer.attr("pid");
            var object = {
                pid: pid
            }
            MyAppGlobal.MakeAjaxCall("POST", 'Product2/AddToWishlist', object, function (resp) {
                if (resp.data == 0) {
                    alert("Product already in Wishlist");
                }
                else {
                    alert("Product added to Wishlist");
                }

            });

        });

        $("#productsDiv .addcomment").on("click", function () {

            var mainProdContainer = $(this).closest(".product");
            console.log(mainProdContainer);
            const pid = mainProdContainer.attr("pid");

            const comment = $(this).closest(".commentarea").find(".txtComment").val();

            var obj1 = {
                ProductID: pid,
                CommentText: comment
            };


            MyAppGlobal.MakeAjaxCall("POST",
                "Product2/SaveComment",
                obj1,
                function (resp1) {

                    if (resp1.success) {
                        alert("added");
                        //debugger;
                        console.log(resp1);

                        const obj11 = {
                            PictureName: resp1.PictureName,
                            UserName: resp1.UserName,
                            CommentText: obj1.CommentText,
                            CommentOn: moment(resp1.CommentOn)
                                .format("DD/MM/YYYY HH:mm:ss")
                        };

                        const source1 = $("#commenttemplate").html();
                        const template1 = Handlebars.compile(source1);
                        const html1 = template1(obj11);
                        mainProdContainer.find(".comments").append(html1);

                    }

                });
            

            $(this).closest(".commentarea").find(".txtComment").val("");

            return false;
        });
    }


    function SignupHelper() {
        var fileName = "";

        $("#btnSignUp").on("click",
            function () {
                var data = new FormData();
                // getting picture name
                var files = $("#uploadImage").get(0).files;
                if (files.length > 0) {
                    data.append("myProfilePic", files[0]);
                    fileName = files[0].name;
                }
                let name = $("#username").val().trim();
                let login = $("#login").val().trim();
                let password = $("#password").val().trim();
                let cpassword = $("#cpassword").val().trim();
                if (login !== "" && password !== "" && name !== "" && cpassword !== "") {
                    if (password !== cpassword) {
                        $("#cpassword").val("");
                        $("#password").val("");
                        $("#p").text("Password not matched!");
                        setTimeout(() => {
                            const elem = $("#p").text("");
                        },
                            2000);
                        return false;
                    }

                    if (fileName === "") {
                        $("#p").text("Click on avatar to upload picture!");
                        setTimeout(() => {
                            const elem = $("#p").text("");
                        },
                            2000);
                        return false;
                    }
                    data.append("Name", name);
                    data.append("Login", login);
                    data.append("Password", password);
                    data.append("PictureName", fileName);

                    var settings = {
                        type: "POST",
                        url: window.BasePath + "User/Signup",
                        contentType: false,
                        processData: false,
                        data: data,
                        success: function (response) {

                            if (response.isUserExist) {
                                $("#password").val("");
                                $("#cpassword").val("");
                                $("#p").text("User already exists!");
                                setTimeout(() => {
                                    const elem = $("#p").text("");
                                }, 2000);
                                return false;
                            }
                            else {
                                window.location.href = window.BasePath + "User/Login";
                            }
                        },
                        error: function (error) {
                            console.log(error);
                        }
                    };

                    $.ajax(settings);
                }
                else {

                    $("#p").text("Empty Fields!");
                    setTimeout(() => {
                        const elem = $("#p").text("");
                    }, 2000);
                    return false;
                }

            });
    }

    return {

        Signup: function () {
            SignupHelper();
        },
        Main: function () {

            LoadProducts();

            $("#btnSave").click(function () {

                SaveProduct();
                return false;
            });

            $("#btnClear").click(function () {

                Clear();
                return false;
            });

            $("#btnSend").click(function () {
                //Call send email function
                $("#emailpopup").hide();
                $("#overlay").hide();
                return false;
            });

            $("#btnClose").click(function () {
                $("#emailpopup").hide();
                $("#overlay").hide();
                return false;
            });

            $("#priceDropDown").change(function () {
                const t = $(this).find(":selected").data("price");
                const a = t.split(":");
                const l = parseFloat(a[0]);
                const u = parseFloat(a[1]);
                // get lower and upper range and load products accordingly.
                LoadProducts(l, u);
            });

            $("#newProdBtn").click(function () {
                $("#addNewProd").slideToggle(700);
            });
        }
    };

})();
