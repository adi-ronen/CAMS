NewUser = function () {
    email = $("#NewUserEmail").val();
    if (validateEmail(email) && email.toLowerCase().indexOf("bgu.ac.il") !== -1) {
        $("#UsersList").append("<option selected>" + email + "</option>");
        $("#newUser").attr('hidden', 'hidden');
    } else {
        alert(email + " is not valid ");
    }
}
ShowNewUser = function () {
    $('#newUser').removeAttr('hidden');
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}