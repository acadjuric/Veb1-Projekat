
$(document).ready(function () {

    var Uloga = {
        Kupac: 0,
        Prodavac: 1,
        Administrator: 2
    }

    function string_of_enum(Pol, value) {
        for (var k in Pol) {
            if (Pol[k] == value) { return k; }
        }
        return null;
    }

    var user = JSON.parse(sessionStorage.getItem("user"));
    if (user != null && string_of_enum(Uloga, user.Uloga) == "Administrator") {
        $("#btn").click(function () {

            $("#info").hide();

            if ($("#username").val().length < 3) {
                alert("Korisnicko ime mora imati vise od 3 karaktera");
                $("#username").focus();
                return false;
            }
            if (!isNaN($("#username").val())) {
                alert("Polje za korisnicko ime ne moze da ima samo brojeve.")
                $("#username").focus();
                return false;
            }
            if ($("#sifra").val().length < 5) {
                alert("Sifra mora imati najmanje 5 karaktera");
                $("#sifra").focus();
                return false;
            }
            if ($("#ime").val().length < 3) {
                alert("Ime ne moze imati manje od 3 karaktera");
                $("#ime").focus();
                return false;
            }
            if (!isNaN($("#ime").val())) {
                alert("Polje za ime ne moze imati brojeve,iskljucivo slova.");
                $("#ime").focus();
                return false;
            }
            if ($("#prezime").val().length < 4) {
                alert("Prezime ne moze imati manje od 4 karaktera");
                $("#prezime").focus();
                return false;
            }
            if (!isNaN($("#prezime").val())) {
                alert("Polje za prezime ne moze imati brojeve,iskljucivo slova.");
                $("#prezime").focus();
                return false;
            }
            if ($("#pol").val() == "") {
                alert("Molimo vas izaberite pol");
                $("#pol").focus();
                return false;
            }
            if ($("#datum").val() == "") {
                alert("Molimo vas izaberite datum rodjenja");
                $("#datum").focus();
                return false;
            }

            var korisnik = {
                "Korisnicko_Ime": $("#username").val(),
                "Lozinka": $("#sifra").val(),
                "Ime": $("#ime").val(),
                "Prezime": $("#prezime").val(),
                "Pol": $("#pol").val(),
                "Datum_Rodjenja": $("#datum").val(),
            }

            $.ajax({
                type: "post",
                url: "/api/admin/kreirajprodavca",
                data: korisnik,
                success: function (data) {
                    alert(data);
                    window.location.href = "/Views/Admin/AdminPanel.html";
                },
                error: function (error) {
                    var greska = JSON.parse(error.responseText);
                    $("#info").show();
                    $("#info").html("<p style='color:red'>" + greska.Message + "</p>");
                    
                }
            })




        })
    }
    else {
        //$("body").html("<img src='/Images/zabrana.png' />");
        window.location.href = "/Views/Pocetna.html";
    }
})
