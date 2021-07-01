
$(document).ready(function () {






    var TipManifestacije = {
        Koncert: 0,
        Festival: 1,
        Pozoriste: 2,
        Predstava: 3,
    };
    var Uloga = {
        Kupac: 0,
        Prodavac: 1,
        Administrator: 2,
    }

    function string_of_enum(TipManifestacije, value) {
        for (var k in TipManifestacije) {
            if (TipManifestacije[k] == value) { return k; }
        }
        return null;
    }
    //var popust;
    //$.ajax({
    //    type: "get",
    //    url: "/api/korisnik/vratikorisnika",
    //    success: function (data) {
    //        if (data == null) { $("#rezervacija").hide();}
    //        else if (string_of_enum(Uloga, data.Uloga) == "Kupac") {
    //            $("#rezervacija").show();
    //            popust = data.TipKorisnika.Popust
    //            user = data;
    //        }
    //        else {

    //            $("#rezervacija").hide();
    //        }
    //    },
    //    error: function (error) {
    //        alert(error.responseText);
    //    }
    //})


    var id = "";
    var cenaRegularneKarte;
    var ostaleRegular;
    var ostaleVip;
    var ostaleFanPit;
    var popust;

    if (document.location.search.includes("?id=")) {
        id = document.location.search.split("?id=")[1];//.split("&")[0];
        var a = id;

        $.ajax({
            type: "post",
            url: "/api/default/manifestacija",
            data: "=" + id,
            success: function (data) {

                if (data == null) {
                    alert("Manifestacija nije pronadjena");
                    window.location.href = "Pocetna.html";
                }

                $("#slika").attr("src", "/Images/" + data.Putanja_do_slike);
                $("#naziv").val(data.Naziv);
                $("#tip").val(string_of_enum(TipManifestacije, data.Tip_Manifestacije));
                $("#brojmesta").val(data.Broj_Mesta);
                $("#ostaleregular").val(data.BrojRegularnihMesta);
                $("#ostalevip").val(data.BrojVipMesta);
                $("#ostalefanpit").val(data.BrojFanPitMesta);
                var datumVreme = data.Datum_i_Vreme_Odrzavanja.split('T');
                $("#datumvreme").val(datumVreme[0] + "   " + datumVreme[1]);
                $("#cena").val(data.Cena_Regularne_Karte);
                if ((Date.parse(data.Datum_i_Vreme_Odrzavanja) - Date.parse(new Date())) < 0)  //if (data.Status)
                    $("#status").val("Nije aktivna");
                else
                    $("#status").val("Aktivna");
                $("#ulicabroj").val(data.Lokacija.Mesto_Odrzavanja.Ulica + "   " + data.Lokacija.Mesto_Odrzavanja.Broj);
                $("#grad").val(data.Lokacija.Mesto_Odrzavanja.Grad + "  " + data.Lokacija.Mesto_Odrzavanja.Postanski_Broj.toString());
                $("#geografija").val(data.Lokacija.Geografska_Sirina + "   " + data.Lokacija.Geografska_Duzina);

                cenaRegularneKarte = data.Cena_Regularne_Karte;
                ostaleRegular = data.BrojRegularnihMesta;
                ostaleVip = data.BrojVipMesta;
                ostaleFanPit = data.BrojFanPitMesta;
                data.Tip_Manifestacije = string_of_enum(TipManifestacije, data.Tip_Manifestacije);
                var sirina = data.Lokacija.Geografska_Sirina;
                var duzina = data.Lokacija.Geografska_Duzina;

                const centar = ol.proj.transform([duzina, sirina], 'EPSG:4326', 'EPSG:3857');
                var map = new ol.Map({
                    layers: [
                        new ol.layer.Tile({
                            source: new ol.source.OSM()
                        })
                    ],
                    target: 'map',
                    view: new ol.View({
                        center: centar,
                        zoom: 16
                    })
                })
                var stroke = new ol.style.Stroke({ color: 'black', width: 2 });
                var fill = new ol.style.Fill({ color: 'red' });
                var style = new ol.style.Style({
                    image: new ol.style.RegularShape({
                        fill: fill,
                        stroke: stroke,
                        points: 3,
                        radius: 10,
                        rotation: Math.PI / 3,
                        angle: 0
                    })
                })

                var feature = new ol.Feature(new ol.geom.Point(new ol.proj.fromLonLat([duzina, sirina])));
                feature.setStyle(style);

                var layer = new ol.layer.Vector({
                    source: new ol.source.Vector({
                        features: [feature]
                    })
                });
                map.addLayer(layer);


                $.ajax({
                    type: "get",
                    url: "/api/korisnik/vratikorisnika",
                    success: function (user) {
                        $("#greska").hide();
                        if (user == null) {
                            //NEREGISTROVAN KORISNIK
                            $("#rezervacija").hide();
                            $("#unosKomentara").hide();
                            $("#komentari").hide();
                        }
                        else if (string_of_enum(Uloga, user.Uloga) == "Kupac") {
                            popust = user.TipKorisnika.Popust
                            if ((Date.parse(data.Datum_i_Vreme_Odrzavanja) - Date.parse(new Date())) < 0) {
                                if (data.ProsecnaOcena > 0)
                                    $("#komentari").prepend("<p>Prosecna ocena: <b>" + data.ProsecnaOcena + "</b></p>");
                                else
                                    $("#komentari").prepend("<p>Prosecna ocena: <b>-</b></p>");

                                $.ajax({
                                    type: "get",
                                    url: "/api/default/provera",
                                    success: function (data) {
                                        if (data) {
                                            $("#unosKomentara").show();
                                        }
                                        else {
                                            $("#unosKomentara").hide();
                                        }
                                    },
                                    error: function (error) {
                                        var greska = JSON.parse(error.responseText);
                                        $("#greska").show();
                                        $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                                    }
                                })
                                $("#rezervacija").hide();
                                $("#komentari").show();

                                $.ajax({
                                    type: "get",
                                    url: "/api/prodavac/vratikomentare",
                                    success: function (data) {
                                        var kom = "";
                                        for (product in data) {
                                            if (data[product].Odobren && data[product].LogickiObrisan == false) {
                                                kom += "<div class='kupac' > <div class='ko'>" + data[product].Kupac.Ime + "  " + data[product].Kupac.Prezime + "</div>";
                                                kom += "<div class='tekst'>" + data[product].Tekst_komentara + "</div>";
                                                kom += "<div class='broj' >" + data[product].Ocena + "</div> </div>";
                                            }
                                        }

                                        $("#komentari").append(kom);

                                    },
                                    error: function (error) {
                                        var greska = JSON.parse(error.responseText);
                                        $("#greska").show();
                                        $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                                    }
                                })
                            }
                            else {

                                $("#komentari").hide();
                                $("#unosKomentara").hide();
                            }

                        }
                        else {
                            // PRODAVAC I ADMINISTRATRO MOGU DA VIDE KOMENTARE
                            //OVDE VIDE SAMO ODOBRNE, A KOD NJIH NA POCETNIM STRANICAMA VIDE SVE KOMENTARE (ODOBRENE I ONE KOJE NISU)
                            $("#rezervacija").hide();
                            $("#unosKomentara").hide();
                            if ((Date.parse(data.Datum_i_Vreme_Odrzavanja) - Date.parse(new Date())) < 0) {
                                $("#komentari").show();

                                if (data.ProsecnaOcena > 0)
                                    $("#komentari").prepend("<p>Prosecna ocena: <b>" + data.ProsecnaOcena + "</b></p>");
                                else
                                    $("#komentari").prepend("<p>Prosecna ocena: <b>-</b></p>");

                                $.ajax({
                                    type: "get",
                                    url: "/api/prodavac/vratikomentare",
                                    success: function (data) {
                                        var kom = "";
                                        for (product in data) {
                                            if (data[product].Odobren && data[product].LogickiObrisan == false) {
                                                kom += "<div class='kupac' > <div class='ko'>" + data[product].Kupac.Ime + "  " + data[product].Kupac.Prezime + "</div>";
                                                kom += "<div class='tekst'>" + data[product].Tekst_komentara + "</div>";
                                                kom += "<div class='broj' >" + data[product].Ocena + "</div> </div>";
                                            }
                                        }

                                        $("#komentari").append(kom);

                                    },
                                    error: function (error) {
                                        var greska = JSON.parse(error.responseText);
                                        $("#greska").show();
                                        $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                                    }
                                })

                            }
                            else {
                                $("#komentari").hide();
                            }
                        }
                    },
                    error: function (error) {
                        alert(error.responseText);
                    }
                })
            },
            error: function (error) {
                alert(error.responseText);
            }
        })

        $("#dugmerezervacija").click(function () {
            $("#greska").hide();
            var brojKarata = parseInt($("#brojkarata").val());
            //proveri da li je uneto u input polje vrednost i da  li je  vrednost > 0
            if (brojKarata <= 0) {
                alert("Popunite polje za broj karata tako da bude pozitivan broj i veci od nule");
                $("#brojkarata").focus();
                return false;
            }
            if (!Number.isInteger(brojKarata)) {
                alert("Broj karata mora biti ceo broj");
                $("#brojkarata").focus();
                return false;
            }

            var ukupnaCenaSaPopustom;
            var tipKarte = $("#tipkarte").val();

            if (tipKarte == "fan pit") {
                if (brojKarata > ostaleFanPit) {
                    alert("Za izabrani tip nema dovoljno karata. Pogledajte preostali broj karata za izabrani tip karte");
                    return false;
                }
            }
            else if (tipKarte == "vip") {
                if (brojKarata > ostaleVip) {
                    alert("Za izabrani tip nema dovoljno karata. Pogledajte preostali broj karata za izabrani tip karte");
                    return false;
                }
            }
            else if (tipKarte == "regular") {
                if (brojKarata > ostaleRegular) {
                    alert("Za izabrani tip nema dovoljno karata. Pogledajte preostali broj karata za izabrani tip karte");
                    return false;
                }
            }

            var ukupanPopust = (100 - popust) / 100;
            if (tipKarte == "vip") {

                ukupnaCenaSaPopustom = brojKarata * (cenaRegularneKarte * 4 * ukupanPopust);
            }
            else if (tipKarte == "fan pit") {

                ukupnaCenaSaPopustom = brojKarata * (cenaRegularneKarte * 2 * ukupanPopust);
            }
            else if (tipKarte == "regular") {

                ukupnaCenaSaPopustom = brojKarata * (cenaRegularneKarte * ukupanPopust);
            }

            if (confirm("Potvrda rezervacije. Broj karata za rezervaciju: " + brojKarata + " Cena karata: " + ukupnaCenaSaPopustom + ".")) {
                $.ajax({
                    type: "get",
                    url: "/api/korisnik/rezervisi/" + brojKarata + "/" + tipKarte,
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

        })

        $("#dugmekomentar").click(function () {
            $("#greska").hide();
            var komentar = $("#komentar").val();
            var ocena = $("#ocena").val();
            // proveri da li su polja za komentar i ocenu prazna
            if (komentar == "") {
                alert("Molimo Vas napisite komentar u predvidjenom polju");
                $("#komentar").focus();
                return false;
            }
            else if (komentar.trim().length < 10) {
                alert("Komentar mora imati najmanje 10 karaktera");
                $("#komentar").focus();
                return false;
            }
            else {

                var model = {
                    "Tekst_komentara": komentar,
                    "Ocena": ocena,
                }
                $.ajax({
                    type: "post",
                    url: "/api/prodavac/komentar",
                    data: model,
                    success: function (data) {
                        alert(data);
                        console.log("Komentar poslat, ceka odobrenje");
                        window.location.reload();
                    },
                    error: function (error) {
                        var greska = JSON.parse(error.responseText);
                        $("#greska").show();
                        $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                    }
                })
            }
        })
    }
    else {
        window.location.href = "Pocetna.html";
    }

})
