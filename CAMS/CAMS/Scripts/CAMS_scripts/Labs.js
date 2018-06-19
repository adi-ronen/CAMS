$(document).ready(function () {
    Draggable();
    ComputerSize = $("#ComputerSize").val();
    ChangeComputerSize(ComputerSize);
});
Draggable = function () {
    $(".multidraggable").multidraggable({
        containment: "#LabErea"
    });
    $("#LabErea").selectable();
}
SaveComputersLocations = function () {
    var coms = {};
    coms[0] = 0;
    $(".multidraggable").each(function () {
        let Computer_Position = $(this).position();
        let Div_Height = parseFloat($(this).parent().css("height"));
        let Div_Width = parseFloat($(this).parent().css("width"));
        let New_Computer_Top = Math.round(Computer_Position.top * 100 / Div_Height);
        New_Computer_Top = Math.ceil(New_Computer_Top / 5) * 5;
        let New_Computer_Left = Math.round(Computer_Position.left * 100 / Div_Width);
        New_Computer_Left = Math.ceil(New_Computer_Left / 5) * 5;
        coms[$(this).children(2).text()] = New_Computer_Top + '%,' + New_Computer_Left + '%';
    });
    $.ajax({
        url: "/Labs/Update",
        type: 'post',
        data: {
            computers: coms, LabId: $("#LabId").val(), RoomNumber: $("#RoomNumber").val(), ComputerSize: $("#ComputerSize").val()
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
    var computer_name = element.id;
    var computer_id = element.children[2].text;
    element.remove();
    $("#computers_list").append("<figure draggable=\"true\" ondragstart=\"drag(event)\" class=\"flex-nowrap draggable-computer-list row context-menu-one computers-list\" id=" + computer_name + ">" +
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
        $("#LabErea").append("<figure id=" + computer_name + " class=\"multidraggable grab\" style=\"position:absolute;top:" + top + "%; left: " + left + "%\">" +
            "<img class=\"sizeable\" src=\"/Images/clear.png\">" +
            "<figcaption class=\"text-center sizeable\">" + computer_name + "</figcaption>" +
            "<a hidden>," + computer_id + "</a>" +
            "</figure>");
        Draggable();
        var size;
        ChangeComputerSize($("#ComputerSize").val());        
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
            $(this).removeAttr('hidden');
        }
    });
}
Delete = function () {
    $('figure[class*="ui-selected"]').each(function () {
        dropList(this);
    });
    Search();
}
ChangeComputerSize = function (width) {
    width = parseInt(width);
    $('input[type*="radio"]').each(function () {
        $(this).removeAttr('checked');
    });
    var font =  ((width / 5) + 4) + "px";
    if (width >= 70) {
        $("#Large").attr('checked', 'checked');
    }
    else if (width <= 30) {
        $("#Small").attr('checked', 'checked');
    }
    else {
        $("#Medium").attr('checked', 'checked');
    }
    $('img[class*="sizeable"]').each(function () {
        $(this).css("width", width);
    });
    $('figcaption[class*="sizeable"]').each(function () {
        $(this).css("font-size", font);
    });
    $("#ComputerSize").val(width);
}
