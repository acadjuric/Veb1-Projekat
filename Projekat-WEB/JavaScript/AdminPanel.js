

$(document).ready(function () {
    $(".messi").hide();
    var rezultatPretrage;
    var Uloga = {
        Kupac: 0,
        Prodavac: 1,
        Administrator: 2
    }
    var Pol = {
        Musko: 0,
        Zensko: 1,
    };
    var Tip = {
        Zlatni: 0,
        Srebrni: 1,
        Bronzani: 2,
        Pocetnik: 3,
    }
    var TipManifestacije = {
        Koncert: 0,
        Festival: 1,
        Pozoriste: 2,
        Predstava: 3,
    };
    var StatusKarte = {
        Rezervisana: 0,
        Odustanak: 1,
    };
    var TipKarte = {
        VIP: 0,
        Regular: 1,
        FanPit: 2,
    };
    var id;
    function kreirajTabeluManifestacije(data) {

        let table = "<table border='1' >"
        table += "<th>Naziv manifestacije</th><th>Tip manifestacije</th><th>Datum i vreme odrzavanja</th><th>Cena regular karte</th><th>Poster manifestacije" +
            "</th><th>Ulica i broj</th><th>Grad</th><th>Postanski broj</th><th colspan='2'>Akcije</th>";
        for (product in data) {
            if (!data[product].LogickiObrisan) {
                id = data[product].Naziv + ";" + data[product].Tip_Manifestacije + ";" + data[product].Datum_i_Vreme_Odrzavanja + ";" + data[product].Lokacija.Geografska_Sirina + ";" + data[product].Lokacija.Geografska_Duzina +
                    ";" + data[product].Lokacija.Mesto_Odrzavanja.Ulica + ";" + data[product].Lokacija.Mesto_Odrzavanja.Broj + ";" + data[product].Lokacija.Mesto_Odrzavanja.Grad + ";" + data[product].Lokacija.Mesto_Odrzavanja.Postanski_Broj;

                var datumVreme = data[product].Datum_i_Vreme_Odrzavanja.split('T');

                table += "<tr><td>" + data[product].Naziv + "</td><td>" + string_of_enum(TipManifestacije, data[product].Tip_Manifestacije) + "</td><td>"
                    + datumVreme[0] + "    " + datumVreme[1] + "</td><td>" + data[product].Cena_Regularne_Karte + "</td><td>" +
                    "<img style=\"width:70px;height:50px\" src=\"/Images/" + data[product].Putanja_do_slike + "\" /> </td><td>" + data[product].Lokacija.Mesto_Odrzavanja.Ulica + "  " + data[product].Lokacija.Mesto_Odrzavanja.Broj + "</td><td>" +
                    data[product].Lokacija.Mesto_Odrzavanja.Grad + "</td><td>" + data[product].Lokacija.Mesto_Odrzavanja.Postanski_Broj + "</td><td><button id=\"obrisiManifestaciju\"> Obrisi Manifestaciju </button></td>";
                if (!data[product].Status) {

                    if ((Date.parse(data[product].Datum_i_Vreme_Odrzavanja) - Date.parse(new Date())) < 0) {

                        table += "<td style='color:#fff'> - </td><td hidden class=\"id\">" + id + "</td></tr>";
                    }
                    else {
                        table += "<td> <button id=\"odobriManifestaciju\" > Odobri </button> </td><td hidden class=\"id\">" + id + "</td></tr>";
                    }

                }
                else {
                    table += "<td style='color:#fff;letter-spacing:2px'><b>Odobrena</b></td><td hidden class=\"id\">" + id + "</td></tr>";
                }


            }

        }

        table += "</table>";
        return table;
    }

    function string_of_enum(Pol, value) {
        for (var k in Pol) {
            if (Pol[k] == value) { return k; }
        }
        return null;
    }
    function kerirajTabeluSumnjivihKorisnika(data) {
        let table = "<table border='1'>"
        table += "<th>Korisnicko ime</th><th>Lozinka</th><th>Ime</th><th>Prezime</th><th>Pol</th><th>Uloga</th><th>Datum rodjenja</th><th>Sakupljeni bodovi</th><th>Tip korisnika</th><th>Popust korisnika</th><th>" +
            "Bodovi za napredak</th><th colspan='2'>Akcije</th>";
        for (korisnik in data) {
            table += "<tr> <td class=\"id\">" + data[korisnik].Korisnicko_Ime + "</td>" + "<td> " + data[korisnik].Lozinka + "</td>" + "<td> " + data[korisnik].Ime + "</td>"
                + "<td> " + data[korisnik].Prezime + "</td>" + "<td> " + string_of_enum(Pol, data[korisnik].Pol) + "</td>" + "<td> " + string_of_enum(Uloga, data[korisnik].Uloga) + "</td>" + "<td> " + data[korisnik].Datum_Rodjenja + "</td>"
                + "<td> " + data[korisnik].Broj_Sakupljenih_Bodova.toFixed(2) + "</td>" + "<td> " + string_of_enum(Tip, data[korisnik].TipKorisnika.Ime_Tipa_Korisnika) + "</td>" +
                "<td> " + data[korisnik].TipKorisnika.Popust + "</td>" + "<td> " + data[korisnik].TipKorisnika.Trazeni_Broj_Bodova;
            if (data[korisnik].Blokiran == true) {

                table += "<td style='color:#ff00ff;letter-spacing:1px'><b>Blokiran</b></td></tr>";
            }
            else {
                table += "<td><button id=\"blokirajSumnjivogKorisnika\"> Blokiraj korisnika </button></td></tr>";
            }
        }
        table += "</table>";
        return table;
    }

    function kreirajTabelu(data) {
        let table = "<table border='1' >"
        table += "<th>Korisnicko ime</th><th>Lozinka</th><th>Ime</th><th>Prezime</th><th>Pol</th><th>Uloga</th><th>Datum rodjenja</th><th>Sakupljeni bodovi</th><th>Tip korisnika</th><th>Popust korisnika</th><th>" +
            "Bodovi za napredak</th><th colspan='2'>Akcije</th>";
        for (korisnik in data) {

            table += "<tr> <td class=\"id\">" + data[korisnik].Korisnicko_Ime + "</td>" + "<td> " + data[korisnik].Lozinka + "</td>" + "<td> " + data[korisnik].Ime + "</td>"
                + "<td> " + data[korisnik].Prezime + "</td>" + "<td> " + string_of_enum(Pol, data[korisnik].Pol) + "</td>" + "<td> " + string_of_enum(Uloga, data[korisnik].Uloga) + "</td>" + "<td> " + data[korisnik].Datum_Rodjenja + "</td>"
                + "<td> " + data[korisnik].Broj_Sakupljenih_Bodova.toFixed(2) + "</td>" + "<td> " + string_of_enum(Tip, data[korisnik].TipKorisnika.Ime_Tipa_Korisnika) + "</td>" +
                "<td> " + data[korisnik].TipKorisnika.Popust + "</td>" + "<td> " + data[korisnik].TipKorisnika.Trazeni_Broj_Bodova;
            if (data[korisnik].LogickiObrisan == true || string_of_enum(Uloga, data[korisnik].Uloga) == "Administrator") {

                if (string_of_enum(Uloga, data[korisnik].Uloga) == "Administrator")
                    table += "</td><td>/</td>";
                else
                    table += "</td> <td style='color:#ff1a1a;letter-spacing:1px' ><b>Obrisan</b></td>";
            }
            else {
                table += "</td><td><button id=\"obrisiKorisnika\"> Obrisi korisnika </button></td>";
            }
            if (data[korisnik].Blokiran == true || string_of_enum(Uloga, data[korisnik].Uloga) == "Administrator") {

                if (string_of_enum(Uloga, data[korisnik].Uloga) == "Administrator")
                    table += "<td>/</td></tr>";
                else
                    table += "<td style='color:#ff00ff;letter-spacing:1px'><b>Blokiran</b></td></tr>";
            }
            else {
                if (data[korisnik].LogickiObrisan) {
                    table += "<td>/</td></tr>";
                }
                else {
                    table += "<td><button id=\"blokirajKorisnika\"> Blokiraj korisnika </button></td></tr>";
                }
            }
        }

        table += "</table>";
        return table;
    }

    var user;
    $.ajax({
        type: "get",
        url: "/api/korisnik/vratikorisnika",
        success: function (data) {
            user = data;
            if (user == "null" || user == "" || user == undefined) {
                window.location.href = "/Views/Pocetna.html";
            }
            else if (string_of_enum(Uloga, data.Uloga) != "Administrator") {

                // $("body").html("<img src='/Images/zabrana.png' />");
                window.location.href = "/Views/Pocetna.html";
            }
            sessionStorage.setItem("user", JSON.stringify(user));
            console.log(JSON.parse(sessionStorage.getItem("user")));
        },
        error: function (error) {
            alert(error);
        }
    })

    $(document).on("click", "#obrisiKorisnika", function () {
        $("#greska").hide();
        var id = $(this).closest("tr").find(".id").text();
        if (confirm("Brisanjem kupca brisete i njegove karte i njegove komentare. Brisanjem prodavca brisete njegove manifestacije kao i karte, komentare za te manifestacije. Zelite da nastavite?")) {
            $.ajax({
                type: "get",
                url: "/api/admin/obrisik/" + id,
                success: function (data) {
                    alert(data);
                    $("#dugmeZaKorisnike").click();
                },
                error: function (error) {
                    var greska = JSON.parse(error.responseText);
                    $("#greska").show();
                    $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                }
            })
        }
    })


    //blokirajSumnjivogKorisnika
    $(document).on("click", "#blokirajSumnjivogKorisnika", function () {
        $("#greska").hide();
        var id = $(this).closest("tr").find(".id").text();
        $.ajax({
            type: "get",
            url: "/api/admin/blokiraj/" + id,
            success: function (data) {
                console.log("kliknuto na blokiraj");
                alert(data);
                $("#dugmeZaSumnjive").click();
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })
    })

    $(document).on("click", "#blokirajKorisnika", function () {
        $("#greska").hide();
        var id = $(this).closest("tr").find(".id").text();
        $.ajax({
            type: "get",
            url: "/api/admin/blokiraj/" + id,
            success: function (data) {
                console.log("kliknuto na blokiraj");
                alert(data);
                $("#dugmeZaKorisnike").click();

            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })
    })

    $(document).on("click", "#obrisiManifestaciju", function () {
        $("#greska").hide();
        var id = $(this).closest("tr").find(".id").text();
        if (confirm("Brisanjem manifestacije brisete sve karte i sve komentare za tu manifestaciju. Zelite da nastavite?")) {
            $.ajax({
                type: "post",
                url: "/api/admin/obrisim",
                data: "=" + id,
                success: function (data) {
                    console.log("Uspesno obrisana manifestacija");
                    alert(data);
                    $("#dugmeZaManifestacije").click();
                },
                error: function (error) {
                    var greska = JSON.parse(error.responseText);
                    $("#greska").show();
                    $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                }

            })
        }

    })

    $(document).on("click", "#odobriManifestaciju", function () {
        $("#greska").hide();
        var id = $(this).closest("tr").find(".id").text();
        $.ajax({
            type: "post",
            url: "/api/admin/odobrim",
            data: "=" + id,
            success: function (data) {
                console.log("Uspesno odobrena manifestacija");
                alert(data);
                $("#dugmeZaManifestacije").click();
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }

        })

    })

    $("#dugmeZaKorisnike").click(function () {
        $("#greska").hide();
        $.ajax({
            type: "Get",
            url: "/api/admin/prikazisvekorisnike",
            success: function (data) {
                if (data.length > 0) {
                    var table = kreirajTabelu(data);
                    $("#sadrzaj").html(table);
                    rezultatPretrage = data;
                    $(".messi").show();
                    //$("#sortiranje_i_filtriranje").show();
                }
                else
                    $("#sadrzaj").html("<h3>Ne postoje korisnici u sistemu</h3>")
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })
    })

    $("#dugmeZaManifestacije").click(function () {
        $("#greska").hide();
        $(".messi").hide();
        $.ajax({
            type: 'Get',
            url: "/api/default/manifestacije",
            success: function (data) {
                var table = kreirajTabeluManifestacije(data);
                if (table.includes("<tr>")) {
                    $("#sadrzaj").html(table);
                    // rezultatPretrage = data;
                }
                else
                    $("#sadrzaj").html("<h3>Ne postoje manifestacije u sistemu</h3>");
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

    $("#btnPretraga").click(function () {

        $("#greska").hide();
        if ($("#ime").val() == "" && $("#prezime").val() == "" && $("#kime").val() == "") {
            alert("Popunite bar 1 polje za pretragu");
            return false;
        }
        if ($("#ime").val() != "") {
            if (!isNaN($("#ime").val())) {
                alert("Polje za ime kod pretrage ne sme da bude broj");
                $("#ime").focus();
                return false;
            }
        }
        if ($("#prezime").val() != "") {
            if (!isNaN($("#prezime").val())) {
                alert("Polje za prezime kod pretrage ne sme da bude broj");
                $("#prezime").focus();
                return false;
            }
        }
        if ($("#kime").val() != "") {
            if (!isNaN($("#kime").val())) {
                alert("Polje za korisnicko ime kod pretrage ne sme da bude broj");
                $("#kime").focus();
                return false;
            }
        }
        $("#sortiranje_i_filtriranje").show();
        var model = {
            "Ime": $("#ime").val(),
            "Prezime": $("#prezime").val(),
            "Kime": $("#kime").val(),
        }

        $.ajax({
            type: "post",
            url: "/api/admin/pretraga",
            data: model,
            success: function (data) {
                if (data.length > 0) {
                    var table = kreirajTabelu(data);
                    $("#sadrzaj").html(table);
                    $(".messi").show();
                    rezultatPretrage = data;
                }
                else {
                    $(".messi").hide();
                    $("#sadrzaj").html("<h3>Nema pogodaka </h3>");

                }
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })
    })

    $(document).on("change", "#sortiraj, #nacinsortiranja", function () {
        $("#greska").hide();
        var izbor = $("#sortiraj").val();
        var nacin = $("#nacinsortiranja").val();
        model = {
            "ListaK": rezultatPretrage,
            "Izbor": izbor,
            "Nacin": nacin,
        };
        $.ajax({
            type: "post",
            data: model,
            url: "/api/admin/sortiraj",
            success: function (data) {
                if (data.length > 0) {
                    var table = kreirajTabelu(data);
                    $("#sadrzaj").html(table);
                }
                else
                    $("#sadrzaj").html("<h3>Nema pogodaka </h3>");
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })
    })

    $(document).on("change", "#filtriraj", function () {
        $("#greska").hide();
        var izbor = $("#filtriraj").val();
        model = {
            "ListaK": rezultatPretrage,
            "Izbor": izbor.toString(),
        };

        $.ajax({
            type: "post",
            url: "/api/admin/filtriraj",
            data: model,
            success: function (data) {
                if (data.length > 0) {
                    var table = kreirajTabelu(data);
                    $("#sadrzaj").html(table);
                }
                else
                    $("#sadrzaj").html("<h3>Nema pogodaka </h3>");
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }

        })
    })

    function kreirajTabeluKomentari(data) {
        let table = "<table>";
        table += "<th>Ime kupca</th><th>Prezime kupca</th><th>Tekst komentara</th><th>Naziv manifestacije</th><th>Tip manifestacije</th><th>Datum i vreme odrzavanja</th><th>Ocena kupca</th><th>Status</th>";
        for (product in data) {
            if (data[product].LogickiObrisan == false) {
                var datumVreme = data[product].Manifestacija.Datum_i_Vreme_Odrzavanja.split('T');

                table += "<tr><td>" + data[product].Kupac.Ime + "</td><td>" + data[product].Kupac.Prezime + "</td><td>" + data[product].Tekst_komentara + "</td><td>" +
                    data[product].Manifestacija.Naziv + "</td><td>" + string_of_enum(TipManifestacije, data[product].Manifestacija.Tip_Manifestacije) + "</td><td>" +
                    datumVreme[0] + "    " + datumVreme[1] + "</td><td>" + data[product].Ocena + "</td>";
                if (data[product].Odobren == true && data[product].Odbijen == false) {

                    table += "<td style='color:aqua;letter-spacing:2px'><b> ODOBREN</b> </td> </tr>";
                }
                else if (data[product].Odbijen == true && data[product].Odobren == false) {

                    table += "<td style='color:red;letter-spacing:2px'><b> ODBIJEN</b> </td> </tr>";
                }
                else {

                    table += "<td style='color:#fff;letter-spacing:2px'><b> CEKA ODOBRENJE/ODBIJANJE</b> </td> </tr>";
                }
            }

        }
        return table;
    }

    $("#dugmekomentar").click(function () {
        $("#greska").hide();
        $(".messi").hide();
        $.ajax({
            type: "get",
            url: "/api/admin/svikomentari",
            success: function (data) {
                var table = kreirajTabeluKomentari(data);
                if (table.includes("<tr>")) {

                    $("#sadrzaj").html(table);
                }
                else
                    $("#sadrzaj").html("<h3>Nema komentara </h3>");
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })
    })

    $("#dugmeZaSumnjive").click(function () {
        $("#greska").hide();
        $(".messi").hide();
        $.ajax({
            type: "get",
            url: "/api/admin/sumnjivikorisnici",
            success: function (data) {
                if (data.length > 0) {
                    var table = kerirajTabeluSumnjivihKorisnika(data);
                    $("#sadrzaj").html(table);
                }
                else
                    $("#sadrzaj").html("<h3>Nema sumnjivih korisnika </h3>");
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


    function kreirajTabeluKarte(data) {
        let table = "<table>"
        table += "<th>ID karte </th><th> Naziv manifestacije</th><th>Tip manifestacije</th><th>Datum i vreme odrzavanja</th><th> Ime kupca</th><th>Prezime kupca</th><th>Status karte</th><th>Tip karte</th><th>Cena karte</th><th>Ulica i broj" +
            "</th><th>Grad i postanski broj</th><th>Akcija</th>";
        for (product in data) {
            if (!data[product].LogickiObrisan) {
                var datumVreme = data[product].Datum_i_Vreme_Manifestacije.split('T');

                table += "<tr><td class=\"id\">" + data[product].ID_Karte + "</td><td class=\"naziv\">" + data[product].Manifestacija.Naziv + "</td><td>" + string_of_enum(TipManifestacije, data[product].Manifestacija.Tip_Manifestacije)
                    + "</td><td class=\"vreme\">" + datumVreme[0] + "   " + datumVreme[1] + "</td><td>" + data[product].Kupac.Ime + "</td><td>" + data[product].Kupac.Prezime + "</td><td>" +
                    string_of_enum(StatusKarte, data[product].Status_Karte) + "</td><td>" + string_of_enum(TipKarte, data[product].TipKarte) + "</td><td>" + data[product].Cena + "</td><td>" + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica + " " + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Broj +
                    "</td><td>" + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Grad + ", " + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj +
                    "</td><td><button id=\"obrisiKartu\"> Obrisi kartu </button></td></tr>";
            }
        }
        table += "</table>";
        return table;
    }

    $("#dugmeKarte").click(function () {
        $("#greska").hide();
        $(".messi").hide();
        $.ajax({
            type: "get",
            url: "/api/admin/svekarte",
            success: function (data) {

                var table = kreirajTabeluKarte(data);
                if (table.includes("<tr>")) {
                    $("#sadrzaj").html(table);
                }
                else
                    $("#sadrzaj").html("<h3>Nema karata </h3>");
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })

    })

    $(document).on("click", "#obrisiKartu", function () {
        $("#greska").hide();
        var id = $(this).closest("tr").find(".id").text();
        var naziv = $(this).closest("tr").find(".naziv").text();
        var vreme = $(this).closest("tr").find(".vreme").text();

        var identifikator = id.toString() + ";" + naziv.toString() + ";" + vreme.toString();
        $.ajax({
            type: "post",
            url: "/api/admin/obrisikartu",
            data: "=" + identifikator.toString(),
            success: function (data) {
                alert(data);
                $("#dugmeKarte").click();
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })


    })

})

