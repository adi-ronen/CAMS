$(document).ready(function () {
    $(function () {
        $(".datepicker").datepicker();
        $('.timepicker').timePicker({
            timeFormat: 'hh:mm',
            interval: 60,
            defaultTime: '11',
            dynamic: false,
            dropdown: true,
            scrollbar: true
        });
    });
});