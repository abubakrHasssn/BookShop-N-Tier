﻿var datatable;

$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "category.name", "width": "15%" },
            {
                "data": "id",
                "render":function(data) {
                    return `
                    <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-primary">Edit</a>
                    <a onclick="Delete('/Admin/Product/Delete?id=${data}')" class="btn btn-danger">Delete</a>
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