package nc.grool.clinotag;

import androidx.appcompat.app.AppCompatActivity;

import android.annotation.SuppressLint;
import android.media.AudioManager;
import android.media.ToneGenerator;
import android.nfc.NfcAdapter;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;

import java.util.Date;
import java.util.concurrent.ExecutionException;

import nc.grool.clinotag.dto.Lieu;
import nc.grool.clinotag.dto.Utilisation;
import nc.grool.clinotag.json.JsonTaskIntegerPost;
import nc.grool.clinotag.json.JsonTaskLieu;
import nc.grool.clinotag.json.JsonTaskUtilisationPost;
import nc.grool.clinotag.outil.Format;

public class UtilisationActivity extends AppCompatActivity {

    TextView labDateHeureUtilisation = null;
    TextView labInstruction = null;
    EditText editCommentaire = null;
    static boolean scanEnCours = false;
    NfcAdapter mAdapter;

    @Override
    public void onStart() {
        super.onStart();
        mAdapter = NfcAdapter.getDefaultAdapter(this);

//        if(mAdapter != null)
//        {
//            if(mAdapter.isEnabled())
//                toogleNfc(true);
//        }

        if(mAdapter == null){
        } else {
            if (!mAdapter.isEnabled()) {
            } else {
                toogleNfc(true);
            }
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_utilisation);

        Globals.UtilisationEnCours = new Utilisation();
        Globals.UtilisationEnCours.dateDebut = new Date();
        Globals.UtilisationEnCours.dhDebut = String.valueOf(Globals.UtilisationEnCours.dateDebut.getTime());
        Globals.UtilisationEnCours.idAgent = Globals.cetAgent.idAgent;
        Globals.UtilisationEnCours.idMateriel  = Globals.MaterialInProgress.idMateriel;

        Globals g = (Globals)getApplication();
        if(g.isNetworkConnected()){
            try {
                Utilisation uneU;
                uneU = new JsonTaskUtilisationPost().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR,
                        Globals.urlAPIClinoTag + "NouvelleUtilisation",
                        new Gson().toJson(Globals.UtilisationEnCours)).get();
                if(uneU == null && uneU.idUtilisation > 0){
                    Toast.makeText(getApplicationContext(), "Error saving usage", Toast.LENGTH_LONG).show();
                    finish();
                    return;
                } else {
                    Globals.UtilisationEnCours = uneU;
                }
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }

            Globals.UtilisationEnCours.dateDebut = Format.DateMilliEpoch(Globals.UtilisationEnCours.dhDebut);
            labDateHeureUtilisation = (TextView) findViewById(R.id.labDateHeureUtilisation);
            labDateHeureUtilisation.setText("Beginning : " + Format.ShowShortDateTime(Globals.UtilisationEnCours.dateDebut) + " either " + Format.DifferenceMinute(Globals.UtilisationEnCours.dateDebut, new Date()) + " min.");

            labInstruction = (TextView) findViewById(R.id.labInstructions);
            String sInstruction = "aucune";

            if(Globals.MaterialInProgress.instruction != null)
                sInstruction = Globals.MaterialInProgress.instruction;

            labInstruction.setText("Instruction(s) : " + sInstruction);
            editCommentaire = (EditText) findViewById(R.id.editCommentaireUtilisation);

            setTitle(Globals.getCurrentTime() + " - " + Globals.MaterialInProgress.client + "/" + Globals.MaterialInProgress.nom);
            new CountDownTimer(5000, 300) {

                public void onTick(long millisUntilFinished) {}

                public void onFinish() {
                    setTitle(Globals.getCurrentTime() + " - " + Globals.MaterialInProgress.client + "/" + Globals.MaterialInProgress.nom);
                    labDateHeureUtilisation.setText("Beginning : " + Format.ShowShortDateTime(Globals.UtilisationEnCours.dateDebut) + " either " + Format.DifferenceMinute(Globals.UtilisationEnCours.dateDebut, new Date()) + " min.");
                    this.start();
                }
            }.start();
        } else {
            Toast.makeText(getApplicationContext(), "No Internet connection", Toast.LENGTH_LONG).show();
            finish();
        }
    }

    static String hexTagId;

    public void onClickImgNfc(View v) {
        if(!scanEnCours) {
            ReadingTag(hexTagId); // BQS Car
        }
    }

    void toogleNfc(Boolean enable){

        if (enable) {
            final ToneGenerator tg = new ToneGenerator(AudioManager.STREAM_MUSIC, ToneGenerator.MAX_VOLUME);

            mAdapter.enableReaderMode(this, tag -> runOnUiThread(() -> {
                if(!scanEnCours){
                    hexTagId = Format.bytesToHexString(tag.getId()).substring(2).toUpperCase();
                    Toast.makeText(getApplicationContext(), hexTagId, Toast.LENGTH_SHORT).show();
//                  tg.startTone(ToneGenerator.TONE_CDMA_SOFT_ERROR_LITE,200);
//                  tg.startTone(ToneGenerator.TONE_CDMA_PIP,200);
                    tg.startTone(ToneGenerator.TONE_CDMA_ALERT_CALL_GUARD,200);
                    ReadingTag(hexTagId);
                }
            }), NfcAdapter.FLAG_READER_NFC_A |
                    NfcAdapter.FLAG_READER_NFC_B |
                    NfcAdapter.FLAG_READER_NFC_F |
                    NfcAdapter.FLAG_READER_NFC_V |
                    NfcAdapter.FLAG_READER_NFC_BARCODE |
                    NfcAdapter.FLAG_READER_NO_PLATFORM_SOUNDS, null);
        } else {
            mAdapter.disableReaderMode(this);
        }
    }

    private void ReadingTag(String hexTagId) {
        try {
            scanEnCours = true;
            if(hexTagId.equals(Globals.MaterialInProgress.uidTag)){

                String req = Globals.urlAPIClinoTag + "ScanLieu/" + hexTagId ;
                Lieu rLieu = new JsonTaskLieu().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

                if(rLieu.progress == 1)
                    Globals.isWorking = true;
                else if(rLieu.progress == 2)
                    Globals.isWorking = false;

                new enregistrerUtilisationTask().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
                finish();
            }else{
                Toast.makeText(getApplicationContext(), "The tag does not match the tag of the initial request.", Toast.LENGTH_SHORT).show();
            }
            scanEnCours = false;
        } catch (Exception e) {
            Toast.makeText(getApplicationContext(), "Error reading tag.", Toast.LENGTH_SHORT).show();
        }
    }

    private class enregistrerUtilisationTask extends AsyncTask<Void, Integer, Integer> {

        protected void onPreExecute() {
//            findViewById(R.id.progressBar).setVisibility(View.VISIBLE);
        }

        @SuppressLint("WrongThread")
        protected Integer doInBackground(Void... params) {
            Globals g = (Globals) getApplication();
//            Location location = g.getLocation();
//            if (location != null) {

            if (!g.isNetworkConnected()) return -100;

            Utilisation utilisation = Globals.UtilisationEnCours;
            utilisation.dhDebut = String.valueOf(utilisation.dateDebut.getTime());
            utilisation.dhFin = String.valueOf(new Date().getTime());
            utilisation.commentaire = String.valueOf(editCommentaire.getText());

            Integer result = null;
            try {
                result = new JsonTaskIntegerPost().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR,
                        Globals.urlAPIClinoTag + "UtilisationTerminee",
                        new Gson().toJson(utilisation)).get();
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            if (result != null) return result;

            return -999;
        }

        protected void onPostExecute(Integer result) {
//            findViewById(R.id.progressBar).setVisibility(View.INVISIBLE);
            switch (result) {
                case -100:
                    Toast.makeText(getApplicationContext(), R.string.noconnexion, Toast.LENGTH_SHORT).show();
                    break;
                case -999:
                    Toast.makeText(getApplicationContext(), "Error saving usage.", Toast.LENGTH_SHORT).show();
                    break;
                case -2:
                    Toast.makeText(getApplicationContext(), "No geolocation, recording of use impossible.", Toast.LENGTH_SHORT).show();
                    break;
                case -1:
                    Toast.makeText(getApplicationContext(), "The contactless tag " + hexTagId + " is not that of initial use.", Toast.LENGTH_SHORT).show();
                    break;
                case 0:
                    Toast.makeText(getApplicationContext(), "End of use recorded.", Toast.LENGTH_SHORT).show();
                    finish();
                    break;
            }
            super.onPostExecute(result);
        }
    }
}