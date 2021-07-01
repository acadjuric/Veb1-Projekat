
$(document).ready(function () {
    $("#sortiranje_i_filtriranje").hide();
    var user = JSON.parse(sessionStorage.getItem("user"));
    if (user != null) {

        var StatusKarte = {
            Rezervisana: 0,
            Odustanak: 1,
        };
        var TipKarte = {
            VIP: 0,
            Regular: 1,
            FanPit: 2,
        };
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
        function string_of_enum(Pol, value) {
            for (var k in Pol) {
                if (Pol[k] == value) { return k; }
            }
            return null;
        }
        var uloga = "";
        function kreirajTabelu(data) {
            let table = "<table id='tabelaKarte'>"
            table += "<th>ID karte </th><th> Naziv manifestacije</th><th>Tip manifestacije</th><th>Datum i vreme odrzavanja</th><th> Ime kupca</th><th>Prezime kupca</th><th>Status karte</th><th>Tip karte</th><th>Cena karte</th><th>Ulica i broj" +
                "</th><th>Grad i postanski broj</th>";
            if (uloga == "Kupac") {
                for (product in data) {
                    if (!data[product].LogickiObrisan) {
                        var datumVreme = data[product].Datum_i_Vreme_Manifestacije.split('T');

                        table += "<tr><td class=\"id\">" + data[product].ID_Karte + "</td><td class=\"naziv\">" + data[product].Manifestacija.Naziv + "</td><td>" + string_of_enum(TipManifestacije, data[product].Manifestacija.Tip_Manifestacije)
                            + "</td><td class=\"vreme\">" + datumVreme[0] + "   " + datumVreme[1] + "</td> <td>" + data[product].Kupac.Ime + "</td> <td>" + data[product].Kupac.Prezime + "</td> <td>" +
                            string_of_enum(StatusKarte, data[product].Status_Karte) + "</td> <td>" + string_of_enum(TipKarte, data[product].TipKarte) + "</td> <td>" + data[product].Cena + "</td> <td>" + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica + " " + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Broj +
                            "</td> <td>" + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Grad + ", " + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj + "</td>";

                        if ((Date.parse(data[product].Datum_i_Vreme_Manifestacije) - Date.parse(new Date())) < 0) {

                            table += "<td style='color:#fff'> - </td></tr>";
                        }
                        else {
                            table += "<td><button id=\"otkazikartu\"> Otkazi kartu </button></td></tr>";
                        }


                    }
                }
                table += "</table>";

            }
            else {
                for (product in data) {
                    if (!data[product].LogickiObrisan) {
                        var datumVreme = data[product].Datum_i_Vreme_Manifestacije.split('T');

                        table += "<tr><td>" + data[product].ID_Karte + "</td><td>" + data[product].Manifestacija.Naziv + "</td><td>" + string_of_enum(TipManifestacije, data[product].Manifestacija.Tip_Manifestacije)
                            + "</td><td>" + datumVreme[0] + "   " + datumVreme[1] + "</td><td>" + data[product].Kupac.Ime + "</td><td>" + data[product].Kupac.Prezime + "</td><td>" +
                            string_of_enum(StatusKarte, data[product].Status_Karte) + "</td><td>" + string_of_enum(TipKarte, data[product].TipKarte) + "</td><td>" + data[product].Cena + "</td><td>" + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica + " " + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Broj +
                            "</td><td>" + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Grad + ", " + data[product].Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj + "</td></tr>";
                    }
                }
                table += "</table>";
            }
            return table;
        }

        $(document).on("click", "#otkazikartu", function () {
            $("#greska").hide();
            var id = $(this).closest("tr").find(".id").text();
            var naziv = $(this).closest("tr").find(".naziv").text();
            var vreme = $(this).closest("tr").find(".vreme").text();

            var identifikator = id.toString() + ";" + naziv.toString() + ";" + vreme.toString();

            console.log(identifikator);
            $.ajax({
                type: "post",
                url: "/api/korisnik/otkazikartu",
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


        var rezultatPretrage;

        //var user = "";
        //if (document.location.search.includes("?user=")) {
        //  id = document.location.search.split("?user=")[1].split("&")[0];
        $.ajax({
            type: "get",
            url: "/api/default/profil/" + user.Korisnicko_Ime,
            success: function (data) {
                $("#greska").hide();
                $("#username").val(data.Korisnicko_Ime);
                $("#sifra").val(data.Lozinka);
                $("#ime").val(data.Ime);
                $("#prezime").val(data.Prezime);
                $("#pol").val(string_of_enum(Pol, data.Pol));
                $("#Datum_Rodjenja").val(data.Datum_Rodjenja);
                $("#brojBodova").val(data.Broj_Sakupljenih_Bodova.toFixed(2));
                $("#tipkorisnika").val(string_of_enum(Tip, data.TipKorisnika.Ime_Tipa_Korisnika));
                $("#popust").val(data.TipKorisnika.Popust);
                uloga = string_of_enum(Uloga, data.Uloga);
            },
            error: function (error) {
                var greska = JSON.parse(error.responseText);
                $("#greska").show();
                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
            }
        })
        //}

        $("#dugmeizmeni").click(function () {
            $("#greska").hide();
            //validacija
            if ($("#username").val().length < 3) {
                alert("Korisnicko ime mora imati vise od 3 karaktera");
                $("#username").focus();
                return false;
            }
            else if (!isNaN($("#username").val())) {
                alert("Polje za korisnicko ime ne moze da ima samo brojeve.")
                $("#username").focus();
                return false;
            }
            else if ($("#sifra").val().length < 5) {
                alert("Sifra mora imati najmanje 5 karaktera");
                $("#sifra").focus();
                return false;
            }
            else if ($("#ime").val().length < 3) {
                alert("Ime ne moze imati manje od 3 karaktera");
                $("#ime").focus();
                return false;
            }
            else if ($("#prezime").val().length < 4) {
                alert("Prezime ne moze imati manje od 4 karaktera");
                $("#prezime").focus();
                return false;
            }
            else if (!isNaN($("#prezime").val())) {
                alert("Polje za prezime ne moze imati brojeve,iskljucivo slova.");
                $("#prezime").focus();
                return false;
            }
            else if (!isNaN($("#ime").val())) {
                alert("Polje za ime ne moze imati brojeve,iskljucivo slova.");
                $("#ime").focus();
                return false;
            }
            else {
                var korisnik = {
                    "Korisnicko_Ime": $("#username").val(),
                    "Lozinka": $("#sifra").val(),
                    "Ime": $("#ime").val(),
                    "Prezime": $("#prezime").val(),
                    "Pol": $("#pol").val(),
                    "Datum_Rodjenja": $("#Datum_Rodjenja").val(),
                }

                if (uloga == "Kupac" || uloga == "Administrator") {
                    $.ajax({
                        type: "post",
                        url: "/api/korisnik",
                        data: korisnik,
                        success: function (data) {
                            alert(data);
                            window.location.reload();
                        },
                        error: function (error) {
                            var greska = JSON.parse(error.responseText);
                            $("#greska").show();
                            $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                        }

                    })
                }
                else if (uloga == "Prodavac") {
                    $.ajax({
                        type: "post",
                        url: "/api/admin/kreirajprodavca",
                        data: korisnik,
                        success: function (data) {
                            alert(data);
                            window.location.reload();
                        },
                        error: function (error) {
                            var greska = JSON.parse(error.responseText);
                            $("#greska").show();
                            $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                        }

                    })
                }
            }

        })

        $("#dugmeKarte").click(function () {
            $("#greska").hide();
            $("table").hide();
            $.ajax({
                type: "get",
                url: "/api/korisnik/karte",
                success: function (data) {
                    var table = kreirajTabelu(data);
                    if (table.includes("<tr>")) {
                        rezultatPretrage = data;

                        $("#sortiranje_i_filtriranje").show();
                        $("#naslovSortirajFiltriraj").show();
                        $("#sadrzajKarte").show();
                        $("#sadrzajKarte").html(table);
                    }
                    else {
                        $("#sadrzajKarte").show();
                        $("#sadrzajKarte").html("<h3>Korisnik nema karte</h3>");
                    }

                },
                error: function (error) {
                    var greska = JSON.parse(error.responseText);
                    $("#greska").show();
                    $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                }
            })
        })

        $("#btnPretraga").click(function () {
            $("#greska").hide();
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
            if ($("#naziv").val() == "" && $("#datumOD").val() == "" && $("#cenaOD").val() == "") {
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


            var pretragaModel = {
                "Naziv": $("#naziv").val(),
                "DatumOD": $("#datumOD").val(),
                "DatumDO": $("#datumDO").val(),
                "CenaOD": Number.parseInt($("#cenaOD").val()),
                "CenaDO": Number.parseInt($("#cenaDO").val()),
            }
            $.ajax({
                type: "Post",
                url: "/api/korisnik/pretrazi",
                data: pretragaModel,
                success: function (data) {
                    var table = kreirajTabelu(data);
                    if (table.includes("<tr>")) {
                        rezultatPretrage = data;

                        $("table").hide();
                        $("#sadrzajKarte").show();
                        $("#sortiranje_i_filtriranje").show();
                        $("#naslovSortirajFiltriraj").show();

                        $("#sadrzajKarte").html(table);
                    }
                    else {
                        $("#sadrzajKarte").show();
                        $("#sadrzajKarte").html("<h3>Nema pogodaka</h3>");
                    }

                },
                error: function (error) {
                    var greska = JSON.parse(error.responseText);
                    $("#greska").show();
                    $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                }

            })
        })

        $(document).on("change", "#nacinsortiranja ,#sortiraj", function () {
            $("#greska").hide();
            var izbor = $("#sortiraj").val();
            var nacin = $("#nacinsortiranja").val();
            model = {
                "Lista": rezultatPretrage,
                "Izbor": izbor,
                "Nacin": nacin,
            };
            $.ajax({
                type: "post",
                data: model,
                url: "/api/korisnik/sortiraj",
                success: function (data) {
                    if (data.length > 0) {
                        var table = kreirajTabelu(data);
                        $("#sadrzajKarte").html(table);
                    }
                    else
                        $("#sadrzajKarte").html("<h3>Nema pogodaka</h3>");
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
                "Lista": rezultatPretrage,
                "Izbor": izbor.toString(),
                "Nacin": "",
            };

            $.ajax({
                type: "post",
                url: "/api/korisnik/filtriraj",
                data: model,
                success: function (data) {
                    if (data.length > 0) {
                        var table = kreirajTabelu(data);
                        $("#sadrzajKarte").html(table);
                    }
                    else
                        $("#sadrzajKarte").html("<h3>Nema pogodaka</h3>");
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

        $("#dugmepodaci").click(function () {
            $("table").show();
            $("#sadrzajKarte").hide();
            $("#greska").hide();
            $("#sortiranje_i_filtriranje").hide();
            $("#naslovSortirajFiltriraj").hide();
            $.ajax({
                type: "get",
                url: "/api/default/profil/" + user.Korisnicko_Ime,
                success: function (data) {
                    $("#greska").hide();
                    $("#username").val(data.Korisnicko_Ime);
                    $("#sifra").val(data.Lozinka);
                    $("#ime").val(data.Ime);
                    $("#prezime").val(data.Prezime);
                    $("#pol").val(string_of_enum(Pol, data.Pol));
                    $("#Datum_Rodjenja").val(data.Datum_Rodjenja);
                    $("#brojBodova").val(data.Broj_Sakupljenih_Bodova.toFixed(2));
                    $("#tipkorisnika").val(string_of_enum(Tip, data.TipKorisnika.Ime_Tipa_Korisnika));
                    $("#popust").val(data.TipKorisnika.Popust);
                    uloga = string_of_enum(Uloga, data.Uloga);
                },
                error: function (error) {
                    var greska = JSON.parse(error.responseText);
                    $("#greska").show();
                    $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                }
            })

        })
    }
    else {
        //$("body").html("<img src='/Images/zabrana.png' />");
        window.location.href = "/Views/Pocetna.html";
    }


})
