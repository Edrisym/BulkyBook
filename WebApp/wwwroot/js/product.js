var dataTable;

$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    dataTable = $('#tabledata').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "10%" },
            { "data": "price", "width": "5%" },
            { "data": "author", "width": "15%" },
            { "data": "category.name", "width": "8%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                     <div class=" btn-group" role="group">
                     <a class="btn btn-primary" href="/Admin/Product/Upsert?id=${data}" >
                        <i class="bi bi-pencil-square"></i>&nbsp;Edit</a></div>

                        <div class=" btn-group" role="group">
                        <a onClick=Delete('/Admin/Product/Delete/${data}')
                        class="btn btn-danger" ><i class="bi bi-trash3-fill"></i>&nbsp;Delete</a></div> `
                },

                "width": "17%"
            }


        ]
    });
}


function Delete(url) {
    Swal.fire({
        title: 'Are you sure?!',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#B90E0A',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}
