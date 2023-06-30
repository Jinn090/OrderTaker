$(() => {
    
    const PRICE_AUTO_NUMERIC = new AutoNumeric(
        '#PurchaseItem_Price',
        {
            leadingZero: 'deny',
            emptyInputBehavior: 'zero',
            modifyValueOnWheel: false
        });

    const PRICE_DETAILS_AUTO_NUMERIC = new AutoNumeric(
        '#form-order-item-details #PurchaseItem_Price',
        {
            leadingZero: 'deny',
            emptyInputBehavior: 'zero',
            modifyValueOnWheel: false
        });
    const UNIT_PRICE_AUTO_NUMERIC = new AutoNumeric(
        '#form-order-item-details #PurchaseItem_SKU_UnitPrice',
        {
            leadingZero: 'deny',
            emptyInputBehavior: 'zero',
            modifyValueOnWheel: false
        });
    
    $("#add-sku").click(function (event) {
        //default
        if (SKU_LIST_DT.row(0).data() != null) {
            SKU_LIST_DT.row(':eq(0)').select();
            SKU_OBJ.id = SKU_LIST_DT.row(0).data().id;
            SKU_OBJ.name = SKU_LIST_DT.row(0).data().name
            UpdatePurchaseItemPrice(
                SKU_LIST_DT.row({ selected: true }).data().unitPrice,
                $('#form-order-item #PurchaseItem_Quantity').val());
        } else {
            PRICE_AUTO_NUMERIC.set(0);
        }

        $('#modal-order-item').modal('show');
        $('#modal-order-item').find('.modal-title').append('<i class="fa-solid fa-plus fa-fw"></i> SKU List');
    });

    let SKU_DT = $('#dt-sku').DataTable({
        ajax: {
            url: "/api/orders/skus/gePurchasedItems",
            dataSrc: '',
            type: "POST",
            datatype: "json",
            data(d) {
                d.id = $('#PurchaseOrder_ID').val();
            },
        },
        rowId: 'id',
        "columnDefs": [
            {
                targets: [0,1],
                visible: false,
                searchable: false
            },
            {
                targets: [3],
                className: 'text-right pr-4',
            }
        ],
        columns: [
            { data: "skuId", name: "SKUID" },
            { data: "unitPrice", name: "UnitPrice" },
            { data: "name", name: "Name" },
            { data: "quantity", name: "Quantity" },
            {
                data: "price",
                name: "Price",
                render: $.fn.dataTable.render.number(',', '.', 2),
            },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (data) {
                    return "<button type='button' class='btn btn-link item-edit' >Edit</button>";
                }
            },
        ],
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                return typeof i === 'string' ? i.replace(/[\$,]/g, '') * 1 : typeof i === 'number' ? i : 0;
            };

            // Total over all pages
            total = api
                .column(4)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            var formattedTotal = $.fn.dataTable.render.number(',', '.', 2).display(Number.parseFloat(total).toFixed(2));
            // Update footer
            $(api.column(4).footer()).html(formattedTotal);
        }
    });

    SKU_DT.on('click', '.item-edit', function (event) {
        event.preventDefault();
        let rowData = SKU_DT.row($(this).parents('tr')).data();
        SKU_OBJ.id = rowData.id;
        SKU_OBJ.name = rowData.name;
        //console.log(rowData);

        $('#form-order-item-details #PurchaseItem_Quantity').val(rowData.quantity);
        UNIT_PRICE_AUTO_NUMERIC.set(rowData.unitPrice);
        PRICE_DETAILS_AUTO_NUMERIC.set(rowData.price);

        PRICE_AUTO_NUMERIC.set(rowData.price);
        UpdatePurchaseItemPrice(rowData.unitPrice, rowData.quantity);

        $('#modal-order-item-details').find('.modal-title')
            .append('<i class="fa-solid fa-plus fa-fw"></i>'+ rowData.name);
        $('#modal-order-item-details').modal("show");

    });

    $("#form-sku").on('submit', function (e) {
        e.preventDefault();

        var form = $("#form-sku");
        var data = $(form).serialize() + '&' + $.param({ PurchaseItems: SKU_DT.rows().data().toArray() });
        /*console.log(form.attr('action'));*/
        if (form.attr('action').includes('Create')) {
            //create ajax call
            $.ajax({
                url: form.attr('action'),
                type: 'POST',
                data: data,
            })
                .done(function (response) {
                    window.location.href = response.redirectToUrl;
                })
                .fail(function (xhr, status, error) {
                    Swal.fire('Error!', 'Something went wrong.', 'error')
                });
        }
        else //edit ajax call
        {
            $.ajax({
                url: form.attr('action'),
                type: 'POST',
                data: data,
            })
                .done(function (response) {
                    if ($('#PurchaseOrder_Status').val() == 'Completed') {
                        window.location.href = "/Orders/Index";
                    }
                    else
                    {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success!',
                            text: 'Customer order updated.',
                            showConfirmButton: false,
                            timer: 3000
                        })
                    }
                    
                })
                .fail(function (xhr, status, error) {
                    Swal.fire('Error!', 'Something went wrong.', 'error')
                });
        }

    });

    let SKU_LIST_DT = $('#dt-sku-list').DataTable({
        select: {
            style: 'single'
        },
        rowId: 'id',
        ajax: {
            url: "/api/orders/skus/get",
            dataSrc: '',
            type: "POST",
            datatype: "json",
            data(d) {
                d.SKUs = JSON.stringify(SKU_DT.rows().data().toArray())
            },
        },
        columns: [
            { data: "name", name: "Name" },
            { data: "code", name: "Code" },
            {
                data: "unitPrice",
                name: "unitPrice",
                render: $.fn.dataTable.render.number(',', '.', 2),
            },
        ]
    });

    let SKU_OBJ = {
        id: null,
        unitPrice: null,
        name: null,
        quantity: null,
        price: null,
    };

    $('#dt-sku-list tbody').on('click', 'tr', function () {
        SKU_OBJ.id = SKU_LIST_DT.row(this).data().id;
        SKU_OBJ.name = SKU_LIST_DT.row(this).data().name;

        UpdatePurchaseItemPrice(SKU_LIST_DT.row(this).data().unitPrice);
    });

    $('#PurchaseItem_Quantity').on('change', () => {
        //console.log($('#PurchaseItem_Quantity').val());
        UpdatePurchaseItemPrice(
            SKU_LIST_DT.row({ selected: true }).data().unitPrice,
            $('#form-order-item #PurchaseItem_Quantity').val());
    });

    $('#form-order-item-details #PurchaseItem_Quantity').on('change', () => {
        console.log("#form-order-item-details #PurchaseItem_Quantity on change");
        UpdatePurchaseItemPrice(
            UNIT_PRICE_AUTO_NUMERIC.rawValue,
            $('#form-order-item-details #PurchaseItem_Quantity').val());
    });

    function UpdatePurchaseItemPrice(unitPrice, qty) {
        PRICE_AUTO_NUMERIC.set(unitPrice * $('#form-order-item #PurchaseItem_Quantity').val());
        PRICE_DETAILS_AUTO_NUMERIC.set(unitPrice * qty);
        SKU_OBJ.unitPrice = unitPrice;
        SKU_OBJ.quantity = qty;
        SKU_OBJ.price = PRICE_AUTO_NUMERIC.rawValue;
    }

    $("#form-order-item").validate();
    $("#form-order-item").on('submit', function (e) {
        var isvalid = $("#form-order-item").valid();
        if (isvalid) {
            e.preventDefault();
            console.log(SKU_OBJ);
            SKU_DT.row.add({
                id: null,
                unitPrice: SKU_OBJ.unitPrice,
                skuId: SKU_OBJ.id,
                name: SKU_OBJ.name,
                quantity: SKU_OBJ.quantity,
                price: SKU_OBJ.price
            }).draw(false);

            if (SKU_LIST_DT.rows().data().toArray() == null) {
                PRICE_AUTO_NUMERIC.set(0);
            }

            $("#modal-order-item").modal("hide");
        }
    });

    $("#form-order-item-details").validate();
    $("#form-order-item-details").on('submit', function (e) {
        var isvalid = $("#form-order-item-details").valid();
        if (isvalid) {
            e.preventDefault();

            var row = SKU_DT.row('#' + SKU_OBJ.id);
            var data = row.data();

            console.log(data);

            data.quantity = SKU_OBJ.quantity;
            data.price = SKU_OBJ.price;

            row.data(data).draw(false);

            $("#modal-order-item-details").modal("hide");
        }
    });

 

    $("#modal-order-item").on('show.bs.modal', function (e) {
        SKU_DT.quantity = $('#form-order-item #PurchaseItem_Quantity').val();
        SKU_LIST_DT.ajax.reload();
    })

});