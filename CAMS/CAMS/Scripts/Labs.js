$(document).ready(function () {
    Draggable();
});
Draggable = function () {
    $(".draggable").draggable({
        containment: "#LabErea",
        grid: [5, 5],
        snap: true
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
                        Search();
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
dropList = function (element) {
    var computer_name = element[0].id;
    var computer_id = element[0].children[2].text;
    element.remove();
    $("#computers_list").append("<figure draggable=\"true\" ondragstart=\"drag(event)\" class=\"draggable-computer-list row context-menu-one computers-list\" id=" + computer_name + ">" +
        "<img draggable=\"false\" src=\"/Images/clear.png\" height=\"30\">" +
        "<figcaption class=\"text-left grab\">" + computer_name + "</figcaption>" +
        "<a hidden>" + computer_id+"</a>"+
        "</figure>");
}
dropErea = function (ev) {
    ev.preventDefault();
    var computer_name = ev.dataTransfer.getData("computer_name");
    var computer_id = ev.dataTransfer.getData("computer_id");
    if (computer_name !== 'undefined' || computer_id !== 'undefined' && computer_name !== '') {
        $("#" + computer_name).remove();
        var left = Math.round((ev.offsetX / $("#LabErea").width()) * 100);
        var top = Math.round((ev.offsetY / $("#LabErea").height()) * 100);
        $("#LabErea").append("<figure id=" + computer_name + " class=\"draggable context-menu-one\" style=\"position:absolute;top:" + top + "%; left: " + left + "%\">" +
            "<img src=\"/Images/clear.png\" width=\"70\">" +
            "<figcaption class=\"text-center\">" + computer_name + "</figcaption>" +
            "<a hidden>," + computer_id + "</a>" +
            "</figure>");
        Draggable();
    }
}
drag = function (ev) {
    if (typeof ev.target.id !== 'undefined' && typeof (ev.target.children[2].id) !== 'undefined') {
        ev.dataTransfer.setData("computer_name", ev.target.id);
        ev.dataTransfer.setData("computer_id", ev.target.children[2].id );
    }
}
Search = function () {
    var search = $("#search").val();
    $('#computers_list > figure').each(function () {
        if (this.id.toLowerCase().indexOf(search.toLowerCase()) === -1) {
            this.setAttribute('hidden', 'hidden');
        }
        else {
            this.removeAttribute('hidden');
        }
    });

}