var mapPreview = null, previewZoom = 17, previewMarker = null, maps = [], previewMarkers = [], markers = [];

function loadMapPreview() {
    var geocoder = new google.maps.Geocoder();
    var lat = $("#Latitude").val();
    var lng = $("#Longitude").val();
    var def_lat = $(".map_marker_btn").data("default-latitude");
    var def_lng = $(".map_marker_btn").data("default-longitude");
    var address = $(".map_marker_btn").data("address");
    previewZoom = $(".map_marker_btn").data("zoom");
    previewZoom = previewZoom || 17;
    var default_lat_lng = new google.maps.LatLng(40.1133472, -99.1702885);
    if (lat == "" || lat == undefined || lng == "" || lng == undefined) {
        if (def_lat == "" || def_lng == "" || def_lat == undefined || def_lng == undefined) {
            if (address != undefined) {
                geocoder.geocode({ 'address': address }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        latlng = results[0].geometry.location;
                        showMapPreview(latlng);
                    } else {
                        console.log("setting static");
                        latlng = default_lat_lng;
                        previewZoom = 0;
                        showMapPreview(latlng);
                    }
                });
            } else {
                console.log("setting static");
                latlng = default_lat_lng;
                previewZoom = 4;
                showMapPreview(latlng);
            }
        } else {
            console.log("setting defaults");
            proximity_latlng = new google.maps.LatLng(parseFloat(def_lat), parseFloat(def_lng));
            if (address != undefined) {
                geocoder.geocode({ 'address': address, 'location': proximity_latlng }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        latlng = results[0].geometry.location;
                    } else {
                        console.log("setting default");
                        previewZoom = 10;
                        latlng = proximity_latlng;
                    }
                    showMapPreview(latlng);
                });
            } else {
                console.log("setting default");
                previewZoom = 10;
                latlng = proximity_latlng;
                showMapPreview(latlng);
            }
        }
    }
    else {
        console.log("setting normal");
        latlng = new google.maps.LatLng(parseFloat(lat), parseFloat(lng));
        showMapPreview(latlng);
    }

}

function showMapPreview(latlng) {
    console.log(latlng);
    var mapOptions = {
        zoom: previewZoom,
        center: latlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        disableDefaultUI: true
    }
    mapPreview = new google.maps.Map(document.getElementById("map_preview"), mapOptions);
    maps.push(mapPreview);
    previewMarker = createMarker(mapPreview, latlng);
    console.log(previewMarker);
    previewMarkers.push(previewMarker);
}

function setMapOnAll(m) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].map = m;
    }
}

function deleteMarkers() {
    setMapOnAll(null);
    markers = [];
}

function generateMap() {
    return maps.length && maps[0];
}

function createMarker(theMap, latLng) {
    var m = new google.maps.Marker({
        map: theMap,
        draggable: false,
        animation: google.maps.Animation.DROP,
        position: latLng
    });
    markers.push(m);
    return m;
}

function refreshMapPreview(loadFresh) {
    deleteMarkers();
    var latLong = null,
        mapPreview = mapPreview || generateMap(),
        previewMarker = previewMarker || createMarker(mapPreview, latLong),
        address = $(".map_marker_btn").data("address");
    console.log("address " + address);
  
    if (loadFresh && address) {
        geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                latLong = results[0].geometry.location;
                previewMarker.setPosition(latLong);
                mapPreview.setCenter(latLong);
                previewZoom = 17;
                showMapPreview(latLong);
                google.maps.event.trigger(mapPreview, 'resize');
                $("#Latitude").val(latLong.lat());
                $("#Longitude").val(latLong.lng());
            }
        });
    } else {
        latLong = new google.maps.LatLng(parseFloat($("#Latitude").val()), parseFloat($("#Longitude").val()))
        previewMarker.setPosition(latLong);
        mapPreview.setCenter(latLong);
        google.maps.event.trigger(mapPreview, 'resize');

    }
}

//function printMap() {
//    //console.log("print");
//    //var contents = window.document.getElementById("map");
//    ////console.log(contents.innerHTML);
//    //document.write(contents.innerHTML);
//    //window.print();
//    //window.document.getElementById("map").print();
//    var headstr = "<html><head><title></title></head><body>";
//    var footstr = "</body>";
//    var newstr = document.all.item("map").innerHTML;
//    var oldstr = document.body.innerHTML;
//    document.body.innerHTML = headstr + newstr + footstr;
//    window.print();
//    document.body.innerHTML = oldstr;
//    return false;
//}