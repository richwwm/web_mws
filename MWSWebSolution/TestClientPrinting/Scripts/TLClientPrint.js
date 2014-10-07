/*
* ThermalLabel Client Print Utility
* part of ThermalLabel SDK 5.0
* http://www.neodynamic.com/
* Copyright (c) 2012 - Neodynamic SRL
*/

$(document).ready(function () {
    $('<iframe />', {
        name: 'tlprint',
        id: 'tlprint',
        width: '1',
        height: '1',
        style: 'visibility:hidden;position:absolute'
    }).appendTo('body');
})

function printThermalLabel() {
    var webPrintJobUrl = $(location).attr('href') + "?webPrintJob=t";
    $("#tlprint").attr("src", "tlprint:" + webPrintJobUrl);
}
                            