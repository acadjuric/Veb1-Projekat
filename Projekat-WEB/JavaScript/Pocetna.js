
$(document).ready(function () {

    $(document).on("mouseover", ".karta", function () {
        $(this).siblings().css({ "filter": "blur(5px)" });
    }).on("mouseout", ".karta", function () {
        $(this).siblings().css({ "filter": "blur(0px)" });
    })

    var rezultatPretrage;
    var TipManifestacije = {
        Koncert: 0,
        Festival: 1,
        Pozoriste: 2,
        Predstava: 3,
    };
    var Uloga = {
        Kupac: 0,
        Prodavac: 1,
        Administrator: 2
    };

    function string_of_enum(TipManifestacije, value) {
        for (var k in TipManifestacije) {
            if (TipManifestacije[k] == value) { return k; }
        }
        return null;
    }
    var pretraga = false;
    function kreirajTabelu(data) {

        let table = "<table border = '1'>"
        var cards = "";
        for (product in data) {

            if (data[product].LogickiObrisan == false && data[product].Status) {

                var id = data[product].Naziv + ";" + data[product].Lokacija.Geografska_Sirina + ";" + data[product].Lokacija.Geografska_Duzina
                    + ";" + data[product].Lokacija.Mesto_Odrzavanja.Ulica + ";" + data[product].Lokacija.Mesto_Odrzavanja.Broj + ";" + data[product].Lokacija.Mesto_Odrzavanja.Grad + ";" + data[product].Lokacija.Mesto_Odrzavanja.Postanski_Broj;

                //    table += "<tr><td>" + data[product].Naziv + "</td><td>" + string_of_enum(TipManifestacije, data[product].Tip_Manifestacije) + "</td><td>"
                //        + data[product].Datum_i_Vreme_Odrzavanja + "</td><td>" + data[product].Cena_Regularne_Karte + "</td><td>" +
                //        "<img style=\"width:20px;height:20px\" src=\"/Images/" + data[product].Putanja_do_slike + "\" /> </td><td>" + data[product].Lokacija.Mesto_Odrzavanja.Ulica + data[product].Lokacija.Mesto_Odrzavanja.Broj + "</td><td>" +
                //        data[product].Lokacija.Mesto_Odrzavanja.Grad + "</td><td>" + data[product].Lokacija.Mesto_Odrzavanja.Postanski_Broj + "</td>";
                //    if (pretraga) {
                //        if ((Date.parse(data[product].Datum_i_Vreme_Odrzavanja) - Date.parse(new Date())) < 0) {

                //            table += "<td>" + data[product].ProsecnaOcena + "</td>";
                //        }
                //        else {
                //            table += "<td>Nema prosecne ocene, nije jos zavrsena</td>";
                //        }
                //    }
                //    table += "<td><a href=\"PrikazManifestacije.html?id=" + id + " \"> Vise informacija </a></td></tr> "

                var datumVreme = data[product].Datum_i_Vreme_Odrzavanja.split('T');
                cards += "<div class='karta'> <div class='slika'> <img src=\"/Images/" + data[product].Putanja_do_slike + "\" /></div>";
                cards += "<div class='naslov'><h1>" + data[product].Naziv + "</h1> <h3>" + string_of_enum(TipManifestacije, data[product].Tip_Manifestacije) + "</h3></div>"; //dodati i tip manifestacije kao h3 ili h4
                cards += "<div class='opis'> <p>" + datumVreme[0] + "    " + datumVreme[1] + "</p><span>" + data[product].Lokacija.Mesto_Odrzavanja.Ulica + "  " + data[product].Lokacija.Mesto_Odrzavanja.Broj + "</span><br /><span>"
                    + data[product].Lokacija.Mesto_Odrzavanja.Grad + ", " + data[product].Lokacija.Mesto_Odrzavanja.Postanski_Broj + "</span><p>"
                    + data[product].Cena_Regularne_Karte + "</p>";
                if (pretraga) {
                    if ((Date.parse(data[product].Datum_i_Vreme_Odrzavanja) - Date.parse(new Date())) < 0) {
                        if (data[product].ProsecnaOcena > 0)
                            cards += "<p>" + data[product].ProsecnaOcena + "</p>";
                        else
                            cards += "<p>Nije jos ocenjena</p>";
                    }
                    else {
                        cards += "<p>Nema prosecne ocene, nije jos zavrsena</p>";
                    }
                }

                cards += "<p><a href=\"PrikazManifestacije.html?id=" + id + " \"> Vise informacija </a></p></div> </div>"
            }

        }

        //table += "</table>";
        //return table;
        return cards;
    }

    $.ajax({
        type: 'Get',
        url: "/api/default/manifestacije",
        success: function (data) {
            var table = kreirajTabelu(data);
            if (table.includes("</div>")) {

                $("#sadrzaj").html(table);
                $(".messi").show();
                rezultatPretrage = data;
            }
            else {
                $(".messi").hide();
                $("#sadrzaj").html("<h2>Nema manifestacija</h2>");
            }
        },
        error: function (jqXHR) {
            alert(jqXHR.responseText);
        }
    })

    $("#btnPretraga").click(function () {

        if ($("#datumOD").val() != "" && $("#datumDO").val() == "") {
            alert("Popunite lepo polja za datum kako bi pretraga uspela");
            $("#datumDO").focus()
            return false;
        }
        else if ($("#datumDO").val() != "" && $("#datumOD").val() == "") {
            alert("Popunite lepo polja za datum kako bi pretraga uspela");
            $("#datumOD").focus();
            return false;
        }
        if ($("#cenaOD").val() != "" && $("#cenaDO").val() == "") {
            alert("Popunite lepo polja za cenu kako bi pretraga uspela");
            $("#cenaDO").focus()
            return false;
        }
        else if ($("#cenaDO").val() != "" && $("#cenaOD").val() == "") {
            alert("Popunite lepo polja za cenu kako bi pretraga uspela");
            $("#cenaOD").focus();
            return false;
        }
        if ($("#cenaOD").val() < 0 || $("#cenaDO").val() < 0) {
            alert("Polja za cenu moraju imati pozitivne brojeve");
            return false;
        }
        if ($("#naziv").val() == "" && $("#mestoOdrzavanja").val() == "" && $("#datumOD").val() == "" && $("#cenaOD").val() == "") {
            alert("Popunite bar jedno polje za pretragu");
            return false;
        }
        if ($("#naziv").val() != "") {
            if (!isNaN($("#naziv").val())) {
                alert("Polje za naziv kod pretrage ne sme da bude broj");
                $("#naziv").focus();
                return false;
            }
        }
        if ($("#mestoOdrzavanja").val() != "") {
            if (!isNaN($("#mestoOdrzavanja").val())) {
                alert("Polje za grad/drzavu kod pretrage ne sme da bude broj");
                $("#mestoOdrzavanja").focus();
                return false;
            }
        }


        var pretragaModel = {
            "Naziv": $("#naziv").val(),
            "MestoOdrzavanja": $("#mestoOdrzavanja").val(),
            "DatumOD": $("#datumOD").val(),
            "DatumDO": $("#datumDO").val(),
            "CenaOD": Number.parseInt($("#cenaOD").val()),
            "CenaDO": Number.parseInt($("#cenaDO").val()),
        }
        $.ajax({
            type: "Post",
            url: "/api/default/pretrazi",
            data: pretragaModel,
            success: function (data) {
                pretraga = true;
                var table = kreirajTabelu(data);
                if (table.includes("</div>")) {

                    $(".messi").show();
                    $("#sadrzaj").html(table);
                    rezultatPretrage = data;
                }
                else {
                    $(".messi").hide();
                    $("#sadrzaj").html("<h2>Nema pogodaka</h2>");
                }
                pretraga = false;
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }

        })
    })

    $(document).on("change", "#sortiraj", function () {
        var izbor = $("#sortiraj").val()
        model = {
            "Lista": rezultatPretrage,
            "Izbor": izbor.toString(),
        };
        $.ajax({
            type: "post",
            url: "/api/default/sortiraj",
            data: model,
            success: function (data) {
                var table = kreirajTabelu(data);
                if (table.includes("</div>")) {
                    
                    $("#sadrzaj").html(table);
                }
                else
                    $("#sadrzaj").html("<h2>Nema pogodaka</h2>");
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })
    })

    $(document).on("change", "#filtriraj", function () {

        var izbor = $("#filtriraj").val();
        model = {
            "Lista": rezultatPretrage,
            "Izbor": izbor.toString(),
        };

        $.ajax({
            type: "post",
            url: "/api/default/filtriraj",
            data: model,
            success: function (data) {
                var table = kreirajTabelu(data);
                if (table.includes("</div>")) {
                    
                    $("#sadrzaj").html(table);
                }
                else
                    $("#sadrzaj").html("<h2>Nema pogodaka</h2>");
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }

        })

    })
    //var user = localStorage.getItem("user");
    //ide ajax poziv da vrati korisnika iz sesije
    var user;
    $.ajax({
        type: "get",
        url: "/api/korisnik/vratikorisnika",
        success: function (data) {
            user = data;
            if (user == "null" || user == "" || user == undefined) {
                $("#profilKorisnika").hide();
                $("#dugmeOdjava").hide();
                $("#adminpanel").hide();
                $("#prodavacindex").hide();
            }
            else {
                $("#profilKorisnika").show();
                $("#dugmeOdjava").show();
                $("#prijava").hide();
                $("#registracija").hide();
                if (string_of_enum(Uloga, data.Uloga) == "Administrator") {

                    $("#adminpanel").show();
                    $("#prodavacindex").hide();
                }
                else if (string_of_enum(Uloga, data.Uloga) == "Prodavac") {
                    $("#adminpanel").hide();
                    $("#prodavacindex").show();
                }
                else {
                    $("#adminpanel").hide();
                    $("#prodavacindex").hide();
                }
            }
            sessionStorage.setItem("user", JSON.stringify(user));
            console.log(JSON.parse(sessionStorage.getItem("user")));
        },
        error: function (error) {
            var greska = JSON.parse(error.responseText);
            $("#greska").show();
            $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
        }
    })


    $("#profilKorisnika").click(function () {
        window.location.href = "ProfilKorisnika.html";
    })

    $("#dugmeOdjava").click(function () {
        $("#greska").hide();
        $.ajax({
            type: "get",
            url: "/api/default/odjava",
            success: function (data) {
                window.location.href = data;
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
