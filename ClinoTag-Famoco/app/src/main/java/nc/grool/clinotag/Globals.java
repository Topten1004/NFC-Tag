package nc.grool.clinotag;

import android.app.Application;
import android.content.SharedPreferences;
import android.location.Location;
import android.os.AsyncTask;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.concurrent.ExecutionException;

import nc.grool.clinotag.composant.Connectivity;
import nc.grool.clinotag.composant.NetworkTask;
import nc.grool.clinotag.dto.Agent;
import nc.grool.clinotag.dto.Client;
import nc.grool.clinotag.dto.Lieu;
import nc.grool.clinotag.dto.Materiel;
import nc.grool.clinotag.dto.Passage;
import nc.grool.clinotag.dto.Utilisation;

public class Globals extends Application {

    public static Agent cetAgent = null;
    public static Lieu LocationInProgress = null;
    public static Materiel MaterialInProgress = null;
    public static Passage PassageInProgress = null;
    public static Utilisation UtilisationEnCours = null;
    public static List<Client> listeClient = new ArrayList<>();
    public static List<Lieu> listLieus = new ArrayList<>();
    public static List<String> lstCarteMaitresse = new ArrayList<>();
    public static String idConstructor;
    public static String dns = "bqs-clinotag.square.nc";

    public static String url = "https://demo.clinotag.com";

    public static Boolean trainMode = false;
    public static Boolean isWorking = false;

    public static String urlAPIClinoTag = url + "/api/clinotag/";
    public static Boolean dispoAPI= false;

    private static Location location;
    public static String msgErreur;

    public Location getLocation() {
        return location;
    }
    public void setLocation(Location _location) {
        location = _location;
    }

    public void ecrirePref(String cle, String valeur){
        SharedPreferences sp = getSharedPreferences("ClinoTag_Preference", MODE_PRIVATE);
        SharedPreferences.Editor esp = sp.edit();
        esp.putString(cle, valeur);
        esp.commit();
    }

    public String lirePref(String cle){
        SharedPreferences sp = getSharedPreferences("ClinoTag_Preference", MODE_PRIVATE);
        return sp.getString(cle, null);
    }

    @SuppressWarnings("deprecation")
    public boolean isNetworkConnected() {
        if(Connectivity.isConnected(getApplicationContext())){
            dispoAPI = false;
            try {
                dispoAPI = new NetworkTask().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,dns).get();
//                if(dispoAPI) creerBearer();
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            return true;
        }
        return  false;
    }

    public static String getCurrentTime() {
        DateFormat dateFormat = new SimpleDateFormat("HH:mm");
        Date date = new Date();
        return dateFormat.format(date);
    }
}