package nc.grool.clinotag;

import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import android.content.DialogInterface;
import android.content.Intent;
import android.media.AudioManager;
import android.media.ToneGenerator;
import android.nfc.NfcAdapter;
import android.nfc.Tag;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.material.dialog.MaterialAlertDialogBuilder;
import com.google.gson.Gson;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ExecutionException;

import nc.grool.clinotag.composant.DialogTexte;
import nc.grool.clinotag.composant.HttpsTrustManager;
import nc.grool.clinotag.dto.Agent;
import nc.grool.clinotag.dto.Client;
import nc.grool.clinotag.dto.Lieu;
import nc.grool.clinotag.dto.LieuOuMaterielPost;
import nc.grool.clinotag.dto.Materiel;
import nc.grool.clinotag.dto.QtyPost;
import nc.grool.clinotag.json.JsonTaskAgent;
import nc.grool.clinotag.json.JsonTaskClients;
import nc.grool.clinotag.json.JsonTaskIntegerPost;
import nc.grool.clinotag.json.JsonTaskLieu;
import nc.grool.clinotag.json.JsonTaskMateriel;
import nc.grool.clinotag.json.JsonTaskString;
import nc.grool.clinotag.outil.Format;

public class QtyActivity extends AppCompatActivity {

    TextView labQtyCount;

    TextView labLieuName;
    String QtyCount = "";

    static boolean scanEnCours = false;

    static String hexTagId;
    NfcAdapter mAdapter;
    @RequiresApi(api = Build.VERSION_CODES.O)
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_qty);


        mAdapter = NfcAdapter.getDefaultAdapter(this);
        Globals.idConstructeur = Build.SERIAL.toUpperCase();

        HttpsTrustManager.allowAllSSL();
        labQtyCount = (TextView) findViewById(R.id.InputQtyCount);
        labQtyCount.setText("");

        labLieuName = (TextView) findViewById(R.id.labLieuName);
        labLieuName.setText(Globals.LieuEnCours.nom);

        QtyCount = "";
        setTitle("ClinoTag - QTY");
    }

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

            // Post AjoutQTY data
            Globals g = (Globals)getApplication();
            if(g.isNetworkConnected()){
                int result = -100;
                QtyPost lieu = new QtyPost(Globals.LieuEnCours.uidTag, Integer.parseInt(QtyCount));
                try {
                    result = new JsonTaskIntegerPost().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,
                            Globals.urlAPIClinoTag + "AjoutQTY",
                            new Gson().toJson(lieu)).get();
                } catch (ExecutionException e) {
                    e.printStackTrace();
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                if (result == 0) {
                    Toast.makeText(getApplicationContext(), "The Qty count is successfully registered.", Toast.LENGTH_SHORT).show();
                    startActivityForResult(new Intent(getApplicationContext(), MainActivity.class), 0);
                } else {
                    Toast.makeText(getApplicationContext(), "Failed to register Qty count.", Toast.LENGTH_SHORT).show();
                }
            }else{
                Toast.makeText(getApplicationContext(), R.string.noconnexion, Toast.LENGTH_LONG).show();
            }

            scanEnCours = true;
            String req = Globals.urlAPIClinoTag + "IdentificationTag/" + hexTagId ;
            try {
                String result = new JsonTaskString().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();
                if (result != null && !result.equals("")) {
                    if(result.equals("LIEU")) {
                        req = Globals.urlAPIClinoTag + "ScanLieu/" + hexTagId ;
                        Lieu rLieu = new JsonTaskLieu().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

                        // find location from Uid Tag
                        if (rLieu != null) {
                            // finish();

                            if(rLieu.progress == 1)
                                Globals.isWorking = true;
                            else if(rLieu.progress == 2)
                                Globals.isWorking = false;

                            Globals.LieuEnCours = rLieu;
                            startActivityForResult(new Intent(getApplicationContext(), PassageActivity.class), 0);
                            Toast.makeText(getApplicationContext(), rLieu.client + "/" + rLieu.nom + " récupérée.", Toast.LENGTH_SHORT).show();
                        }
                    } else if(result.equals("MATERIEL")) {

                        // set agent is working part
                        if(Globals.isWorking == true)
                            Globals.isWorking = false;

                        else if(Globals.isWorking == false)
                            Globals.isWorking = true;

                        req = Globals.urlAPIClinoTag + "ScanMateriel/" + hexTagId ;
                        Materiel rMateriel = new JsonTaskMateriel().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();
                        if (rMateriel != null) {
                            //finish();
                            Globals.MaterielEnCours = rMateriel;
                            startActivityForResult(new Intent(getApplicationContext(), UtilisationActivity.class), 0);
                            Toast.makeText(getApplicationContext(), rMateriel.client + "/" + rMateriel.nom + " récupérée.", Toast.LENGTH_SHORT).show();
                        }
                    } else if(result.equals("QTY"))
                    {
                        req = Globals.urlAPIClinoTag + "ScanLieu/" + hexTagId ;
                        Lieu rLieu = new JsonTaskLieu().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

                        Globals.LieuEnCours = rLieu;

                        // find location from Uid Tag
                        if (rLieu != null) {
                            startActivityForResult(new Intent(getApplicationContext(), QtyActivity.class), 0);
                        }
                    }
                } else {
                    DialogInterface.OnClickListener dialogClickListener = (dialog, which) -> {
                        switch (which){
                            case DialogInterface.BUTTON_NEUTRAL:
                                DialogNouveauLieu(hexTagId);
                                break;
                            case DialogInterface.BUTTON_NEGATIVE:
                                DialogNouveauMateriel(hexTagId);
                                break;
                        }
                    };

                    req = Globals.urlAPIClinoTag + "ListeClient";
                    Globals.listeClient = new JsonTaskClients().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.setMessage("Le tag " + hexTagId + " est inconnu, que souhaitez-vous enregistrer ?")
                            .setNeutralButton("Lieu", dialogClickListener)
                            .setNegativeButton("Matériel", dialogClickListener)
                            .setPositiveButton("Annuler", dialogClickListener)
                            .show();
                }
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            scanEnCours = false;
        } catch (Exception e) {
            Toast.makeText(getApplicationContext(), "Erreur lors de la lecture du tag.", Toast.LENGTH_SHORT).show();
        }
    }

    void dialogNomMateriel(String hexTagId, int idClient) {

        final AlertDialog dialogNomMateriel = DialogTexte.creerDialogTexte(QtyActivity.this);
        dialogNomMateriel.show();
        dialogNomMateriel.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(v0 -> {

            Globals g = (Globals)getApplication();
            if(g.isNetworkConnected()){
                int result = -100;
                LieuOuMaterielPost materiel = new LieuOuMaterielPost(hexTagId, idClient, DialogTexte.inputTexte.getText().toString());
                try {
                    result = new JsonTaskIntegerPost().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,
                            Globals.urlAPIClinoTag + "AjoutMateriel",
                            new Gson().toJson(materiel)).get();
                } catch (ExecutionException e) {
                    e.printStackTrace();
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                if (result == 0) {
                    Toast.makeText(getApplicationContext(), "The material is associated with the tag.", Toast.LENGTH_SHORT).show();
                    ReadingTag(hexTagId);
                } else {
                    Toast.makeText(getApplicationContext(), "Hardware registration failed.", Toast.LENGTH_SHORT).show();
                }
                dialogNomMateriel.dismiss();
            }else{
                Toast.makeText(getApplicationContext(), R.string.noconnexion, Toast.LENGTH_LONG).show();
            }

        });
    }


    private void DialogNouveauMateriel(String hexTagId) {
        List<String> lClientId = new ArrayList<String>();
        List<String> lClientNom = new ArrayList<String>();
        for(Client p : Globals.listeClient){
            lClientId.add(String.valueOf(p.idClient));
            lClientNom.add(p.nom);
        }

        final CharSequence[] csClientsNom = lClientNom.toArray(new CharSequence[lClientNom.size()]);

        AlertDialog.Builder dialogPubliBuilder = new MaterialAlertDialogBuilder(QtyActivity.this)
                .setTitle("Sélection du client")
                .setItems(csClientsNom, (dialog, which) -> {
                    dialogNomMateriel(hexTagId, Integer.parseInt(lClientId.get(which)));
                });

        dialogPubliBuilder.create().show();
    }

    private void DialogNouveauLieu(String hexTagId) {
        List<String> lClientId = new ArrayList<String>();
        List<String> lClientNom = new ArrayList<String>();
        for(Client p : Globals.listeClient){
            lClientId.add(String.valueOf(p.idClient));
            lClientNom.add(p.nom);
        }

        final CharSequence[] csClientsNom = lClientNom.toArray(new CharSequence[lClientNom.size()]);

        AlertDialog.Builder dialogPubliBuilder = new MaterialAlertDialogBuilder(QtyActivity.this)
                .setTitle("Sélection du client")
                .setItems(csClientsNom, (dialog, which) -> {
                    dialogNomLieu(hexTagId, Integer.parseInt(lClientId.get(which)));
                });

        dialogPubliBuilder.create().show();
    }

    void dialogNomLieu(String hexTagId, int idClient) {

        final AlertDialog dialogNomLieu = DialogTexte.creerDialogTexte(QtyActivity.this);
        dialogNomLieu.show();
        dialogNomLieu.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(v0 -> {

            Globals g = (Globals)getApplication();
            if(g.isNetworkConnected()){
                int result = -100;
                LieuOuMaterielPost lieu = new LieuOuMaterielPost(hexTagId, idClient, DialogTexte.inputTexte.getText().toString());
                try {
                    result = new JsonTaskIntegerPost().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,
                            Globals.urlAPIClinoTag + "AjoutLieu",
                            new Gson().toJson(lieu)).get();
                } catch (ExecutionException e) {
                    e.printStackTrace();
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                if (result == 0) {
                    Toast.makeText(getApplicationContext(), "The name is associated with the tag.", Toast.LENGTH_SHORT).show();
                    ReadingTag(hexTagId);
                } else {
                    Toast.makeText(getApplicationContext(), "Failed to register name.", Toast.LENGTH_SHORT).show();
                }
                dialogNomLieu.dismiss();
            }else{
                Toast.makeText(getApplicationContext(), R.string.noconnexion, Toast.LENGTH_LONG).show();
            }
        });
    }

    public void onClose(View v) {
        startActivityForResult(new Intent(getApplicationContext(), MainActivity.class), 0);
    }

    public  void onRemove(View v) {
        if(QtyCount.length() >= 1)
        {
            QtyCount = QtyCount.substring(0, QtyCount.length()-1);
            labQtyCount.setText(QtyCount);
        }
    }

    public void onCheck(View v) {
        Globals g = (Globals)getApplication();
        if(g.isNetworkConnected()){
            int result = -100;
            QtyPost lieu = new QtyPost(Globals.LieuEnCours.uidTag, Integer.parseInt(QtyCount));
            try {
                result = new JsonTaskIntegerPost().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,
                        Globals.urlAPIClinoTag + "AjoutQTY",
                        new Gson().toJson(lieu)).get();
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }

            if (result == 0) {
                Toast.makeText(getApplicationContext(), "The Qty count is successfully registered.", Toast.LENGTH_SHORT).show();
                startActivityForResult(new Intent(getApplicationContext(), MainActivity.class), 0);
            } else {
                Toast.makeText(getApplicationContext(), "Failed to register Qty count.", Toast.LENGTH_SHORT).show();
            }
        }else{
            Toast.makeText(getApplicationContext(), R.string.noconnexion, Toast.LENGTH_LONG).show();
        }
    }
    public void SaisieCode(View v) {
        Button btn = (Button) findViewById(v.getId());
        QtyCount = QtyCount + btn.getText();
        labQtyCount.append(btn.getText());
    }
}