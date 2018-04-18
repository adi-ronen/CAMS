$(document).ready(function () {
    $(".datepicker").datepicker($.datepicker.regional['he']);
    $(".datepicker").datepicker({
        changeMonth: true,
        changeYear: true
    });
    //TBD - תאריך עד היום, שעות דיפולטיביות
    $(".timepicker").timepicker({ 'timeFormat': 'H:i', 'step': '60' });

    $(".accordion")
        .accordion({
            collapsible: true,
            active: false,
            heightStyle: "content"
        });
    $(".accordion").find('input[type="checkbox"]').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        setTimeout(function () {
            this.checked = !this.checked;
        }.bind(this), 100);
    });
});

CreateReport = function () {
    fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    var fromTime = $("#fromTime").val();
    var ToTime = $("#ToTime").val();
    var LabsId = [];
    labs = document.getElementsByClassName('form-check-inline')
    for (var i = 0, n = labs.length; i < n; i++) {
        if (labs[i].checked) {
            LabsId.push(labs[i].value);
        }
    }
    //TBD - CALL MODEL TO CREATE REPORT WITH THIS PARAMS
}

Checkbox = function (depId) {
    checkboxes = document.getElementsByClassName(depId);
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        checkboxes[i].checked = document.getElementById(depId).checked;
    }
}

CheckboxAll = function (checked) {
    deps = document.getElementsByClassName('form-check-label');
    for (var i = 0, n = deps.length; i < n; i++) {
        deps[i].checked = checked;
        Checkbox(deps[i].id);
    }
}