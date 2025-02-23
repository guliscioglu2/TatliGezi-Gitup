function sil(url, id) {
    var cnf = confirm("Silmek istediğinize emin misiniz?");
    if (cnf)
        if (cnf) {
            $.ajax({
                type: 'POST',
                url: url + id,
                success: function () {
                    $("#a_" + id).fadeOut(2000);
                    window.location.reload(false);
                }
            });
        }
        else
            alert("Vazgeçtiniz");

}