package nc.grool.clinotag.composant;

import android.Manifest;
import android.content.Context;
import android.content.pm.PackageManager;
import android.location.Criteria;
import android.location.LocationListener;
import android.location.LocationManager;
import android.location.LocationProvider;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;

import androidx.annotation.RequiresApi;
import androidx.core.app.ActivityCompat;

import java.util.concurrent.ExecutionException;

import nc.grool.clinotag.Globals;
import nc.grool.clinotag.json.JsonTaskBoolean;

public class Location {

    private final Context context;
    public Location(Context _context){
        context = _context;
    }

    LocationManager locationManager = null;

    private String provider;
    public double latitude;
    public double longitude;
    public double altitude;



    LocationListener listenerGPS = new LocationListener() {
//        @RequiresApi(api = Build.VERSION_CODES.M)
        @RequiresApi(api = Build.VERSION_CODES.N)
        @Override
        public void onLocationChanged(android.location.Location location) {

            Globals g = (Globals) context.getApplicationContext();
            //g.verifLivraison(new Intent());
            g.setLocation(location);

            latitude = location.getLatitude();
            longitude = location.getLongitude();
            altitude = location.getAltitude();

            //Distance magasins
//            for (Magasin m :MainActivity.lstMagasin) {
//                if (m.geoloc == null)
//                    m.distance = 1000.0;
//                else
//                    m.distance = m.distance = Matheux.distanceEntreDeuxPoints(m.geoloc.lati, m.geoloc.longi, latitude, longitude);
//            }
//
////            MainActivity.lstMagasin.sort((o1, o2) -> o1.distance.compareTo(o2.distance));
//            Collections.sort(MainActivity.lstMagasin, new Comparator<Magasin>(){
//                public int compare(Magasin m1, Magasin m2) {
//                    return m1.distance.compareTo(m2.distance);
//                }
//            });

            if(g.isNetworkConnected()){
                String req = Globals.urlAPIClinoTag + "GeolocAgent/"
                        + Globals.idConstructeur + "/"
                        + Globals.cetAgent.idAgent + "/"
                        + latitude + "/"
                        + longitude + "/";
                try {
                    Boolean result = new JsonTaskBoolean().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();
                } catch (ExecutionException e) {
                    e.printStackTrace();
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }

        @Override
        public void onProviderDisabled(String fournisseur) {
//            Toast.makeText(getApplicationContext(), fournisseur + " désactivé !", Toast.LENGTH_SHORT).show();
        }

        @Override
        public void onProviderEnabled(String fournisseur) {
//            Toast.makeText(getApplicationContext(), fournisseur + " activé !", Toast.LENGTH_SHORT).show();
        }

        @Override
        public void onStatusChanged(String fournisseur, int status, Bundle extras) {
            switch (status) {
                case LocationProvider.AVAILABLE:
//                    Toast.makeText(getApplicationContext(), fournisseur + " état disponible", Toast.LENGTH_SHORT).show();
                    break;
                case LocationProvider.OUT_OF_SERVICE:
//                    Toast.makeText(getApplicationContext(), fournisseur + " état indisponible", Toast.LENGTH_SHORT).show();
                    break;
                case LocationProvider.TEMPORARILY_UNAVAILABLE:
//                    Toast.makeText(getApplicationContext(), fournisseur + " état temporairement indisponible", Toast.LENGTH_SHORT).show();
                    break;
                default:
//                    Toast.makeText(getApplicationContext(), fournisseur + " état : " + status, Toast.LENGTH_SHORT).show();
            }
        }
    };

    public void initLocation() {

        if (locationManager == null) {
            locationManager = (LocationManager) context.getApplicationContext().getSystemService(Context.LOCATION_SERVICE);
            Criteria criteres = new Criteria();

            // la précision  : (ACCURACY_FINE pour une haute précision ou ACCURACY_COARSE pour une moins bonne précision)
            criteres.setAccuracy(Criteria.ACCURACY_FINE);

            // l'altitude
            criteres.setAltitudeRequired(false);

            // la direction
            criteres.setBearingRequired(false);

            // la vitesse
            criteres.setSpeedRequired(false);

            // la consommation d'énergie demandée
            criteres.setCostAllowed(true);

            //criteres.setPowerRequirement(Criteria.POWER_HIGH);
            criteres.setPowerRequirement(Criteria.POWER_MEDIUM);

            provider = locationManager.getBestProvider(criteres, true);
            Log.d("GPS", "provider : " + provider);
        }

        if (provider != null) {
            // dernière position connue
            if (ActivityCompat.checkSelfPermission(context.getApplicationContext(), Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED && ActivityCompat.checkSelfPermission(context.getApplicationContext(), Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                Log.d("GPS", "no permissions !");
                return;
            }

            android.location.Location localisation = locationManager.getLastKnownLocation(provider);
            if (localisation != null) {
                // on notifie la localisation
                listenerGPS.onLocationChanged(localisation);
            }

            // on configure la mise à jour automatique : au moins 30 mètres et 120 secondes
            locationManager.requestLocationUpdates(provider, 3 * 60 * 1000, 30, listenerGPS);
        }
    }

    public void stopLocation() {
        if (locationManager != null) {
            locationManager.removeUpdates(listenerGPS);
            listenerGPS = null;
        }
    }


}
