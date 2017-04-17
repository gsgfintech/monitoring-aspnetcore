function resetTradingConnectionStatus(tradeEngine, newConnectionStatus) {
    $.ajax({
        type: 'GET',
        url: '/api/tradeengine/' + tradeEngine + '/resettradingconnectionstatus/' + newConnectionStatus,
        success: function (data) {
            console.log(data);
            if (data.item1) {
                showBootstrapAlert('success', data.item2 || 'Successfully reset trading connection status for ' + tradeEngine);
                //$('#systemslist').load('/Alerts/SystemsListViewComponent');
            }
            else {
                showBootstrapAlert('danger', data.item2 || 'Failed to reset trading connection status for ' + tradeEngine);
            }
        }
    });
}