$(document).ready(function () {
    $(".draggable").draggable({
        containment: "#LabErea"
    });
});
SaveComputersLocations = function () {
    $(".draggable").each(function () {
            let Computer_Position = $(this).position();
            let Div_Height = parseFloat($(this).parent().css("height"));
            let Div_Width = parseFloat($(this).parent().css("width"));
            let New_Computer_Top = Math.round(Computer_Position.top * 100 / Div_Height)
            let New_Computer_Left = Math.round(Computer_Position.left * 100 / Div_Width)
            alert($(this).children(1).text() + '     ' + New_Computer_Top + '%,' + New_Computer_Left + '%');
    });
};