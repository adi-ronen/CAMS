$(document).ready(function () {
    $(".datepicker").datepicker($.datepicker.regional['he']);
    $(".datepicker").datepicker({
        changeMonth: true,
        changeYear: true
    });
    $(".timepicker").timepicker({ 'timeFormat': 'H:i', 'step': '60' }); 
});

CreateReport = function () {
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    var fromTime = $("#fromTime").val();
    var ToTime = $("#ToTime").val();
    //TBD - CALL MODEL TO CREATE REPORT WITH THIS PARAMS
}

