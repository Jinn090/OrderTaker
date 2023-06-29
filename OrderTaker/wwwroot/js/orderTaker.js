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
        SKU_LIST_DT.row(':eq(0)').select();
        SKU_OBJ.id = SKU_LIST_DT.row(0).data().id;
        SKU_OBJ.name = SKU_LIST_DT.row(0).data().name
        UpdatePurchaseItemPrice(SKU_LIST_DT.row({ selected: true }).data().unitPrice);

        $('#modal-order-item').modal('show');
        $('#modal-order-item').find('.modal-title').append('<i class="fa-solid fa-plus fa-fw"></i> SKU List');
    });

    let SKU_DT = $('#dt-sku').DataTable({
        ajax: {
            url: "/api/orders/skus/gePurchasedItems",
            dataSrc: '',
            type: "POST",
            datatype: "json"
        },
        rowId: 'id',
        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
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
        ]
    });

    $("#form-sku").on('submit', function (e) {
        e.preventDefault();

        var form = $("#form-sku");
        var data = $(form).serialize() + '&' + $.param({ PurchaseItems: SKU_DT.rows().data().toArray() });
        console.log(data);
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
            datatype: "json"
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

            var form = $("#form-order-item");
            var data = $(form).serialize();

            var filteredData = SKU_DT
                .rows()
                .data()
                .toArray();

            var result = filteredData.find(e => e.id === SKU_OBJ.id);
            if (result == null) {
                SKU_DT.row.add({
                    id: null,
                    skuId: SKU_OBJ.id,
                    name: SKU_OBJ.name,
                    quantity: SKU_OBJ.quantity,
                    price: SKU_OBJ.price
                }).draw(false);
            } else {

                var row = SKU_DT.row('#' + SKU_OBJ.id);
                var data = row.data();
    
                data.quantity = SKU_OBJ.quantity;
                data.price = SKU_OBJ.price;

                row.data(data).draw(false);
            }

            $("#modal-order-item").modal("hide");
        }
    });

   
});