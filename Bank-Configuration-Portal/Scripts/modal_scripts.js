$(document).ready(function () {
    $('.modal').appendTo('body');

    $('#confirmDeleteModal').on('show.bs.modal', function (event) {
        const button = $(event.relatedTarget);
        const id = button.data('id');
        const rowVersionBase64 = button.data('rowversion');

        const deleteForm = $('#deleteForm');
        deleteForm.find('#deleteId').val(id);
        deleteForm.find('#deleteRowVersion').val(rowVersionBase64);
    });
});