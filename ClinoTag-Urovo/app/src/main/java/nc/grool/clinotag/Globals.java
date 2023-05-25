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
    public static Lieu LieuEnCours = null;
    public static Materiel MaterielEnCours = null;
    public static Passage PassageEnCours = null;
    public static Utilisation UtilisationEnCours = null;
    public static List<Client> listeClient = new ArrayList<>();

    public static List<Lieu> listLieus = new ArrayList<>();

    public static List<String> lstCarteMaitresse = new ArrayList<>();
    public static String idConstructeur;

    public static String dns = "bqs-clinotag.square.nc";
    public static String url = "https://demo.clinotag.com";

    public static Boolean isWorking = false;

//  public static String dns = "10.0.2.2";
//  public static String url = "https://10.0.2.2:44328";

//  public static String urlAPIGrool = "https://10.0.2.2:44330/api/grool/";
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

//    public void creerBearer() {
//        try {
//            if (bTok != null) bTok.bAuthentication(urlAPIBqsClinoTag);
//            else {
//                idConstructeur = InfoAppareilKt.getDeviceSerial(getApplicationContext());
//                bTok = new BearerToken.AccessTokenRequest().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR, idConstructeur, "b37d8d68-3fd1-4500-a15d-28f36254olic", urlAPIBqsClinoTag).get();
//                if(bTok.access_token == null)bTok = null;
//            }
//        } catch (ExecutionException e) {
//            e.printStackTrace();
//        } catch (InterruptedException e) {
//            e.printStackTrace();
//        }
//    }

    public static String getCurrentTime() {
//        DateFormat dateFormat = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
        DateFormat dateFormat = new SimpleDateFormat("HH:mm");
        Date date = new Date();
        return dateFormat.format(date);
    }
}