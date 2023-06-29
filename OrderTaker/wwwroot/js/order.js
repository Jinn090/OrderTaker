function EditOrder(orderID) {
    window.location.href = "/Orders/Edit/" + orderID;
}

$(() => {
    let SKU_DT = $('#dt-order').DataTable({
        ajax: {
            url: "/api/orders/get",
            dataSrc: '',
            type: "POST",
            datatype: "json"
        },
        columns: [
            { data: "customer.fullName", name: "CustomerFullName" },
            {
                data: "dateOfDelivery",
                name: "DateOfDelivery",
                render: function (data, type) {
                    return moment.utc(data).local().format('DD/MM/YYYY');
                },
            },
            {
                data: "status", name: "Status",
            },
            {
                data: "amountDue",
                name: "AmountDue",
                render: $.fn.dataTable.render.number(',', '.', 2),
            },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (data) {
                    if (data.status == "Completed") {
                        return "<button type='button' class='btn btn-link' disabled>Edit</button>";
                    } else {
                        return "<button asp-action='Edit' type='button' class='btn btn-link' onclick=EditOrder('" + data.id + "')>Edit</button>";
                    }
                    
                }
            },
        ]
    });
});