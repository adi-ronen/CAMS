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
    ev.dataTransfer.setData("text", ev.target.id);
}
drop = function(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    document.getElementById(data).remove();
    
    //computers_list.remove --> id computer name
    //LabErea.add {
    //    string top = item.LocationInLab.Split(',')[0];
    //    string left = item.LocationInLab.Split(',')[1];
    //    <figure class="draggable context-menu-one" style="position:absolute;top:@top;left:@left">
    //        <img src="~/Images/clear.png" width="70">
    //            <figcaption class="text-center">@item.ComputerName</figcaption>
    //            <a style="visibility:hidden">@item.ComputerId</a>
    //                    </figure>
    //}
}