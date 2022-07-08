var datatable;

$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "15%" },
            { "data": "state", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "id",
                "render":function(data) {
                    return `
                    <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-primary">Edit</a>
                    <a onclick="Delete('/Admin/Company/Delete?id=${data}')" class="btn btn-danger">Delete</a>
                            `
                }
                
            }
            ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You want to Delete this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function(data) {
                    if (data.success) {
                        datatable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}