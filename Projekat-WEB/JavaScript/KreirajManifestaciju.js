
$(document).ready(function () {

    var Uloga = {
        Kupac: 0,
        Prodavac: 1,
        Administrator: 2
    }

    function string_of_enum(TipManifestacije, value) {
        for (var k in TipManifestacije) {
            if (TipManifestacije[k] == value) { return k; }
        }
        return null;
    }


    var user = JSON.parse(sessionStorage.getItem("user"));
    if (user != null && string_of_enum(Uloga, user.Uloga) == "Prodavac") {

        $("#promenaslike").hide();
        $("#slicica").hide();

        //var map = new ol.Map({
        //      layers: [
        //          new ol.layer.Tile({
        //              source: new ol.source.OSM()
        //          })
        //      ],
        //      target: 'map',
        //      view: new ol.View({
        //          center: [0, 0],
        //          zoom: 2
        //      })
        //  })

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

        var boolDugmePromenaSlike = false;
        var id = "";
        var putanja = "#";
        var map;
        if (document.location.search.includes("?id=")) {
            id = document.location.search.split("?id=")[1];//.split("&")[0];
            $.ajax({
                type: "post",
                url: "/api/default/manifestacija",
                data: "=" + id,
                success: function (data) {
                    $("#naziv").val(data.Naziv);
                    $("#tip").val(string_of_enum(TipManifestacije, data.Tip_Manifestacije));
                    $("#mestaregular").val(data.BrojRegularnihMesta);
                    $("#mestavip").val(data.BrojVipMesta);
                    $("#mestafanpit").val(data.BrojFanPitMesta);
                    $("#datumvreme").val(data.Datum_i_Vreme_Odrzavanja);
                    $("#cena").val(data.Cena_Regularne_Karte);
                    //$("#slika").val(data.Putanja_do_slike);
                    $("#ulicabroj").val(data.Lokacija.Mesto_Odrzavanja.Ulica);
                    $("#broj").val(data.Lokacija.Mesto_Odrzavanja.Broj);
                    $("#grad").val(data.Lokacija.Mesto_Odrzavanja.Grad);
                    $("#postanskibroj").val(data.Lokacija.Mesto_Odrzavanja.Postanski_Broj);
                    $("#sirina").val(data.Lokacija.Geografska_Sirina);
                    $("#duzina").val(data.Lokacija.Geografska_Duzina);
                    putanja = data.Putanja_do_slike;
                    $("#promenaslike").show();
                    $("#divslika").hide();
                    $("#kreiranjenaslov").hide();
                    $("#izmeninaslov").show();
                    $("#btn").hide();
                    $("#btnizmeni").show();
                    var sirina = data.Lokacija.Geografska_Sirina;
                    var duzina = data.Lokacija.Geografska_Duzina;
                },
                error: function (error) {
                    alert(error.responseText);
                }
            })


        }

        function Validacija() {
            if ($("#naziv").val().length < 4) {
                alert("Naziv manifestacija mora imati najmanje 4 karaktera");
                $("#naziv").focus();
                return false;
            }
            if (!isNaN($("#naziv").val())) {
                alert("Naziv manifestacije ne moze da bude broj");
                $("#naziv").focus();
                return false;
            }
            if ($("#mestaregular").val() == "") {
                alert("Unesite broj regular mesta");
                $("#mestaregular").focus();
                return false;
            }
            if ($("#mestaregular").val() < 0) {
                alert("Broj regularnih mesta mora biti pozitivan broj");
                $("#mestaregular").focus();
                return false;
            }
            if ($("#mestavip").val() == "") {
                alert("Unesite broj vip mesta");
                $("#mestavip").focus();
                return false;
            }
            if ($("#mestavip").val() < 0) {
                alert("Broj vip mesta mora biti pozitivan broj");
                $("#mestavip").focus();
                return false;
            }
            if ($("#mestafanpit").val() == "") {
                alert("Unesite broj fan pit mesta");
                $("#mestafanpit").focus();
                return false;
            }
            if ($("#mestafanpit").val() < 0) {
                alert("Broj fan pit mesta mora biti pozitivan broj");
                $("#mestafanpit").focus();
                return false;
            }
            if (parseInt($("#mestaregular").val() + $("#mestavip").val() + $("#mestafanpit").val()) == 0) {
                alert("Ne mozete kreirati manifestaciju bez mesta za rezervaciju");
                return false;
            }
            if ($("#datumvreme").val() == "") {
                alert("Izaberite datum i vreme");
                $("#datumvreme").focus();
                return false;
            }
            if ($("#cena").val() == "") {
                alert("Unesite cenu regulrane karte");
                $("#cena").focus();
                return false;
            }
            if ($("#cena").val() < 0) {
                alert("Cena regularne karte mora biti pozitivan broj");
                $("#cena").focus();
                return false;
            }
            if ($("#slika").val() == "") {
                alert("Izaberite sliku");
                $("#slika").focus();
                return false;
            }
            if ($("#sirina").val() == "") {
                alert("Izaberite lokaciju na mapi");
                $("#map").focus();
                return false;
            }
            if ($("#duzina").val() == "") {
                alert("Izaberite lokaciju na mapi");
                $("#map").focus();
                return false;
            }
            if ($("#ulicabroj").val().length < 3) {
                alert("Naziv ulice mora imati najmanje 3 karaktera");
                $("#ulicabroj").focus();
                return false;
            }
            if (!isNaN($("#ulicabroj").val())) {
                alert("Naziv ulice ne moze da bude broj");
                $("#ulicabroj").focus();
                return false;
            }
            if ($("#broj").val() == "") {
                alert("Unesite broj ulice");
                $("#broj").focus();
                return false;
            }
            var filter = /^[0-9]+[A-Za-z]{0,2}$/;
            if (!filter.test($("#broj").val())) {
                alert("Broj ulice nije validan.");
                $("#broj").focus();
                return false;
            }
            if ($("#grad").val().length < 3) {
                alert("Naziv grada mora imati najmanje 3 karaktera");
                $("#grad").focus();
                return false;
            }
            if (!isNaN($("#grad").val())) {
                alert("Naziv grada ne moze da bude broj");
                $("#grad").focus();
                return false;
            }
            if ($("#postanskibroj").val() == "") {
                alert("Unesite postanski broj grada");
                $("#postanskibroj").focus();
                return false;
            }
            if ($("#postanskibroj").val() < 0) {
                alert("Postanski broj grada mora biti pozitivan broj");
                $("#postanskibroj").focus();
                return false;
            }
            if ($("#postanskibroj").val() < 1000 || $("#postanskibroj").val() > 99999) {
                alert("Postanski broj mora imati 4 ili 5 cifara");
                $("#postanskibroj").focus();
                return false;
            }

            return true;
        }

        function ValidacijaZaIzmenuManifestacije() {
            if ($("#naziv").val().length < 4) {
                alert("Naziv manifestacija mora imati najmanje 4 karaktera");
                $("#naziv").focus();
                return false;
            }
            if (!isNaN($("#naziv").val())) {
                alert("Naziv manifestacije ne moze da bude broj");
                $("#naziv").focus();
                return false;
            }
            if ($("#mestaregular").val() == "") {
                alert("Unesite broj regular mesta");
                $("#mestaregular").focus();
                return false;
            }
            if ($("#mestaregular").val() < 0) {
                alert("Broj regularnih mesta mora biti pozitivan broj");
                $("#mestaregular").focus();
                return false;
            }
            if ($("#mestavip").val() == "") {
                alert("Unesite broj vip mesta");
                $("#mestavip").focus();
                return false;
            }
            if ($("#mestavip").val() < 0) {
                alert("Broj vip mesta mora biti pozitivan broj");
                $("#mestavip").focus();
                return false;
            }
            if ($("#mestafanpit").val() == "") {
                alert("Unesite broj fan pit mesta");
                $("#mestafanpit").focus();
                return false;
            }
            if ($("#mestafanpit").val() < 0) {
                alert("Broj fan pit mesta mora biti pozitivan broj");
                $("#mestafanpit").focus();
                return false;
            }
            if (parseInt($("#mestaregular").val() + $("#mestavip").val() + $("#mestafanpit").val()) == 0) {
                alert("Ne mozete kreirati manifestaciju bez mesta za rezervaciju");
                return false;
            }
            if ($("#datumvreme").val() == "") {
                alert("Izaberite datum i vreme");
                $("#datumvreme").focus();
                return false;
            }
            if ($("#cena").val() == "") {
                alert("Unesite cenu regulrane karte");
                $("#cena").focus();
                return false;
            }
            if ($("#cena").val() < 0) {
                alert("Cena regularne karte mora biti pozitivan broj");
                $("#cena").focus();
                return false;
            }
            if (boolDugmePromenaSlike == true) {
                if ($("#slika").val() == "") {
                    alert("Izaberite sliku");
                    $("#slika").focus();
                    return false;
                }
            }
            if ($("#sirina").val() == "") {
                alert("Izaberite lokaciju na mapi");
                $("#map").focus();
                return false;
            }
            if ($("#duzina").val() == "") {
                alert("Izaberite lokaciju na mapi");
                $("#map").focus();
                return false;
            }
            if ($("#ulicabroj").val().length < 3) {
                alert("Naziv ulice mora imati najmanje 3 karaktera");
                $("#ulicabroj").focus();
                return false;
            }
            if (!isNaN($("#ulicabroj").val())) {
                alert("Naziv ulice ne moze da bude broj");
                $("#ulicabroj").focus();
                return false;
            }
            if ($("#broj").val() == "") {
                alert("Unesite broj ulice");
                $("#broj").focus();
                return false;
            }
            var filter = /^[0-9]+[A-Za-z]{0,2}$/;
            if (!filter.test($("#broj").val())) {
                alert("Broj ulice nije validan.");
                $("#broj").focus();
                return false;
            }
            if ($("#grad").val().length < 3) {
                alert("Naziv grada mora imati najmanje 3 karaktera");
                $("#grad").focus();
                return false;
            }
            if (!isNaN($("#grad").val())) {
                alert("Naziv grada ne moze da bude broj");
                $("#grad").focus();
                return false;
            }
            if ($("#postanskibroj").val() == "") {
                alert("Unesite postanski broj grada");
                $("#postanskibroj").focus();
                return false;
            }
            if ($("#postanskibroj").val() < 0) {
                alert("Postanski broj grada mora biti pozitivan broj");
                $("#postanskibroj").focus();
                return false;
            }
            if ($("#postanskibroj").val() < 1000 || $("#postanskibroj").val() > 99999) {
                alert("Postanski broj mora imati 4 ili 5 cifara");
                $("#postanskibroj").focus();
                return false;
            }

            return true;
        }

        function NapraviManifestaciju() {
            //ide parseInt tamo gde su brojevi jer nisam mogao da sprecim da korisnik unese decimalni broj
            // number.isInteger ne radi, jer u html je sve string, svako input polje bez obzira na type je string kad se pokupi sa
            // jquery funkcijom val()
            var manifestacija = {
                "Naziv": $("input[name='Naziv']").val(),
                "Tip_Manifestacije": $("#tip").val(),
                "BrojRegularnihMesta": parseInt($("input[name='BrojRegularnihMesta']").val()),
                "BrojVipMesta": parseInt($("input[name='BrojVipMesta']").val()),
                "BrojFanPitMesta": parseInt($("input[name='BrojFanPitMesta']").val()),
                "Datum_i_Vreme_Odrzavanja": $("input[name='Datum_i_Vreme_Odrzavanja']").val(),
                "Cena_Regularne_Karte": parseInt($("input[name='Cena_Regularne_Karte']").val()),
                "Putanja_do_slike": $("input[name='Putanja_do_slike']").val(),
                "Lokacija": {
                    "Geografska_Duzina": $("#duzina").val(),
                    "Geografska_Sirina": $("#sirina").val(),
                    "Mesto_Odrzavanja": {
                        "Ulica": $("input[name='Ulica_i_Broj']").val(),
                        "Broj": $("input[name='broj']").val(),
                        "Grad": $("input[name='Grad']").val(),
                        "Postanski_Broj": parseInt($("input[name='Postanski_Broj']").val()),
                    }
                }
            }
            return manifestacija;
        }

        $("#btn").click(function () {
            $("#greska").hide();

            //validacija
            if (!Validacija()) { return false; }

            var fajl = new FormData();
            var files = $("#slika").get(0).files;
            if (files.length > 0) {
                fajl.append("Fajl", files[0]);
            }

            var manifestacija = NapraviManifestaciju();

            $.ajax({
                type: "Post",
                url: "/api/prodavac/kreirajmanifestaciju",
                data: manifestacija,
                success: function (data) {
                    if (data != "") {
                        alert(data);
                        return false;
                    }
                    else {
                        $.ajax({
                            type: "post",
                            url: "/api/prodavac/sacuvajsliku",
                            data: fajl,
                            contentType: false,
                            processData: false,
                            success: function (data) {
                                if (data != "") {
                                    alert(data);
                                    return false;
                                }
                                else {
                                    alert("Uspesno kreirana manifestacija");
                                    window.location.href = "/Views/Prodavac/Index.html";
                                }

                            },
                            error: function (error) {
                                var greska = JSON.parse(error.responseText);
                                $("#greska").show();
                                $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                            }
                        })

                    }

                },
                error: function (error) {

                    var greska = JSON.parse(error.responseText);
                    $("#greska").show();
                    $("#greska").html("<p tabindex='1' style='color:red'>" + greska.Message + "</p>");
                    $("[tabindex='1']").focus().css("border", "none").css("outline", "none");
                }

            })

        })

        $("#btnizmeni").click(function () {
            $("#greska").hide();

            //validacija
            if (!ValidacijaZaIzmenuManifestacije()) { return false; }

            var manifestacija = NapraviManifestaciju();

            var fajl = new FormData();
            var files = $("#slika").get(0).files;
            if (files.length > 0) {
                fajl.append("Fajl", files[0]);
            }

            if (boolDugmePromenaSlike == false) {
                manifestacija.Putanja_do_slike = "C:\\fakepath\\" + putanja;
            }


            $.ajax({
                type: "Post",
                url: "/api/prodavac/izmenimanifestaciju",
                data: manifestacija,
                success: function (data) {
                    if (data != "") {
                        alert(data);
                        return false;
                    }
                    else {
                        if (boolDugmePromenaSlike == true) {
                            $.ajax({
                                type: "post",
                                url: "/api/prodavac/sacuvajsliku",
                                data: fajl,
                                contentType: false,
                                processData: false,
                                success: function (data) {
                                    if (data != "") {
                                        alert(data);
                                        return false;
                                    }
                                    else {
                                        alert("Uspesno izmenjena manifestacija");
                                        window.location.href = "/Views/Prodavac/Index.html";
                                    }

                                },
                                error: function (error) {
                                    var greska = JSON.parse(error.responseText);
                                    $("#greska").show();
                                    $("#greska").html("<p style='color:red'>" + greska.Message + "</p>");
                                }
                            })
                        }
                        alert("Uspesno izmenjena manifestacija");
                        window.location.href = "/Views/Prodavac/Index.html";

                    }

                },
                error: function (error) {

                    var greska = JSON.parse(error.responseText);
                    $("#greska").show();
                    $("#greska").html("<p tabindex='1' style='color:red'>" + greska.Message + "</p>");
                    $("[tabindex='1']").focus().css("border", "none").css("outline", "none");
                }

            })

        })

        $("#promenaslike").click(function () {

            if (confirm("Nakon ovoga morate izabrati sliku! Da li ste sigurni u izbor?")) {
                boolDugmePromenaSlike = true;
                $("#divslika").show();
                $("#slicica").show();
                $("#aca").attr("src", "/Images/" + putanja);
                $("#promenaslike").hide();
            }
        })

        var map = new ol.Map({
            layers: [
                new ol.layer.Tile({
                    source: new ol.source.OSM()
                })
            ],
            target: 'map',
            view: new ol.View({
                center: [0, 0],
                zoom: 2
            })
        })
        var brojac = 0;
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

        //var feature = new ol.Feature(new ol.geom.Point(new ol.proj.fromLonLat([0,0])));
        //feature.setStyle(style);

        //var layer = new ol.layer.Vector({
        //    source: new ol.source.Vector({
        //        features: [feature]
        //    })
        //});
        //layer.set('name', 'novi');
        //map.addLayer(layer);

        function ObrisiSlojDodajSloj(sirina, duzina) {

            if (brojac > 1) {
                map.getLayers().forEach(layer => {
                    if (layer.get('name') && layer.get('name') == 'novi') {
                        map.removeLayer(layer);
                    }
                })
            }
            var feature = new ol.Feature(new ol.geom.Point(new ol.proj.fromLonLat([sirina, duzina])));
            feature.setStyle(style);

            var layer = new ol.layer.Vector({
                source: new ol.source.Vector({
                    features: [feature]
                })
            });
            layer.set('name', 'novi');
            map.addLayer(layer);

        }

        function ReverseGeocoding(lon, lat) {
            fetch('http://nominatim.openstreetmap.org/reverse?format=json&lon=' + lon + '&lat=' + lat + '&zoom=18').then(function (response) {
                return response.json();
            }).then(function (json) {
                console.log(json);
                // dodaj u polja za ulicu grad postanski broj g sirinu i g duzinu vrednosti iz json objekta
                $("#ulicabroj").val(json.address.road);
                $("#broj").val(json.address.house_number);
                $("#grad").val(json.address.city);
                $("#postanskibroj").val(json.address.postcode);
            })
        }

        map.on("click", function (e) {
            var koordinate = ol.proj.toLonLat(e.coordinate).map(function (val) {
                return val.toFixed(6);
            });
            // dodaj da polja za
            var lon = koordinate[0];
            var lat = koordinate[1];
            //polja za g sirinu i duzinu neka budu popunjena sa lon i lat
            $("#sirina").val(lat);
            $("#duzina").val(lon);
            brojac++;
            ObrisiSlojDodajSloj(lon, lat);

            ReverseGeocoding(lon, lat);
        });
    }
    else {
        // $("body").html("<img src='/Images/zabrana.png' />");
        window.location.href = "/Views/Pocetna.html";
    }
})
