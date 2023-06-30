$(() => {
    const PRICE_AUTO_NUMERIC = new AutoNumeric(
        '#PurchaseItem_Price',
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
            UpdatePurchaseItemPrice(SKU_LIST_DT.row({ selected: true }).data().unitPrice);
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
                targets: [0],
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
                    return "<button type='button' class='btn btn-link' onclick=EditOrder('" + data.id + "')>Edit</button>";
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
                .column(3)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            // Total over this page
            pageTotal = api
                .column(3, { page: 'applied' })
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            var formattedTotal = $.fn.dataTable.render.number(',', '.', 2).display(Number.parseFloat(total).toFixed(2));
            // Update footer
            $(api.column(3).footer()).html(formattedTotal);
        }
    });

    $("#form-sku").on('submit', function (e) {
        e.preventDefault();

        var form = $("#form-sku");
        var data = $(form).serialize() + '&' + $.param({ PurchaseItems: SKU_DT.rows().data().toArray() });
        /*console.log(form.attr('action'));*/
        if (form.attr('action').includes('create')) {
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
        UpdatePurchaseItemPrice(SKU_LIST_DT.row({ selected: true }).data().unitPrice);
    });

    function UpdatePurchaseItemPrice(unitPrice) {
        PRICE_AUTO_NUMERIC.set(unitPrice * $('#PurchaseItem_Quantity').val());
        SKU_OBJ.quantity = $('#PurchaseItem_Quantity').val();
        SKU_OBJ.price = PRICE_AUTO_NUMERIC.rawValue;
    }

    $("#form-order-item").validate();
    $("#form-order-item").on('submit', function (e) {
        var isvalid = $("#form-order-item").valid();
        if (isvalid) {
            e.preventDefault();

            SKU_DT.row.add({
                id: null,
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

    $("#modal-order-item").on('show.bs.modal', function (e) {
        SKU_LIST_DT.ajax.reload();
    })

});