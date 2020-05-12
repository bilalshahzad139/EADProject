﻿var MyApp = {};

MyApp = (function () {


    function Clear() {
        $("#txtProductID").val(0);
        $("#txtPictureName").val("");
        $("#txtName").val("");
        $("#txtPrice").val("");
        $("#selProdCategory").val(0);
        $("#prodimg").hide();
    }
    function SaveProduct() {

        var data = new FormData();

        var id = $("#txtProductID").val();
        var name = $("#txtName").val();
        var price = $("#txtPrice").val();
        var oldPicName = $("#txtPictureName").val();
        var category = $("#selProdCategory").val();
        alert(category);
        if (category == 0)
        {
            alert("No category is selected");
            return;
        }
        data.append("ProductID", id);
        data.append("Name", name);
        data.append("Price", price);
        data.append("PictureName", oldPicName);
        data.append("CategoryID", category);


        var files = $("#myfile").get(0).files;
        if (files.length > 0) {
            data.append("Image", files[0]);
        }

        var settings = {
            type: "POST",
            url: window.BasePath + 'Product2/Save',
            contentType: false,
            processData: false,
            data: data,
            success: function (r) {
                console.log(r);
                r.CreatedOn = moment(r.CreatedOn).format('DD/MM/YYYY HH:mm:ss');
                var obj = {};
                obj.data = [];
                obj.data.push({ ProductID: r.ProductID, Name: name, Price: price, PictureName: r.PictureName, CreatedOn: r.CreatedOn, CreatedBy: r.CreatedBy });

                var source = $("#listtemplate").html();
                var template = Handlebars.compile(source);

                var html = template(obj);

                if (id > 0) {
                    $("#tblBody tr[pid=" + id + "]").replaceWith(html);
                }
                else {
                    $("#tblBody").prepend(html);
                }

                BindEvents();

                Clear();

                alert("record is saved");
            },
            error: function () {
                alert('error has occurred');
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
            source = $("#dropdowntemplate").html();
            template = Handlebars.compile(source);
            html = template(resp);
            $("#selProdCategory").append(html);
        });
    }
    //function ProductCategoriesAJAXcall(id) {
    //    var data = { "id": id };

    //    //object pass to $.ajax function to make an AJAX call.

    //    var settings = {
    //        type: "GET",
    //        dataType: "json",
    //        url: window.BasePath + "Product2/GetAllCategories",
    //        data: data,
    //        success: function (resp) {
    //            //response.data contains whatever is sent from server

               

    //        },
    //        error: function (err, type, httpStatus) {
    //            alert('error has occured222');
    //        }
    //    };

    //    $.ajax(settings);

    //}
    function LoadProductsByCategory(categoryid) {
        var action = "Product2/GetProductsByCategory";
        $('#tblBody').empty();
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
                $("#tblBody").append(html);


                $("#tblBody .addcomment").click(function () {

                    var mainProdContainer = $(this).closest(".prodbox");
                    var pid = mainProdContainer.attr("pid");

                    var comment = $(this).closest(".commentarea").find(".txtComment").val();

                    var obj = {
                        ProductID: pid,
                        CommentText: comment
                    }


                    MyAppGlobal.MakeAjaxCall("POST", 'Product2/SaveComment', obj, function (resp) {

                        if (resp.success) {
                            alert("added");


                            var obj1 = {
                                PictureName: resp.PictureName,
                                UserName: resp.UserName,
                                CommentText: obj.CommentText,
                                CommentOn: moment(resp.CommentOn).format('DD/MM/YYYY HH:mm:ss')
                            };

                            var source = $("#commenttemplate").html();
                            var template = Handlebars.compile(source);

                            var html = template(obj1);
                            mainProdContainer.find(".comments").append(html);

                        }

                    });

                    return false;
                });

                BindEvents();

            }
                });

    }
    function LoadProducts(from, to) {

        
        var action = null;
        if (to == null && from == null) // in case of all products, range will be null.
            action = 'Product2/GetAllProducts';
        else {
            action = 'Product2/GetPriceRangedProducts?from=' + from + "&to=" + to;
            $('#tblBody').empty();  // remove previous products before refreshing product list.
        }
           
        MyAppGlobal.MakeAjaxCall("GET", action ,{}, function (resp) {

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
                $("#tblBody").append(html);


                $("#tblBody .addcomment").click(function () {

                    var mainProdContainer = $(this).closest(".prodbox");
                    var pid = mainProdContainer.attr("pid");

                    var comment = $(this).closest(".commentarea").find(".txtComment").val();

                    var obj = {
                        ProductID: pid,
                        CommentText: comment
                    }
                   

                    MyAppGlobal.MakeAjaxCall("POST", 'Product2/SaveComment', obj, function (resp) {

                        if (resp.success) {
                            alert("added");
                           

                            var obj1 = {
                                PictureName: resp.PictureName,
                                UserName: resp.UserName,
                                CommentText: obj.CommentText,
                                CommentOn: moment(resp.CommentOn).format('DD/MM/YYYY HH:mm:ss')
                            };

                            var source = $("#commenttemplate").html();
                            var template = Handlebars.compile(source);

                            var html = template(obj1);
                            mainProdContainer.find(".comments").append(html);
                            
                        }

                    });

                    return false;
                });

                BindEvents();

            }
        });
                       
    }


    function BindEvents() {

        $(".editprod").unbind("click").bind("click", function () {
            var $tr = $(this).closest("tr");
            var pid = $tr.attr("pid");

            var d = { "pid": pid };

            MyAppGlobal.MakeAjaxCall("GET", 'Product2/GetProductById', d, function (resp) {
                $("#txtProductID").val(resp.data.ProductID);
                $("#txtPictureName").val(resp.data.PictureName);
                $("#txtName").val(resp.data.Name);
                $("#txtPrice").val(resp.data.Price);
                $("#prodimg").show().attr("src", window.BasePath + "UploadedFiles/" + resp.data.PictureName);
                
            });

            return false;
        });

        $(".deleteprod").unbind("click").bind("click", function () {

            if (!confirm("Do you want to continue?")) {
                return;
            }
            var $tr = $(this).closest("tr");
            var pid = $tr.attr("pid");

            var d = { "pid": pid };

            MyAppGlobal.MakeAjaxCall("POST", 'Product2/DeleteProduct', d, function (resp) {
                
                $tr.remove();
            });
            

            return false;
        });

        $(".emailprod").unbind("click").bind("click", function () {
            var $tr = $(this).closest("tr");
            var pid = $tr.attr("pid");

            var d = { "pid": pid };

            MyAppGlobal.MakeAjaxCall("GET", 'Product2/GetProductById', d, function (resp) {

                $("#popupname").text(resp.data.Name);

                $("#overlay").show();

                $("#emailpopup").show();

            });

            return false;
        });
    }


    return {
        addCategory: function () {
            $("#btn_submit_Addcategory").click(function () {
                var categoryName = $("#categoryName").val();
                var data = new FormData();
                data.append("Cat_name", categoryName);
                var d= { "Cat_name": categoryName };
              
                if (categoryName == "") {
                    alert("empty");

                }
                else {
                    
                    var settings = {
                        type: "GET",
                                dataType: "json",
                        url: window.BasePath +'Product2/AddCategoryinDatabase',
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
        Main: function () {

            LoadProducts();
            loadProductCategories();

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
                var t = $(this).find(':selected').data('price');
                var a = t.split(':');
                var l = parseFloat(a[0]);
                var u = parseFloat(a[1]);
                // get lower and upper range and load products accordingly.
                LoadProducts(l, u);
            });
            $("#maindropdown").change(function () {
                var categoryid = $(this).val();
                if (categoryid == 0) {
                    LoadProducts(null, null);
                }
                else {
                    LoadProductsByCategory(categoryid);
                }

            });
        }
    };

})();