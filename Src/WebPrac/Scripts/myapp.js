var MyApp = {};

MyApp = (function() {


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
        var name = $("#txtName").val().trim();
        var price = $("#txtPrice").val().trim();
        const oldPicName = $("#txtPictureName").val();
        var category = $("#selProdCategory").val();
        alert(category);
        if (category == 0) {
            alert("No category is selected");
            return;
        }
        var files = $("#myfile").get(0).files;

        if (name === "" || price === "") {
            $("#ErrMsg").text("Empty Fields!");
            setTimeout(() => {
                    const elem = $("#ErrMsg").text("");
                },
                3000);
            return false;
        }
        if (oldPicName === "" && files.length === 0) {
            $("#ErrMsg").text("Click on Choose File to upload Picture of Product!");
            setTimeout(() => {
                    const elem = $("#ErrMsg").text("");
                },
                3000);
            return false;
        }


        data.append("ProductID", id);
        data.append("Name", name);
        data.append("Price", price);
        data.append("PictureName", oldPicName);
        data.append("CategoryID", category);

        if (files.length > 0) {
            data.append("Image", files[0]);
        }

        const settings = {
            type: "POST",
            url: window.BasePath + "Product2/Save",
            contentType: false,
            processData: false,
            data: data,
            success: function(r) {
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
            error: function() {
                alert("error has occurred");

            }
        };

        $.ajax(settings);
    }

function loadProductCategories() {
        
        var action = "Product2/GetAllCategories"
        MyAppGlobal.MakeAjaxCall("GET", action, {}, function (resp) {

            var source = $("#dropdowntemplate").html();
            var template = Handlebars.compile(source);

            var html = template(resp);
            $("#maindropdown").append(html);
            $("#selProdCategory").append(html);
        });
    }
   
    function LoadProductsByCategory(categoryid) {
        var action = "Product2/GetProductsByCategory";
        $('#productsDiv').empty();
        MyAppGlobal.MakeAjaxCall("GET", action, {"id":categoryid}, function (resp) {

            if (resp.data) {

                for (var k in resp.data) {
                    var obj = resp.data[k];
                    obj.CreatedOn = moment(obj.CreatedOn).format('DD/MM/YYYY HH:mm:ss');

                    for (var k2 in obj.Comments) {
                        var comm = obj.Comments[k2];
                        comm.CommentOn = moment(comm.CommentOn).format('DD/MM/YYYY HH:mm:ss');
                    }
                }


                var source = $("#listtemplate").html();
                var template = Handlebars.compile(source);

                var html = template(resp);
                $("#productsDiv").append(html);


                //$("#productsDiv .addcomment").click(function () {

                //    var mainProdContainer = $(this).closest(".prodbox");
                //    var pid = mainProdContainer.attr("pid");

                //    var comment = $(this).closest(".commentarea").find(".txtComment").val();

                //    var obj = {
                //        ProductID: pid,
                //        CommentText: comment
                //    }


                //    MyAppGlobal.MakeAjaxCall("POST", 'Product2/SaveComment', obj, function (resp) {

                //        if (resp.success) {
                //            alert("added");


                //            var obj1 = {
                //                PictureName: resp.PictureName,
                //                UserName: resp.UserName,
                //                CommentText: obj.CommentText,
                //                CommentOn: moment(resp.CommentOn).format('DD/MM/YYYY HH:mm:ss')
                //            };

                //            var source = $("#commenttemplate").html();
                //            var template = Handlebars.compile(source);

                //            var html = template(obj1);
                //            mainProdContainer.find(".comments").append(html);

                //        }

                //    });

                //    return false;
                //});

                BindEvents();

            }
                });

    }
	
    function LoadProducts(from, to) {

        $("#productsDiv").empty();
        debugger;
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
            function(resp) {

                if (resp.data) {
                    debugger;
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

    function LoadProductsByName(prodName, minPrice, maxPrice, categoryId) {
        $("#productsDiv").empty();
        var action = "Product2/GetProductByName";
        const data = { productName: prodName, minPrice: minPrice, maxPrice: maxPrice, categoryId: categoryId };
        MyAppGlobal.MakeAjaxCall("Get",
            action,
            data,
            function (resp) {
                if (resp.data) {

                    if (resp.data.length == 0) {
                        alert("No Result Found.");
                    }
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
            function() {
                const $tr = $(this).closest("tr");
                const pid = $tr.attr("pid");

                const d = { "pid": pid };

                MyAppGlobal.MakeAjaxCall("GET",
                    "Product2/GetProductById",
                    d,
                    function(resp) {
                        $("#txtProductID").val(resp.data.ProductID);
                        $("#txtPictureName").val(resp.data.PictureName);
                        $("#txtName").val(resp.data.Name);
                        $("#txtPrice").val(resp.data.Price);
                        $("#prodimg").show().attr("src", window.BasePath + "UploadedFiles/" + resp.data.PictureName);

                    });

                return false;
            });

        $(".deleteprod").unbind("click").bind("click",
            function() {

                if (!confirm("Do you want to continue?")) {
                    return;
                }
                var $tr = $(this).closest("tr");
                const pid = $tr.attr("pid");

                const d = { "pid": pid };

                MyAppGlobal.MakeAjaxCall("POST",
                    "Product2/DeleteProduct",
                    d,
                    function(resp) {

                        $tr.remove();
                    });


                return false;
            });

        $(".emailprod").unbind("click").bind("click",
            function() {
                const $tr = $(this).closest("tr");
                const pid = $tr.attr("pid");

                const d = { "pid": pid };

                MyAppGlobal.MakeAjaxCall("GET",
                    "Product2/GetProductById",
                    d,
                    function(resp) {

                        $("#popupname").text(resp.data.Name);

                        $("#overlay").show();

                        $("#emailpopup").show();

                    });

                return false;
            });

        $("#productsDiv .addcomment").on("click", function() {

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
                function(resp1) {

                    if (resp1.success) {
                        alert("added");
                        debugger;
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

    function AutoCompleteHelper(selector, urlP) {

        let url = urlP.source;
        $(`${selector}`).on('propertychange input',
            function (event) {

            //debugger;
            const val = $(`${selector}`).val();
            $(`${selector}autocomplete-list`).empty();
            const data = {
                "val":val
            };

            const settings = {
                type: 'Post',
                dataType: "json",
                url: window.BasePath + url,
                data: data,
                success: function (resp) {
                    console.log(resp);
                    const inp = $(`${selector}`);
                    autocomplete(inp , resp);
                },
                error: function (error) {
                   console.log(error);
                }
            };

            $.ajax(settings);

            }
        );


        function autocomplete(inp ,arr) {
            var a, b, i;
            closeAllLists();
            var currentFocus = -1;
            a = $("<div>", { "id": inp.attr("id") + "autocomplete-list", "class": "autocomplete-items" }).css({
                "z-index":"99"
            });
            inp.parent().append(a);
            for (i = 0; i < arr.length; i++) {
                b = $("<div>");
                b.html(`<strong>${arr[i].substr(0, inp[0].value.length)}</strong>`);
                b.html(b.html() + arr[i].substr(inp[0].value.length));
                b.html(`${b.html()}<input type='hidden' value='${arr[i].trim()}'>`);
                
                b.on("click", function (e) {
                    
                    inp.val($(this).children("input").val());
                    closeAllLists();
                });
                b.css({
                    "padding":"5px 8px"
                })
                a.width(inp.width()+10);
                a.css({
                    "margin":"-2px"
                })
                a.append(b);
            }

            inp.keydown(function (e) {
                
                var x = $(`#${$(this).attr("id")}autocomplete-list`);
                
                if (x) x = $(x).children("div");
                if (e.keyCode === 40) { //down
                    currentFocus++;
                    addActive(x);
                } else if (e.keyCode === 38) { //up
                    currentFocus--;
                    addActive(x);
                } else if (e.keyCode === 13) {
                    //enter
                    e.preventDefault();
                    if (currentFocus > -1) {
                        if (x) x[currentFocus].click();
                    }
                } else {
                    return true;
                }
            });

            //Adds active class to current item of list
            function addActive(x) {
                /*a function to classify an item as "active":*/
                if (!x) return false;
                /*start by removing the "active" class on all items:*/
                removeActive(x);
                if (currentFocus >= x.length) currentFocus = 0;
                if (currentFocus < 0) currentFocus = (x.length - 1);
                /*add class "autocomplete-active":*/
                x[currentFocus].classList.add("autocomplete-active");
            }

        
            /*Removes active class from any active item*/
            function removeActive(x) {
                /*a function to remove the "active" class from all autocomplete items:*/
                for (let item = 0; item < x.length; item++) {
                    
                    $($(x)[item]).removeClass("autocomplete-active");
                }
            }

            // For closing already opened lists
            document.addEventListener("click", function (e) {
                closeAllLists(e.target);
            });
        }

        function closeAllLists(elmnt) {
            let inp = $(`${selector}`).parent();
            inp = inp[0];
            /*close all autocomplete lists in the document,
            except the one passed as an argument:*/
            var x = document.getElementsByClassName("autocomplete-items");
            for (var i = 0; i < x.length; i++) {
                if (elmnt != x[i] && elmnt != inp) {
                    x[i].parentNode.removeChild(x[i]);
                }
            }
        }

    }
 
    function SignupHelper() {
        var fileName="";

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
                    //client side validation of Email address
                    if (!validateEmail(login)) {
                        $("#p").text("You have entered an invalid email address!!");
                        setTimeout(() => {
                                const elem = $("#p").text("");
                            },
                            2000);
                        return false;
                    }
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
                                alert("An Email is send to you for verification of your Email address.");
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
    function validateEmail(email) {
        var mailFormat = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return mailFormat.test(email);
    }
    return {
		 addCategory: function () {
            $("#btn_submit_Addcategory").click(function () {
                var categoryName = $("#categoryName").val();
                var data = new FormData();
                data.append("Cat_name", categoryName);
                var d = { "Cat_name": categoryName };

                if (categoryName == "") {
                    alert("empty");

                }
                else {

                    var settings = {
                        type: "GET",
                        dataType: "json",
                        url: window.BasePath + 'Product2/AddCategoryinDatabase',
                        data: d,
                        success: function (resp) {
                            //response.data contains whatever is sent from server

                            alert("suceess")

                        },
                        error: function (err, type, httpStatus) {
                            alert('error has occured222');
                        }
                    }

                    $.ajax(settings);
                }

            });
        },

        Signup: function () {
            SignupHelper();
        },
        Main: function () {

            LoadProducts();
            loadProductCategories();
            $("#btnSave").click(function() {

                SaveProduct();
                return false;
            });

            $("#btnClear").click(function() {

                Clear();
                return false;
            });

            $("#btnSend").click(function() {
                //Call send email function
                $("#emailpopup").hide();
                $("#overlay").hide();
                return false;
            });

            $("#btnClose").click(function() {
                $("#emailpopup").hide();
                $("#overlay").hide();
                return false;
            });

            $("#priceDropDown").change(function() {
                const t = $(this).find(":selected").data("price");
                const a = t.split(":");
                const l = parseFloat(a[0]);
                const u = parseFloat(a[1]);
                // get lower and upper range and load products accordingly.
                LoadProducts(l, u);
            });
            $("#maindropdown").change(function () {
                var categoryid = $(this).val();
                if (categoryid == 0) {
                    LoadProducts();
                }
                else {
                    LoadProductsByCategory(categoryid);
                }

            });
            $("#newProdBtn").click(function() {
                $("#addNewProd").slideToggle(700);
            });

            $("#btnSearchProduct").click(function () {
                var prodName = $("#txtProductName").val();
                if (prodName == "") {
                    return;
                }
                const t = $("#priceDropDown").find(":selected").data("price");
                const a = t.split(":");
                const l = parseFloat(a[0]);
                const u = parseFloat(a[1]);
                const category = 0;
                //category = $("#maindropdown").val();
                LoadProductsByName(prodName, l, u, category);


            });
        },
        AutoComplete: function(selector, data) {
            AutoCompleteHelper(selector, data);
        }

    };

})();