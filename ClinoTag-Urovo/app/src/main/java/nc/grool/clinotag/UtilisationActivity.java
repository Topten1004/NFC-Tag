package nc.grool.clinotag;

import androidx.appcompat.app.AppCompatActivity;

import android.annotation.SuppressLint;
import android.media.AudioManager;
import android.media.ToneGenerator;
import android.nfc.NfcAdapter;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.util.Log;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;
import com.urovo.sdk.rfcard.RFCardHandlerImpl;
import com.urovo.sdk.rfcard.listener.RFSearchListener;
import com.urovo.sdk.utils.BytesUtil;

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
//    NfcAdapter mAdapter;

    public RFCardHandlerImpl rfReader;
    @Override
    public void onStart() {
        super.onStart();
//        mAdapter = NfcAdapter.getDefaultAdapter(this);

        startSearchCard();

//        if(mAdapter != null){
//            toogleNfc(true);
//        }
    }

    public void startSearchCard() {
        try {
            rfReader.searchCard(new RFSearchListener() {
                /**
                 * 检测到卡
                 *
                 * @param cardType - 卡类型
                 *                 <ul>
                 *                 <li>S50_CARD(0x00) - S50卡</li>
                 *                 <li>S70_CARD(0x01) - S70卡</li>
                 *                 <li>PRO_CARD(0x02) - PRO卡</li>
                 *                 <li>S50_PRO_CARD(0x03) - 支持S50驱动与PRO驱动的PRO卡</li>
                 *                 <li>S70_PRO_CARD(0x04) - 支持S70驱动与PRO驱动的PRO卡 </li>
                 *                 <li>CPU_CARD(0x05) - CPU卡</li>
                 *                 </ul>
                 * @param UID
                 */
                @Override
                public void onCardPass(int cardType, byte[] UID) {
//                    outputText("onCardPass, cardType " + cardType);
//                    driver = "CPU";
//                    if (cardType == Constant.CardType.PRO_CARD || cardType == Constant.CardType.S50_PRO_CARD
//                            || cardType == Constant.CardType.S70_PRO_CARD) {
//                        driver = "PRO";
//                    } else if (cardType == Constant.CardType.S50_CARD) {
//                        driver = "S50";
//                    } else if (cardType == Constant.CardType.S70_CARD) {
//                        driver = "S70";
//                    }
//                    outputText("" + driver);
                    hexTagId = BytesUtil.bytes2HexString(UID);
                    Log.d("MainActivity","UID: " + BytesUtil.bytes2HexString(UID));
                }

                @Override
                public void onFail(int error, String message) {
                    Log.d("MainActivity","onFail,error: " + error + ",message: " + message);
                }
            });
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_utilisation);

        rfReader = RFCardHandlerImpl.getInstance();

        Globals.UtilisationEnCours = new Utilisation();
        Globals.UtilisationEnCours.dateDebut = new Date();
        Globals.UtilisationEnCours.dhDebut = String.valueOf(Globals.UtilisationEnCours.dateDebut.getTime());
        Globals.UtilisationEnCours.idAgent = Globals.cetAgent.idAgent;
        Globals.UtilisationEnCours.idMateriel  = Globals.MaterielEnCours.idMateriel;

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
//                    String fU = Fichier.lecture(getApplicationContext(), Globals.UtilisationEnCours.idUtilisation + ".utilisation");
//                    if(fU == null)
//                        Fichier.ecriture(getApplicationContext(),
//                                Globals.UtilisationEnCours.idUtilisation + ".utilisation",
//                                new Gson().toJson(Globals.UtilisationEnCours));
//                    else
//                    {
//                        Type t = new TypeToken<Utilisation>(){}.getType();
//                        Globals.UtilisationEnCours =  new Gson().fromJson(fU, t);
//                    }
                }
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }

            Globals.UtilisationEnCours.dateDebut = Format.DateMilliEpoch(Globals.UtilisationEnCours.dhDebut);
            labDateHeureUtilisation = (TextView) findViewById(R.id.labDateHeureUtilisation);
            labDateHeureUtilisation.setText("Start : " + Format.AfficherCourteDateHeure(Globals.UtilisationEnCours.dateDebut) + " > " + Format.DifferenceMinute(Globals.UtilisationEnCours.dateDebut, new Date()) + " min.");

            labInstruction = (TextView) findViewById(R.id.labInstructions);
            String sInstruction = "none";
            if(Globals.MaterielEnCours.instruction != null) sInstruction = Globals.MaterielEnCours.instruction;
            labInstruction.setText("Instruction(s) : " + sInstruction);

            editCommentaire = (EditText) findViewById(R.id.editCommentaireUtilisation);

            setTitle(Globals.getCurrentTime() + " - " + Globals.MaterielEnCours.client + "/" + Globals.MaterielEnCours.nom);
            new CountDownTimer(5000, 300) {

                public void onTick(long millisUntilFinished) {}

                public void onFinish() {
                    setTitle(Globals.getCurrentTime() + " - " + Globals.MaterielEnCours.client + "/" + Globals.MaterielEnCours.nom);
                    labDateHeureUtilisation.setText("Start : " + Format.AfficherCourteDateHeure(Globals.UtilisationEnCours.dateDebut) + " > " + Format.DifferenceMinute(Globals.UtilisationEnCours.dateDebut, new Date()) + " min.");
                    this.start();
                }
            }.start();
        } else {
            Toast.makeText(getApplicationContext(), "No internet connection", Toast.LENGTH_LONG).show();
            finish();
        }
    }

    static String hexTagId;

    public void onClickImgNfc(View v) {
        if(!scanEnCours) {
            ReadingTag(hexTagId); // Voiture BQS
        }
    }

//    void toogleNfc(Boolean enable){
//        if (enable) {
//            final ToneGenerator tg = new ToneGenerator(AudioManager.STREAM_MUSIC, ToneGenerator.MAX_VOLUME);
//
//            mAdapter.enableReaderMode(this, tag -> runOnUiThread(() -> {
//                if(!scanEnCours){
//                    hexTagId = Format.bytesToHexString(tag.getId()).substring(2).toUpperCase();
//                    Toast.makeText(getApplicationContext(), hexTagId, Toast.LENGTH_SHORT).show();
////                  tg.startTone(ToneGenerator.TONE_CDMA_SOFT_ERROR_LITE,200);
////                  tg.startTone(ToneGenerator.TONE_CDMA_PIP,200);
//                    tg.startTone(ToneGenerator.TONE_CDMA_ALERT_CALL_GUARD,200);
//                    ReadingTag(hexTagId);
//                }
//            }), NfcAdapter.FLAG_READER_NFC_A |
//                    NfcAdapter.FLAG_READER_NFC_B |
//                    NfcAdapter.FLAG_READER_NFC_F |
//                    NfcAdapter.FLAG_READER_NFC_V |
//                    NfcAdapter.FLAG_READER_NFC_BARCODE |
//                    NfcAdapter.FLAG_READER_NO_PLATFORM_SOUNDS, null);
//        } else {
//            mAdapter.disableReaderMode(this);
//        }
//    }

    private void ReadingTag(String hexTagId) {
        try {
            scanEnCours = true;
            if(hexTagId.equals(Globals.MaterielEnCours.uidTag)){

                String req = Globals.urlAPIClinoTag + "ScanLieu/" + hexTagId ;
                Lieu rLieu = new JsonTaskLieu().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

                if(rLieu.progress == 1)
                    Globals.isWorking = true;
                else if(rLieu.progress == 2)
                    Globals.isWorking = false;

                new enregistrerUtilisationTask().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
                finish();
            }else{
                Toast.makeText(getApplicationContext(), "The tag does not match the original request tag.", Toast.LENGTH_SHORT).show();
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
                    Toast.makeText(getApplicationContext(), R.string.noconnection, Toast.LENGTH_SHORT).show();
                    break;
                case -999:
                    Toast.makeText(getApplicationContext(), "Error saving usage.", Toast.LENGTH_SHORT).show();
                    break;
                case -2:
                    Toast.makeText(getApplicationContext(), "No geolocation, recording of use impossible.", Toast.LENGTH_SHORT).show();
                    break;
                case -1:
                    Toast.makeText(getApplicationContext(), "The contactless tag " + hexTagId + " is not that of the initial use.", Toast.LENGTH_SHORT).show();
                    break;
                case 0:
                    Toast.makeText(getApplicationContext(), "End of registered use.", Toast.LENGTH_SHORT).show();
                    finish();
                    break;
            }
            super.onPostExecute(result);
        }
    }
}