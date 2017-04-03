function startSystem(systemName) {
    $.ajax({
        type: 'GET',
        url: '/api/systems/start/' + systemName,
        success: function (data) {
            console.log(data);
            if (data.success) {
                showBootstrapAlert('success', data.message || 'Successfully started ' + systemName);
                $('#systemslist').load('/Alerts/SystemsListViewComponent');
            }
            else {
                showBootstrapAlert('danger', data.message || 'Failed to start ' + systemName);
            }
        }
    });
}

function stopSystem(systemName) {
    $.ajax({
        type: 'GET',
        url: '/api/systems/stop/' + systemName,
        success: function (data) {
            console.log(data);
            if (data.success) {
                showBootstrapAlert('success', data.message || 'Successfully stopped ' + systemName);
                $('#systemslist').load('/Alerts/SystemsListViewComponent');
            }
            else {
                showBootstrapAlert('danger', data.message || 'Failed to stop ' + systemName);
            }
        }
    });
}

function deleteSystem(systemName) {
    $.ajax({
        type: 'DELETE',
        url: '/api/systems/' + systemName,
        success: function (data) {
            console.log(data);
            if (data.success) {
                showBootstrapAlert('success', data.message || 'Successfully deleted ' + systemName);
                $('#systemslist').load('/Alerts/SystemsListViewComponent');
            }
            else {
                showBootstrapAlert('danger', data.message || 'Failed to start ' + systemName);
            }
        }
    });
}