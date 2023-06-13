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
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;

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
        setContentView(R.layout.activity_passage);

        Globals.PassageEnCours = new Passage();
        Globals.PassageEnCours.dateDebut = new Date();
        Globals.PassageEnCours.idAgent = Globals.cetAgent.idAgent;
        Globals.PassageEnCours.idLieu  =Globals.LieuEnCours.idLieu;
        Globals.PassageEnCours.lTache = Globals.LieuEnCours.lTache;

        labDateHeurePassage = (TextView) findViewById(R.id.labDateHeurePassage);
        labDateHeurePassage.setText("Début : " + Format.AfficherCourteDateHeure(Globals.PassageEnCours.dateDebut) + " soit " + Format.DifferenceMinute(Globals.PassageEnCours.dateDebut, new Date()) + " min.");

        editCommentaire = (EditText) findViewById(R.id.editCommentaire);

        setTitle(Globals.getCurrentTime() + " - " + Globals.LieuEnCours.client + "/" + Globals.LieuEnCours.nom);
        new CountDownTimer(5000, 300) {

            public void onTick(long millisUntilFinished) {}

            public void onFinish() {
                setTitle(Globals.getCurrentTime() + " - " + Globals.LieuEnCours.client + "/" + Globals.LieuEnCours.nom);
                labDateHeurePassage.setText("Début : " + Format.AfficherCourteDateHeure(Globals.PassageEnCours.dateDebut) + " soit " + Format.DifferenceMinute(Globals.PassageEnCours.dateDebut, new Date()) + " min.");
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
        builder.setMessage("Description de la tâche")
                .setPositiveButton("Ok", dialogClickListener)
                .show();
    }

    void toogleNfc(Boolean enable){
        if(enable){
            final ToneGenerator tg = new ToneGenerator(AudioManager.STREAM_MUSIC, ToneGenerator.MAX_VOLUME);

            mAdapter.enableReaderMode(this, new NfcAdapter.ReaderCallback() {
                @Override
                public void onTagDiscovered(final Tag tag) {
                    runOnUiThread(() -> {
                        if(!scanEnCours){

                            hexTagId = Format.bytesToHexString(tag.getId()).substring(2).toUpperCase();
                            Toast.makeText(getApplicationContext(), hexTagId, Toast.LENGTH_SHORT).show();
//                          tg.startTone(ToneGenerator.TONE_CDMA_SOFT_ERROR_LITE,200);
//                          tg.startTone(ToneGenerator.TONE_CDMA_PIP,200);
                            tg.startTone(ToneGenerator.TONE_CDMA_ALERT_CALL_GUARD,200);
                            ReadingTag(hexTagId);
                        }
                    });

                }
            }, NfcAdapter.FLAG_READER_NFC_A |
                    NfcAdapter.FLAG_READER_NFC_B |
                    NfcAdapter.FLAG_READER_NFC_F |
                    NfcAdapter.FLAG_READER_NFC_V |
                    NfcAdapter.FLAG_READER_NFC_BARCODE |
                    NfcAdapter.FLAG_READER_NO_PLATFORM_SOUNDS, null);
        }else{
            mAdapter.disableReaderMode(this);
        }
    }

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
                Toast.makeText(getApplicationContext(), "Le tag ne correspond pas au tag de la demande initiale.", Toast.LENGTH_SHORT).show();
            }
            scanEnCours = false;
        } catch (Exception e) {
            Toast.makeText(getApplicationContext(), "Erreur lors de la lecture du tag.", Toast.LENGTH_SHORT).show();
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
                    Toast.makeText(getApplicationContext(), R.string.noconnexion, Toast.LENGTH_SHORT).show();
                    break;
                case -999:
                    Toast.makeText(getApplicationContext(), "Erreur lors de l'enregistrement du passage.", Toast.LENGTH_SHORT).show();
                    break;
                case -2:
                    Toast.makeText(getApplicationContext(), "Pas de géolocalisation, enregistrement du passage impossible.", Toast.LENGTH_SHORT).show();
                    break;
                case -1:
                    Toast.makeText(getApplicationContext(), "Le tag sans contact " + hexTagId + " n'est pas celui du passage initial.", Toast.LENGTH_SHORT).show();
                    break;
                case 0:
                    Toast.makeText(getApplicationContext(), "Passage enregistré.", Toast.LENGTH_SHORT).show();
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