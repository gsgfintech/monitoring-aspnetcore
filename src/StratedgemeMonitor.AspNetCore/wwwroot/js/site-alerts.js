function showBootstrapAlert(level, subject) {
    $('#alertholder').after(
        '<div class="alert alert-' + level +' alert-dismissable">' +
            '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
                '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            subject +
        '</div>');
}

function closeAlert(id) {
    $.ajax({
        type: 'POST',
        url: '/Alerts/CloseAlert',
        data: { id: id },
        success: function (data) {
            console.log(data);
            if (data.success) {
                showBootstrapAlert('success', data.message || 'Successfully closed alert ' + id);
                $('#openalerts').load('/Alerts/RefreshOpenAlerts');
                $('#closedalerts').load('/Alerts/RefreshClosedAlerts');
            }
            else {
                showBootstrapAlert('danger', data.message || 'Failed to close alert ' + id);
            }
        }
    });
}

function closeAllAlerts() {
    $.ajax({
        type: 'POST',
        url: '/Alerts/CloseAllAlerts',
        data: null,
        success: function (data) {
            console.log(data);
            if (data.success) {
                showBootstrapAlert('success', 'Successfully closed all alerts');
                $('#openalerts').load('/Alerts/RefreshOpenAlerts');
                $('#closedalerts').load('/Alerts/RefreshClosedAlerts');
            }
            else {
                showBootstrapAlert('danger', data.message || 'Failed to close all alerts');
            }
        }
    });
}
