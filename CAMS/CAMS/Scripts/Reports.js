$(document).ready(function () {
    $(".datepicker").datepicker($.datepicker.regional['he']);
    $(".datepicker").datepicker({
        changeMonth: true,
        changeYear: true,
        maxDate: '0'
    });
    //TBD - תאריך עד היום, שעות דיפולטיביות
    $("#fromTime").timepicker({ 'timeFormat': 'H:i', 'step': '60'});
    $("#toTime").timepicker({ 'timeFormat': 'H:i', 'step': '60'});
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
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    var fromTime = $("#fromTime").val();
    var ToTime = $("#ToTime").val();
    var LabsId = [];
    var labs = document.getElementsByClassName('form-check-inline')
    for (var i = 0, n = labs.length; i < n; i++) {
        if (labs[i].checked) {
            LabsId.push(labs[i].value);
        }
    }
    //TBD - CALL MODEL TO CREATE REPORT WITH THIS PARAMS
}
CheckboxDep = function (depId) {
    var checkboxesDeps = document.getElementsByClassName(depId);
    for (var i = 0, n = checkboxesDeps.length; i < n; i++) {
        checkboxesDeps[i].checked = document.getElementById(depId).checked;
    }
}
CheckboxAll = function (checked) {
    var deps = document.getElementsByClassName('form-check-label');
    for (var i = 0, n = deps.length; i < n; i++) {
        deps[i].checked = checked;
        CheckboxDep(deps[i].id);
    }
}
Checkbox = function (depId) {
    var checkboxes = document.getElementsByClassName(depId);
    var depCheckbox = document.getElementById(depId);
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        if (!checkboxes[i].checked) {
            depCheckbox.checked = false;
            break;
        }
        depCheckbox.checked = true;
    }
}