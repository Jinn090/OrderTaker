function EditSKU(skuID) {
    $('#form-sku').attr('action', 'api/skus/edit/' + skuID);
    $('#modal-sku').find('.modal-title').append('<i class="fa-solid fa-plus fa-fw"></i> Edit Customer');


    $.ajax({
        url: 'api/skus/edit/' + skuID,
        type: "GET",
        dataType: 'json'
    }).done(function (data) {

        $('#modal-sku').modal('show');
        $('#modal-sku').find('.modal-title').append('<i class="fa-solid fa-pen-to-square fa-fw"></i> Edit SKU');

        $('#ID').val(data.id);
        $('#Name').val(data.name);
        $('#Code').val(data.code);
        $('#UnitPrice').val(data.unitPrice);
        $('#IsActive').prop('checked', data.isActive);
        $('#Image').val(data.image);

        $('#modal-sku').modal('show');
    }).fail(function (xhr, status, error) {
        console.log(error);
        Swal.fire(
            'Error!',
            'Can\'t find selected sku.',
            'error'
        )
    });
}

function clearInputs() {
    $('#modal-sku').modal('hide');
    $('#ID').val('');
    $('#Name').val('');
    $('#Code').val('');
    $('#UnitPrice').val('');
    $('#IsActive').prop('checked', false);
}

$(() => {
    $('#create-sku').click(function (event) {
        event.preventDefault();
        clearInputs();
        $('#form-sku').attr('action', 'api/skus/create');
        $('#modal-sku').find('.modal-title').append('<i class="fa-solid fa-plus fa-fw"></i> Create New');
    });

    let SKU_DT = $('#dt-sku').DataTable({
        ajax: {
            url: "/api/skus/get",
            dataSrc: '',
            type: "POST",
            datatype: "json"
        },
        columns: [
            { data: "name", name: "Name" },
            { data: "code", name: "Code" },
            {
                data: "unitPrice",
                name: "UnitPrice",
                render: $.fn.dataTable.render.number(',', '.', 2),
            },
            { data: "isActive", name: "IsActive",
            },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (data) {
                    return "<button type='button' class='btn btn-link' onclick=EditSKU('" + data.id + "')>Edit</button>";
                }
            },
        ]
    });

    $("#form-sku").validate();
    $("#form-sku").on('submit', function (e) {
        var isvalid = $("#form-sku").valid();
        if (isvalid) {
            e.preventDefault();

            var form = $("#form-sku");
            var data = $(form).serialize();

            $.ajax({
                url: form.attr('action'),
                type: 'POST',
                data: data,
                dataType: 'json',
            })
            .done(function (response) {
                SKU_DT.ajax.reload(null, false);
                clearInputs();
    
                if (response.code == 409) {
                    Swal.fire('Error!', response.message, 'error')
                } else {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success!',
                        text: 'Success',
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

});