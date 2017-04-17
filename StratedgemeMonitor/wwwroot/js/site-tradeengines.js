function resetTradingConnectionStatus(tradeEngine, newConnectionStatus) {
    $.ajax({
        type: 'GET',
        url: '/api/tradeengine/' + tradeEngine + '/resettradingconnectionstatus/' + newConnectionStatus,
        success: function (data) {
            console.log(data);
            if (data.item1) {
                showBootstrapAlert('success', data.item2 || 'Successfully reset trading connection status for ' + tradeEngine);
                location.reload();
            }
            else {
                showBootstrapAlert('danger', data.item2 || 'Failed to reset trading connection status for ' + tradeEngine);
            }
        }
    });
}

function requestStratToResetPositionStatus(tradeEngine, strat) {
    $.ajax({
        type: 'GET',
        url: '/api/tradeengine/' + tradeEngine + '/resetpositionstatusstrategy/' + strat,
        success: function (data) {
            console.log(data);
            if (data.item1) {
                showBootstrapAlert('success', data.item2 || 'Successfully requested strat ' + strat + ' of ' + tradeEngine + ' to reset its position status');
            }
            else {
                showBootstrapAlert('danger', data.item2 || 'Failed to request strat ' + strat + ' of ' + tradeEngine + ' to reset its position status');
            }
        }
    });
}