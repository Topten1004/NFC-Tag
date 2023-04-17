var zoom;
var centre;
var load = true;

/*** Elements that make up the popup.*/
var container = document.getElementById('popup');
var content = document.getElementById('popup-content');
var closer = document.getElementById('popup-closer');

/** Add a click handler to hide the popup.
 * @return {boolean} Don't follow the href.*/
closer.onclick = function () {
    overlay.setPosition(undefined);
    closer.blur();
    return false;
};

/*** Create an overlay to anchor the popup to the map.*/
var overlay = new ol.Overlay({
    element: container,
    autoPan: true,
    autoPanAnimation: {
        duration: 250
    }
});

function AfficherMarqueurs() {
    if (!load) {
        var dhDeb = new moment($('#dtpDebut')[0].value, 'DD/MM/YYYY HH:mm:SS');
        var dhFin = new moment($('#dtpFin')[0].value, 'DD/MM/YYYY HH:mm:SS');

        if (dhDeb.isValid() && dhFin.isValid() && idCamion !== "0") {
            GeolocCamionsHisto(dhDeb, dhFin);
        } else {
            GeolocCamionsMoment();
        }            
    }
}

var lat;
var lon;
var taille = 0.5;
var view = null;
var vectorSource = null;
var map = null;
//var fromProjection = new ol.Projection("EPSG:4326"); // transform from WGS 1984
//var toProjection = new ol.Projection("EPSG:900913"); // to Spherical Mercator Projection
//var extent = new ol.Bounds(162.2, -24.2, 168.7, -18.1).transform(fromProjection, toProjection);

window.onload = function (e) {
    initOSM();
}

function initOSM() {

    lat = parseFloat($("#geolocLati")[0].value);
    lon = parseFloat($("#geolocLongi")[0].value);
    if (!(lat && lon)) {
        // Coordonnées géo aléatoire
        var lat = -22.27;
        var lon = 166.44;
    }

    view = new ol.View({
        center: ol.proj.fromLonLat([lon, lat]),
        zoom: 16,
        extent: ol.proj.transformExtent([162.2, -24.2, 168.7, -18.1], 'EPSG:4326', 'EPSG:3857')
    });

    map = new ol.Map({
        target: 'basicMap',
        overlays: [overlay],
        layers: [new ol.layer.Tile({source: new ol.source.OSM()})],
        view: view
    });

    var point = new ol.geom.Point(ol.proj.fromLonLat([lon, lat]));
    var pointFeature = new ol.Feature(point);
    var iconStyleFunction = function (resolution) {
        return [new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 1.0],
                src: '/img/marker.pointeur.32x32.png'
            })
        })];
    };

    pointFeature.setStyle(iconStyleFunction);

    var layer = new ol.layer.Vector({
        source: new ol.source.Vector({
            features: [pointFeature]
        })
    });
    map.addLayer(layer);

    var dragInteraction = new ol.interaction.Modify({
        features: new ol.Collection([pointFeature])
    });
    pointFeature.on('change', function () {
        var lonlat = ol.proj.transform(this.getGeometry().getCoordinates(), 'EPSG:3857', 'EPSG:4326');
        $("#geolocLati")[0].value = lonlat[1];
        $("#geolocLongi")[0].value = lonlat[0];

    }, pointFeature);
    map.addInteraction(dragInteraction);
    map.on('singleclick', function (evt) {
        pointFeature.getGeometry().setCoordinates(evt.coordinate);
    });
}

function marqueursGeo() {
            
    var iconC = function() {
        return [new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 1.0],
                src: '/img/marker.pointeur.32x32.png'
            })
        })];
    };

    if (vectorSource !== null) vectorSource.clear();
    vectorSource = new ol.source.Vector();

    var lati = parseFloat($("#geolocLati")[0].value.replace(',', '.'));
    var longi = parseFloat($("#geolocLongi")[0].value.replace(',', '.'));

    var point = new ol.geom.Point(ol.proj.fromLonLat([longi, lati]));
    var pointFeature = new ol.Feature({ geometry: point, name: "Employé" });

    var minMarkerx = 0, minMarkery = 0, maxMarkerx = 0, maxMarkery = 0;

    pointFeature.setStyle(iconC);


    vectorSource.addFeature(pointFeature);

    if (minMarkerx + minMarkery + maxMarkerx + maxMarkery === 0) {
        maxMarkerx = longi;
        minMarkerx = longi;
        maxMarkery = lati;
        minMarkery = lati;
    }
    else {
        if (longi >= maxMarkerx) maxMarkerx = longi;
        if (longi <= minMarkerx) minMarkerx = longi;
        if (lati >= maxMarkery) maxMarkery = lati;
        if (lati <= minMarkery) minMarkery = lati;
    }

    var layer = new ol.layer.Vector({
    source: vectorSource
    });
    map.addLayer(layer);

    var zoomextent = 0.01;
    var extent = ol.proj.transformExtent([minMarkerx - zoomextent, minMarkery - zoomextent, maxMarkerx + zoomextent, maxMarkery + zoomextent], 'EPSG:4326', 'EPSG:3857');
    map.getView().fit(extent, map.getSize());

    
    map.on("moveend", function (e) {
        zoom = map.getView().getZoom();
        centre = map.getView().getCenter();
    });

    if (centre !== undefined) map.getView().setCenter(centre);
    if (zoom !== undefined) map.getView().setZoom(zoom);

    load = false;
}

function wrapLon(value) {
    var worlds = Math.floor((value + 180) / 360);
    return value - worlds * 360;
}

function LongLat(long, lat) {
    return new ol.LonLat(long, lat)
        .transform(
            new ol.Projection("EPSG:4326"), // transform from WGS 1984
            map.getProjectionObject() // to Spherical Mercator Projection
        );
}