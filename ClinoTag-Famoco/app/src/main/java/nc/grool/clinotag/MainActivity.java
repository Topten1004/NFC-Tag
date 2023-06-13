//cd 'C:\Program Files\Android\platform-tools'
//.\adb tcpip 5555
//.\adb shell ip addr show wlan0
//.\adb connect 192.168.20.18

package nc.grool.clinotag;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.view.menu.MenuBuilder;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.annotation.SuppressLint;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.media.AudioManager;
import android.media.ToneGenerator;
import android.nfc.NfcAdapter;
import android.nfc.Tag;
import android.os.AsyncTask;
import android.os.BatteryManager;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
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
import nc.grool.clinotag.composant.Location;
import nc.grool.clinotag.composant.RecyclerViewLieuAdapter;
import nc.grool.clinotag.composant.RecyclerViewTacheAdapter;
import nc.grool.clinotag.dto.Agent;
import nc.grool.clinotag.dto.Client;
import nc.grool.clinotag.dto.Lieu;
import nc.grool.clinotag.dto.LieuOuMaterielPost;
import nc.grool.clinotag.dto.Materiel;
import nc.grool.clinotag.json.JsonTaskAgent;
import nc.grool.clinotag.json.JsonTaskClients;
import nc.grool.clinotag.json.JsonTaskIntegerPost;
import nc.grool.clinotag.json.JsonTaskLieu;
import nc.grool.clinotag.json.JsonTaskMateriel;
import nc.grool.clinotag.json.JsonTaskNotification;
import nc.grool.clinotag.json.JsonTaskString;
import nc.grool.clinotag.outil.Format;

public class MainActivity extends AppCompatActivity {

    public Location location;

    static boolean scanEnCours = false;
    NfcAdapter mAdapter;

    @Override
    public void onStart() {
        super.onStart();

        mAdapter = NfcAdapter.getDefaultAdapter(this);

        Button btn = this.findViewById(R.id.Notification);
//        TextView txtInstructions = this.findViewById(R.id.txtInstructions);

        if(Globals.isWorking == true)
        {
            btn.setVisibility(View.GONE);
        } else {
            btn.setVisibility(View.VISIBLE);
        }


        if(mAdapter == null){
        } else {
            if (!mAdapter.isEnabled()) {
            } else {
                toogleNfc(true);
            }
        }

//        if(mAdapter != null)
//        {
//            if(mAdapter.isEnabled())
//                toogleNfc(true);
//        }

        chargement();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        location = new Location(getApplicationContext());

        setTitle(Globals.getCurrentTime() + " - " + Globals.cetAgent.nom);
        new CountDownTimer(5000, 300) {

            public void onTick(long millisUntilFinished) {}

            public void onFinish() {
                setTitle(Globals.getCurrentTime() + " - " + Globals.cetAgent.nom); //+ niveauBatterie() + "% - "
                this.start();
            }
        }.start();
    }

    @SuppressLint("RestrictedApi")
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.main, menu);
        if(menu instanceof MenuBuilder){
            MenuBuilder m = (MenuBuilder) menu;
            m.setOptionalIconsVisible(true);
        }
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.action_actualise:
//                actualise(true);
//                chargement();
                break;
            case R.id.action_deconnexion:
                Globals g = (Globals)getApplication();
                g.ecrirePref("codeAgent", null);
                startActivityForResult(new Intent(getApplicationContext(), LoginActivity.class), 0);
                finish();
                break;
        }
        return super.onOptionsItemSelected(item);
    }

    @Override
    protected void onResume() {
        super.onResume();
        location.initLocation();
        chargement();
    }

    private String niveauBatterie() {
        IntentFilter ifilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
        Intent batteryStatus = getApplicationContext().registerReceiver(null, ifilter);
        int level = batteryStatus.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
        int scale = batteryStatus.getIntExtra(BatteryManager.EXTRA_SCALE, -1);

        return Integer.toString( (int)(level * 100 / (float)scale) );
    }

    static String hexTagId;

    public void onClickImgNfc(View v) {
        if(!scanEnCours) {
            ReadingTag(hexTagId); //534E35AF016640-9
        }
    }

    public void onClickNotification(View v) {
        
        String req = Globals.urlAPIClinoTag + "Notify/" ;
        try {
            List<Lieu> result = (List<Lieu>) new JsonTaskNotification().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

            if (result != null) {
                Globals.listLieus = result;
                chargement();
            } else {
                Toast.makeText(getApplicationContext(), "Unknown code", Toast.LENGTH_SHORT).show();
            }

        } catch (ExecutionException e) {
            e.printStackTrace();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }
    void toogleNfc(Boolean enable) {
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

    private void chargement() {

        Button btn = this.findViewById(R.id.Notification);

        if(Globals.isWorking == true)
        {
            btn.setVisibility(View.GONE);
        } else {
            btn.setVisibility(View.VISIBLE);
        }

        RecyclerView recyclerView = findViewById(R.id.recyclerViewLieu);
        RecyclerViewLieuAdapter adapter = new RecyclerViewLieuAdapter(Globals.listLieus, getApplication());
        recyclerView.setAdapter(adapter);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
//        adapter.setOnItemClickListener(onItemClickListener);
        adapter.notifyDataSetChanged();
    }

    private void ReadingTag(String hexTagId) {
        try {
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

    private void DialogNouveauMateriel(String hexTagId) {
        List<String> lClientId = new ArrayList<String>();
        List<String> lClientNom = new ArrayList<String>();
        for(Client p : Globals.listeClient){
            lClientId.add(String.valueOf(p.idClient));
            lClientNom.add(p.nom);
        }

        final CharSequence[] csClientsNom = lClientNom.toArray(new CharSequence[lClientNom.size()]);

        AlertDialog.Builder dialogPubliBuilder = new MaterialAlertDialogBuilder(MainActivity.this)
                .setTitle("Sélection du client")
                .setItems(csClientsNom, (dialog, which) -> {
                    dialogNomMateriel(hexTagId, Integer.parseInt(lClientId.get(which)));
                });

        dialogPubliBuilder.create().show();
    }

    void dialogNomMateriel(String hexTagId, int idClient) {

        final AlertDialog dialogNomMateriel = DialogTexte.creerDialogTexte(MainActivity.this);
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

    private void DialogNouveauLieu(String hexTagId) {
        List<String> lClientId = new ArrayList<String>();
        List<String> lClientNom = new ArrayList<String>();
        for(Client p : Globals.listeClient){
            lClientId.add(String.valueOf(p.idClient));
            lClientNom.add(p.nom);
        }

        final CharSequence[] csClientsNom = lClientNom.toArray(new CharSequence[lClientNom.size()]);

        AlertDialog.Builder dialogPubliBuilder = new MaterialAlertDialogBuilder(MainActivity.this)
                .setTitle("Sélection du client")
                .setItems(csClientsNom, (dialog, which) -> {
                    dialogNomLieu(hexTagId, Integer.parseInt(lClientId.get(which)));
                });

        dialogPubliBuilder.create().show();
    }

    void dialogNomLieu(String hexTagId, int idClient) {

        final AlertDialog dialogNomLieu = DialogTexte.creerDialogTexte(MainActivity.this);
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
}