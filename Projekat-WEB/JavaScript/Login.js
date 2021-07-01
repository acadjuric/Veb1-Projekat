
$(document).ready(function () {

    var user = JSON.parse(sessionStorage.getItem("user"));
    if (user == null) {

        $("#btn").click(function () {
            $("#greska").hide();
            if ($("#username").val() == "") {
                alert("Unesite korisnicko ime");
                $("#username").focus();
                return false;
            }
            else if ($("#sifra").val() == "") {
                alert("Unesite sifru");
                $("#sifra").focus();
                return false;
            }
            else if ($("#username").val().length < 3) {
                alert("Korisnicko ime mora imati vise od 3 karaktera");
                $("#username").focus();
                return false;
            }
            else if ($("#sifra").val().length < 5) {
                alert("Sifra mora imati vise od 5 karaktera");
                $("#sifra").focus();
                return false;
            }
            else {
                var korisnik = {
                    "Ime": $("#username").val().toString(),
                    "Prezime": $("#sifra").val().toString(),
                }

                $.ajax({
                    type: 'Post',
                    url: "/api/default/login",
                    data: korisnik,
                    success: function (data) {

                        var res = data.split(";");
                        if (res[1] == "obrisan") {
                            //korisnik nema nalog neka se registruje
                            alert(res[1]);
                        }
                        if (res[1] == "blokiran") {
                            //korisnik ima nalog trenutno mu nije dozvoljen pristup sajtu
                            alert(res[1]);
                        }
                        window.location.href = res[0];
                    },
                    error: function (error) {
                        var greska = JSON.parse(error.responseText);
                        $("#greska").show();
                        $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                    }
                })
                return true;
            }

        })


    }
    else {
        window.location.href = "/Views/Pocetna.html";
    }


})