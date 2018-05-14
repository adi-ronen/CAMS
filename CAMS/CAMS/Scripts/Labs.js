$(document).ready(function () {
    $(".draggable").draggable({
        containment: "#LabErea",
        grid: [10, 10]
    });
    $(function () {
        $.contextMenu({
            selector: '.context-menu-one',
            callback: function (obj) {
                this.remove(obj);
            },
            items: {
                "delete": {
                    name: "מחק מחשב",
                    callback: function (itemKey, opt, rootMenu, originalEvent) {
                        opt.$trigger.remove();
                    }
                }
            }
        });
    });
});
SaveComputersLocations = function () {
    var coms = {};
    $(".draggable").each(function () {
        let Computer_Position = $(this).position();
        let Div_Height = parseFloat($(this).parent().css("height"));
        let Div_Width = parseFloat($(this).parent().css("width"));
        let New_Computer_Top = Math.round(Computer_Position.top * 100 / Div_Height);
        let New_Computer_Left = Math.round(Computer_Position.left * 100 / Div_Width);
        coms[$(this).children(2).text()] = New_Computer_Top + '%,' + New_Computer_Left + '%';
    });
    $.ajax({
        url: "/Labs/Update",
        type: 'post',
        data: {
            computers: coms, LabId: $("#LabId").val(), RoomNumber: $("#RoomNumber").val(), Building: $("#Building").val()
        },
        success: function (data) {
            alert(data);
        }
    });
};
allowDrop = function(ev) {
    ev.preventDefault();
}
drag = function (ev) {
    var computer = { Name: ev.target.id, Doamin: ev.target.children[2].name }
    ev.dataTransfer.setData("computer", computer);
}
drop = function(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("computer");
    var positionX = ev.positionX;
    var positionY = ev.positionY;

}