Search = function () {
    var search = $("#search").val();
    var header = true;
    $('tr').each(function () {
        if (header) {
            header = false;
        } else {
            if (typeof this.children["0"].textContent !== 'undefined') {
                if (this.children["0"].textContent.toLowerCase().indexOf(search.toLowerCase()) === -1) {
                    this.setAttribute('hidden', 'hidden');
                } else {
                    this.removeAttribute('hidden');
                }
            }
        }
    });
}
