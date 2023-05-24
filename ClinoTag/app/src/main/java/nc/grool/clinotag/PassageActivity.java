package nc.grool.clinotag;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.FileProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.annotation.SuppressLint;
import android.content.DialogInterface;
import android.content.Intent;
import android.media.AudioManager;
import android.media.ToneGenerator;
import android.net.Uri;
import android.nfc.NfcAdapter;
import android.nfc.Tag;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Base64;
import android.util.Log;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;
import com.urovo.sdk.rfcard.RFCardHandlerImpl;
import com.urovo.sdk.rfcard.listener.RFSearchListener;
import com.urovo.sdk.utils.BytesUtil;

import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.concurrent.ExecutionException;

import nc.grool.clinotag.composant.RecyclerViewTacheAdapter;
import nc.grool.clinotag.dto.Lieu;
import nc.grool.clinotag.dto.Passage;
import nc.grool.clinotag.json.JsonTaskIntegerPost;
import nc.grool.clinotag.json.JsonTaskLieu;
import nc.grool.clinotag.outil.Format;

public class PassageActivity extends AppCompatActivity {

    TextView labDateHeurePassage = null;
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
        setContentView(R.layout.activity_passage);

        rfReader = RFCardHandlerImpl.getInstance();

        Globals.PassageEnCours = new Passage();
        Globals.PassageEnCours.dateDebut = new Date();
        Globals.PassageEnCours.idAgent = Globals.cetAgent.idAgent;
        Globals.PassageEnCours.idLieu  =Globals.LieuEnCours.idLieu;
        Globals.PassageEnCours.lTache = Globals.LieuEnCours.lTache;

        labDateHeurePassage = (TextView) findViewById(R.id.labDateHeurePassage);
        labDateHeurePassage.setText("Start : " + Format.AfficherCourteDateHeure(Globals.PassageEnCours.dateDebut) + " > " + Format.DifferenceMinute(Globals.PassageEnCours.dateDebut, new Date()) + " min.");

        editCommentaire = (EditText) findViewById(R.id.editCommentaire);

        setTitle(Globals.getCurrentTime() + " - " + Globals.LieuEnCours.client + "/" + Globals.LieuEnCours.nom);
        new CountDownTimer(5000, 300) {

            public void onTick(long millisUntilFinished) {}

            public void onFinish() {
                setTitle(Globals.getCurrentTime() + " - " + Globals.LieuEnCours.client + "/" + Globals.LieuEnCours.nom);
                labDateHeurePassage.setText("Start : " + Format.AfficherCourteDateHeure(Globals.PassageEnCours.dateDebut) + " > " + Format.DifferenceMinute(Globals.PassageEnCours.dateDebut, new Date()) + " min.");
                this.start();
            }
        }.start();

        chargement();

//        ((EditText)findViewById(R.id.editCommentaire)).setOnFocusChangeListener((v, hasFocus) -> {
//            if (!hasFocus) { chargement(); }
//        });
    }

    static String hexTagId;

    public void onClickImgNfc(View v) {
        if(!scanEnCours) {
            ReadingTag(hexTagId); //534E35AF016640
        }
    }

    public void onClickTache(View v) {
        // Affiche la description
        DialogInterface.OnClickListener dialogClickListener = (dialog, which) -> {
            switch (which){
                case DialogInterface.BUTTON_POSITIVE:

            }
        };

        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setMessage("Description of the task")
                .setPositiveButton("Ok", dialogClickListener)
                .show();
    }

//    void toogleNfc(Boolean enable){
//        if(enable){
//            final ToneGenerator tg = new ToneGenerator(AudioManager.STREAM_MUSIC, ToneGenerator.MAX_VOLUME);
//
//            mAdapter.enableReaderMode(this, new NfcAdapter.ReaderCallback() {
//                @Override
//                public void onTagDiscovered(final Tag tag) {
//                    runOnUiThread(() -> {
//                        if(!scanEnCours){
//
//                            hexTagId = Format.bytesToHexString(tag.getId()).substring(2).toUpperCase();
//                            Toast.makeText(getApplicationContext(), hexTagId, Toast.LENGTH_SHORT).show();
////                          tg.startTone(ToneGenerator.TONE_CDMA_SOFT_ERROR_LITE,200);
////                          tg.startTone(ToneGenerator.TONE_CDMA_PIP,200);
//                            tg.startTone(ToneGenerator.TONE_CDMA_ALERT_CALL_GUARD,200);
//                            ReadingTag(hexTagId);
//                        }
//                    });
//
//                }
//            }, NfcAdapter.FLAG_READER_NFC_A |
//                    NfcAdapter.FLAG_READER_NFC_B |
//                    NfcAdapter.FLAG_READER_NFC_F |
//                    NfcAdapter.FLAG_READER_NFC_V |
//                    NfcAdapter.FLAG_READER_NFC_BARCODE |
//                    NfcAdapter.FLAG_READER_NO_PLATFORM_SOUNDS, null);
//        }else{
//            mAdapter.disableReaderMode(this);
//        }
//    }

    private void ReadingTag(String hexTagId) {
        try {
            scanEnCours = true;

            if(hexTagId.equals(Globals.LieuEnCours.uidTag)){
                String req = Globals.urlAPIClinoTag + "ScanLieu/" + hexTagId ;
                Lieu rLieu = new JsonTaskLieu().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

                if(rLieu.progress == 1)
                    Globals.isWorking = true;
                else if(rLieu.progress == 2)
                    Globals.isWorking = false;

                new enregistrerPassageTask().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
                finish();
            }else{
                Toast.makeText(getApplicationContext(), "The tag does not match the original request tag.", Toast.LENGTH_SHORT).show();
            }
            scanEnCours = false;
        } catch (Exception e) {
            Toast.makeText(getApplicationContext(), "Error reading tag.", Toast.LENGTH_SHORT).show();
        }
    }

    private void chargement() {
        RecyclerView recyclerView = findViewById(R.id.recyclerViewTache);
        RecyclerViewTacheAdapter adapter = new RecyclerViewTacheAdapter(Globals.LieuEnCours.lTache, getApplication());
        recyclerView.setAdapter(adapter);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
//        adapter.setOnItemClickListener(onItemClickListener);
    }

    private class enregistrerPassageTask extends AsyncTask<Void, Integer, Integer> {

        protected void onPreExecute(){
//            findViewById(R.id.progressBar).setVisibility(View.VISIBLE);
        }

        @SuppressLint("WrongThread")
        protected Integer doInBackground(Void... params) {
            Globals g = (Globals)getApplication();
//            Location location = g.getLocation();
//            if (location != null) {

                if(!g.isNetworkConnected()) return -100;

                Passage passage = Globals.PassageEnCours;
                passage.dhDebut = String.valueOf(passage.dateDebut.getTime());
                passage.dhFin = String.valueOf(new Date().getTime());
                passage.commentaire = String.valueOf(editCommentaire.getText());

                if(currentPhotoPath != null){
                    File photoFile = new File(currentPhotoPath);
                    if(photoFile.length() > 0){
                        int size = (int) photoFile.length();
                        byte[] bytes = new byte[size];
                        try {
                            BufferedInputStream buf = new BufferedInputStream(new FileInputStream(photoFile));
                            buf.read(bytes, 0, bytes.length);
                            buf.close();
                        } catch (FileNotFoundException e) {
                            e.printStackTrace();
                        } catch (IOException e) {
                            e.printStackTrace();
                        }
                        passage.photo = Base64.encodeToString(bytes, Base64.DEFAULT);
                    }else{ passage.photo = ""; }
                }else{ passage.photo = ""; }

//                passage.lTache = new ArrayList<>();
//                for (Tache uneTache: Globals.LieuEnCours.lTache
//                     ) {
//                    uneTache.fait = true;
//                    passage.lTache.add(uneTache);
//                }

                Integer result = null;
                try {
                    result = new JsonTaskIntegerPost().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,
                            Globals.urlAPIClinoTag + "PassageEffectue",
                            new Gson().toJson(passage)).get();
                } catch (ExecutionException e) {
                    e.printStackTrace();
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
                if (result != null) return result;
//            } else {
//                return -2;
//            }
            return -999;
        }

        protected void onPostExecute(Integer result) {
//            findViewById(R.id.progressBar).setVisibility(View.INVISIBLE);
            switch (result){
                case -100:
                    Toast.makeText(getApplicationContext(), R.string.noconnection, Toast.LENGTH_SHORT).show();
                    break;
                case -999:
                    Toast.makeText(getApplicationContext(), "Error while recording passage.", Toast.LENGTH_SHORT).show();
                    break;
                case -2:
                    Toast.makeText(getApplicationContext(), "No geolocation, recording of the passage impossible.", Toast.LENGTH_SHORT).show();
                    break;
                case -1:
                    Toast.makeText(getApplicationContext(), "The contactless tag " + hexTagId + " is not that of the initial passage.", Toast.LENGTH_SHORT).show();
                    break;
                case 0:
                    Toast.makeText(getApplicationContext(), "Pass recorded.", Toast.LENGTH_SHORT).show();
                    finish();
                    break;
            }
            super.onPostExecute(result);
        }
    }

    String currentPhotoPath;

    private File createImageFile() throws IOException {
        // Create an image file name
        String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
        String imageFileName = "JPEG_" + timeStamp + "_";
        File storageDir = getExternalFilesDir(Environment.DIRECTORY_PICTURES);
        File image = File.createTempFile(
                imageFileName,  /* prefix */
                ".jpg",         /* suffix */
                storageDir      /* directory */
        );

        // Save a file: path for use with ACTION_VIEW intents
        currentPhotoPath = image.getAbsolutePath();
        return image;
    }


    public void onClickCamera(View v) {
        dispatchTakePictureIntent();
    }

    static final int REQUEST_IMAGE_CAPTURE = 1;

    private void dispatchTakePictureIntent() {
        Intent takePictureIntent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
        // Ensure that there's a camera activity to handle the intent
        if (takePictureIntent.resolveActivity(getPackageManager()) != null) {
            // Create the File where the photo should go
            File photoFile = null;
            try {
                photoFile = createImageFile();
            } catch (IOException ex) {
                // Error occurred while creating the File
                String toto = ex.getMessage();
            }
            // Continue only if the File was successfully created
            if (photoFile != null) {
                Uri photoURI = FileProvider.getUriForFile(this,
                        "nc.grool.clinotag.fileprovider",
                        photoFile);
                takePictureIntent.putExtra(MediaStore.EXTRA_OUTPUT, photoURI);
                startActivityForResult(takePictureIntent, REQUEST_IMAGE_CAPTURE);
            }
        }
    }
}