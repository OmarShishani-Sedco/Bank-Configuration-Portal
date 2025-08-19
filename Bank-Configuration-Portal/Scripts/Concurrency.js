document.addEventListener('DOMContentLoaded', function () {
    var dataEl = document.getElementById('concurrency-data');
    if (!dataEl) return;

    var id = parseInt(dataEl.dataset.id || '0', 10);
    var rowVersion = dataEl.dataset.rowversion || '';

    var modalEl = document.getElementById('concurrencyModal');
    if (id > 0 && rowVersion && modalEl) {
        // Bootstrap 5 modal
        var modal = new bootstrap.Modal(modalEl);

        var form = document.getElementById('forceDeleteForm');
        if (form) {
            var idInput = form.querySelector('#forceDeleteId');
            var rvInput = form.querySelector('#forceDeleteRowVersion');
            if (idInput) idInput.value = id;
            if (rvInput) rvInput.value = rowVersion;
            modal.show();
        }
    }
});
