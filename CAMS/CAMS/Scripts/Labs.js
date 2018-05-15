$(document).ready(function () {
    Draggable();
});
Draggable = function () {
    $(".draggable").draggable({
        containment: "#LabErea",
        grid: [5, 5]
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
                        dropList(opt.$trigger);
                    }
                }
            }
        });
    });
}
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
allowDrop = function (ev) {
    ev.preventDefault();
}
drag = function (ev) {
    ev.dataTransfer.setData("text", ev.target.id);
}
dropList = function (element) {
    //TBD - if this item is from drop list dont do a thing
    //ev.preventDefault();
    //var data = ev.dataTransfer.getData("text");
    data = element[0].id;
    element.remove();
    $("#computers_list").append("<figure draggable=\"true\" ondragstart=\"drag(event)\" class=\"draggable-computer-list row context-menu-one computers-list\" id=" + data + ">" +
        "<img draggable=\"false\" src=\"/Images/clear.png\" height=\"30\">" +
        "<figcaption class=\"text-left grab\">" + data + "</figcaption>" +
        "</figure>");
}
dropErea = function (ev) {
    //TBD - if this item is from lab erea dont do a thing
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    $("#" + data).remove();
    var left = Math.round((ev.offsetX / $("#LabErea").width()) * 100);
    var top = Math.round((ev.offsetY / $("#LabErea").height()) * 100);
    $("#LabErea").append("<figure id=" + data + " class=\"draggable context-menu-one\" style=\"position:absolute;top:" + top + "%; left: " + left + "%\">" +
        "<img src=\"/Images/clear.png\" width=\"70\">" +
        "<figcaption class=\"text-center\">" + data + "</figcaption>" +
        "</figure>");
    Draggable();
}