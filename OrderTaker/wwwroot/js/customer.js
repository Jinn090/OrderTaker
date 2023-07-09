function EditCustomer(customerID) {

    $('#form-customer').attr('action', 'api/customers/edit/' + customerID);
    $('#modal-customer').find('.modal-title').append('<i class="fa-solid fa-plus fa-fw"></i> Edit Customer');
    

    $.ajax({
        url: 'api/customers/edit/' + customerID,
        type: "GET",
        dataType: 'json'
    }).done(function (data) {
    
        $('#modal-customer').modal('show');
        $('#modal-customer').find('.modal-title').append('<i class="fa-solid fa-pen-to-square fa-fw"></i> Edit Customer');

        $('#ID').val(data.id);
        $('#FirstName').val(data.firstName);
        $('#LastName').val(data.lastName);
        $('#MobileNumber').val(data.mobileNumber);
        $('#IsActive').prop('checked', data.isActive);
        $('#City').val(data.city);

        $('#modal-customer').modal('show');
    }).fail(function (xhr, status, error) {
        Swal.fire(
            'Error!',
            'Can\'t find selected customer.',
            'error'
        )
    });
}

function clearInputs() {
    $('#modal-customer').modal('hide');
    $('#ID').val('');
    $('#FirstName').val('');
    $('#LastName').val('');
    $('#MobileNumber').val('');
    $('#City').val('');
    //$('#IsActive').val(0);
    $('#IsActive').prop('checked', false);
    //$('#IsActive').attr('checked', false);
}

$(() => {
    //add
    $('#create-customer').click(function (event) {
        event.preventDefault();
        clearInputs();
        $('#form-customer').attr('action', 'api/customers/create');
        $('#modal-customer').find('.modal-title').append('<i class="fa-solid fa-plus fa-fw"></i> Create New');
        //$('#modal-customer').modal('show');
    });

    let CUSTOMER_DT = $('#dt-customer').DataTable({
        proccessing: true,
        serverSide: true,
        ajax: {
            url: "/api/customers/get",
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: function (d) {
                console.log(d);
                return JSON.stringify(d);
            }
        },
        columns: [
            { data: "fullName", name: "FullName" },
            { data: "mobileNumber", name: "MobileNumber" },
            { data: "city", name: "City" },
            {
                data: "isActive",
                name: "IsActive",
            },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (data) {
                    return "<button type='button' class='btn btn-link' onclick=EditCustomer('" + data.id + "')>Edit</button>";
                }
            },
        ]
    });

    $("#form-customer").validate();
    $("#form-customer").on('submit', function (e) {
        var isvalid = $("#form-customer").valid();
        if (isvalid) {
            e.preventDefault();

            var form = $("#form-customer");
            var data = $(form).serialize();
            console.log(data);
            $.ajax({
                url: form.attr('action'),
                type: 'POST',
                data: data,
                dataType: 'json',
            })
            .done(function (response) {
                CUSTOMER_DT.ajax.reload(null, false);

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