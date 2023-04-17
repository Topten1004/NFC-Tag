var container = document.getElementById('popup');
var content = document.getElementById('popup-content');
var closer = document.getElementById('popup-closer');

closer.onclick = function () {
    overlay.setPosition(undefined);
    closer.blur();
};
var overlay = new ol.Overlay({
    element: container,
    autoPan: true,
    autoPanAnimation: {
        duration: 250
    }
});

var view = null;
var map = null;

function initModalOSM(lat, lon) {

    $(".modalMap").empty();

    if (lat && lon) {

        //var latprev = -22.27;
        //var lonprev = 166.44;

        view = new ol.View({
            center: ol.proj.fromLonLat([lon, lat]),
            zoom: 16,
            extent: ol.proj.transformExtent([162.2, -24.2, 168.7, -18.1], 'EPSG:4326', 'EPSG:3857')
        });

        map = new ol.Map({
            target: 'basicMap',
            overlays: [overlay],
            layers: [new ol.layer.Tile({ source: new ol.source.OSM({ crossOrigin: null }) })],
            view: view
        });

        creerMarqueur({ "lati": lat, "longi": lon }, "Utilisation", '/img/marker.pointeur.32x32.png');
        
        
        //if (idLivreur) {
        //    $.get('/Api/DistriPubli/DistributionLivreurGeoloc/' + idLivreur)
        //        .done(function (response, status, jqxhr) {
        //            $.each(response, function (i, item) {
        //                creerMarqueur(item, item.libelle, '/img/publication_32x32.png');
        //            });
        //        })
        //        .fail(function (jqxhr, status, error) {
        //            $(".alert-danger").show();
        //        });
        //}
    }
}

function creerMarqueur(coordGeo, libelleGeo, iconimg) {

    var point = new ol.geom.Point(ol.proj.fromLonLat([coordGeo.longi, coordGeo.lati]));
    var pointFeature = new ol.Feature(point);
    var iconStyleFunction = function (resolution) {
        return [new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 1.0],
                src: iconimg
            }),
            text: new ol.style.Text({
                text: libelleGeo,
                offsetY: 10,
                scale: 1.2,
                fill: new ol.style.Fill({
                    color: '#000000'
                }),
                stroke: new ol.style.Stroke({
                    color: '#FFFF99',
                    width: 2.3
                })
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

}