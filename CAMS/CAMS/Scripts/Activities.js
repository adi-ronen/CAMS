$(document).ready(function () {
    $(".datepicker").datepicker($.datepicker.regional['he']);
    $(".datepicker").datepicker({
        changeMonth: true,
        changeYear: true
    });
    $(".timepicker").timepicker({ 'timeFormat': 'H:i', 'step': '60' });
});