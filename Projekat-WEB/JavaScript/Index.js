
$(document).ready(function () {

    var TipManifestacije = {
        Koncert: 0,
        Festival: 1,
        Pozoriste: 2,
        Predstava: 3,
    };

    function string_of_enum(TipManifestacije, value) {
        for (var k in TipManifestacije) {
            if (TipManifestacije[k] == value) { return k; }
        }
        return null;
    }

    function kreirajTabelu(data) {

        let table = "<table id='manifestacije'>"
        table += "<th>Naziv manifestacije </th><th>Tip manifestacije</th><th>Datum i vreme odrzavanja</th><th>Preostali broj mesta</th><th>Cena regular karte</th><th>Status</th><th>Poster manifestacije" +
            "</th><th>Ulica i broj</th><th>Grad </th><th>Postanski broj</th><th>Akcija / Ocena</th>";
        for (product in data) {
            if (data[product].LogickiObrisan == false) {
                var id = data[product].Naziv + ";" + data[product].Lokacija.Geografska_Sirina + ";" + data[product].Lokacija.Geografska_Duzina
                    + ";" + data[product].Lokacija.Mesto_Odrzavanja.Ulica + ";" + data[product].Lokacija.Mesto_Odrzavanja.Broj + ";" + data[product].Lokacija.Mesto_Odrzavanja.Grad + ";" + data[product].Lokacija.Mesto_Odrzavanja.Postanski_Broj;

                var datumVreme = data[product].Datum_i_Vreme_Odrzavanja.split('T');
                var status;
                if (data[product].Status)
                    status = "Aktivna";
                else
                    status = "Nije aktivna";

                table += "<tr><td>" + data[product].Naziv + "</td><td>" + string_of_enum(TipManifestacije, data[product].Tip_Manifestacije) + "</td><td>"
                    + datumVreme[0] + "    " + datumVreme[1] + "</td><td>" + data[product].Broj_Mesta + "</td><td>" + data[product].Cena_Regularne_Karte + "</td><td>" + status + "</td><td>" +
                    "<img style=\"width:70px;height:50px\" src=\"/Images/" + data[product].Putanja_do_slike + "\" /> </td><td>" + data[product].Lokacija.Mesto_Odrzavanja.Ulica + "    " + data[product].Lokacija.Mesto_Odrzavanja.Broj + "</td><td>" +
                    data[product].Lokacija.Mesto_Odrzavanja.Grad + "</td><td>" + data[product].Lokacija.Mesto_Odrzavanja.Postanski_Broj + "</td>";

                if ((Date.parse(data[product].Datum_i_Vreme_Odrzavanja) - Date.parse(new Date())) < 0) {

                    if (data[product].ProsecnaOcena == 0) {
                        table += "<td>Nije jos ocenjena</td></tr> "
                    }
                    else {
                        table += "<td>" + data[product].ProsecnaOcena + "</td></tr> "
                    }
                }
                else {

                    table += "<td><a id='izmena' href=\"KreirajManifestaciju.html?id=" + id + " \"> Izmeni </a></td></tr> "
                }


            }

        }

        table += "</table>";
        return table;
    }
    var Uloga = {
        Kupac: 0,
        Prodavac: 1,
        Administrator: 2
    }

    var user;
    $.ajax({
        type: "get",
        url: "/api/korisnik/vratikorisnika",
        success: function (data) {
            user = data;
            if (user == "null" || user == "" || user == undefined) {
                //$("body").html("<img src='/Images/zabrana.png' />");
                window.location.href = "/Views/Pocetna.html";
            }
            else if (string_of_enum(Uloga, data.Uloga) != "Prodavac") {

                //$("body").html("<img src='/Images/zabrana.png' />");
                window.location.href = "/Views/Pocetna.html";
            }
            sessionStorage.setItem("user", JSON.stringify(user));
            console.log(JSON.parse(sessionStorage.getItem("user")));
        },
        error: function (error) {
            alert(error);
        }
    })



    $("#btnucitaj").click(function () {
        $("#greska").hide();
        $.ajax({
            type: "get",
            url: "/api/prodavac/manifestacije",
            success: function (data) {
                var table = kreirajTabelu(data);
                if (table.includes("<tr>")) {
                    //var table = kreirajTabelu(data);
                    $("#sadrzaj").html(table);
                }
                else
                    $("#sadrzaj").html("<h2>Prodavac nema manifestacija</h2>")
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");

            }
        })
    })

    $("#profilKorisnika").click(function () {

        window.location.href = "/Views/ProfilKorisnika.html";
    })

    function kreirajTabeluKomentari(data) {
        let table = "<table id='komentari'>";
        table += "<th>Ime kupca </th><th> Prezime kupca</th><th>Tekst komentara</th><th>Naziv manifestacije</th><th>Tip manifestacije</th><th>Datum i vreme odrzavanja</th><th>Ocena</th><th colspan='2' >Akcija</th>"
        for (product in data) {
            if (data[product].LogickiObrisan == false) {
                var id_komentara = data[product].Kupac.Ime + ";" + data[product].Tekst_komentara;

                var datumVreme = data[product].Manifestacija.Datum_i_Vreme_Odrzavanja.split('T');

                table += "<tr><td>" + data[product].Kupac.Ime + "</td><td>" + data[product].Kupac.Prezime + "</td><td>" + data[product].Tekst_komentara + "</td><td>" +
                    data[product].Manifestacija.Naziv + "</td><td>" + string_of_enum(TipManifestacije, data[product].Manifestacija.Tip_Manifestacije) + "</td><td>" +
                    datumVreme[0] + "    " + datumVreme[1] + "</td><td>" + data[product].Ocena + "</td>";
                if (data[product].Odobren && data[product].Odbijen == false) {
                    table += "<td colspan='2' align='center' style='color:#14c800;letter-spacing:3px;' ><b>Odobren</b></td><td class=\"id\" hidden>" + id_komentara + "</td></tr>";
                }
                else if (data[product].Odobren == false && data[product].Odbijen == false) {
                    table += "<td><button id=\"dugmeodobri\"> Odobri komentar </button></td>";
                    table += "<td><button id=\"dugmeodbij\"> Odbij komentar </button></td><td class=\"id\" hidden>" + id_komentara + "</td></tr>";
                }
                else if (data[product].Odobren == false && data[product].Odbijen) {

                    table += "<td colspan='2' align='center' style='color:#e80000;letter-spacing:3px;' ><b>Odbijen</b></td><td class=\"id\" hidden>" + id_komentara + "</td></tr>";
                }
            }

        }
        return table;
    }

    $(document).on("click", "#dugmeodobri", function () {
        $("#greska").hide();
        var idk = $(this).closest("tr").find(".id").text();
        $.ajax({
            type: "post",
            url: "/api/prodavac/odobri",
            data: "=" + idk.toString(),
            success: function (data) {
                console.log("Izabrani komentar je odobren");
                alert(data);
                $("#dugmekomentar").click();
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })

    })
    $(document).on("click", "#dugmeodbij", function () {
        $("#greska").hide();
        var idk = $(this).closest("tr").find(".id").text();
        $.ajax({
            type: "post",
            url: "/api/prodavac/odbij",
            data: "=" + idk.toString(),
            success: function (data) {
                console.log("Izabrani komentar je odbijen");
                alert(data);
                $("#dugmekomentar").click();
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })

    })

    $("#dugmekomentar").click(function () {
        $("#greska").hide();
        $.ajax({
            type: "get",
            url: "/api/prodavac/svikomentari",
            success: function (data) {
                var table = kreirajTabeluKomentari(data);

                if (data != undefined && table.includes("<tr>")) {

                    $("#sadrzaj").html(table);
                }
                else
                    $("#sadrzaj").html("<h2>Ne postoje komentari za Vase manifestacije</h2>")
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })
    })

    $("#dugmeOdjava").click(function () {
        $("#greska").hide();
        $.ajax({
            type: "get",
            url: "/api/default/odjava",
            success: function (data) {
                window.location.href = "/Views/" + data;
                sessionStorage.clear();
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })
    })

})
