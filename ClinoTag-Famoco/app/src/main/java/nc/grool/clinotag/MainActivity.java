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
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.material.dialog.MaterialAlertDialogBuilder;
import com.google.gson.Gson;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.concurrent.ExecutionException;

import nc.grool.clinotag.composant.DialogTexte;
import nc.grool.clinotag.composant.Location;
import nc.grool.clinotag.composant.RecyclerViewLieuAdapter;
import nc.grool.clinotag.composant.RecyclerViewLocationTaskAdapter;
import nc.grool.clinotag.composant.RecyclerViewTacheAdapter;
import nc.grool.clinotag.dto.Client;
import nc.grool.clinotag.dto.Lieu;
import nc.grool.clinotag.dto.LieuOuMaterielPost;
import nc.grool.clinotag.dto.Materiel;
import nc.grool.clinotag.json.JsonTaskClients;
import nc.grool.clinotag.json.JsonTaskIntegerPost;
import nc.grool.clinotag.json.JsonTaskLieu;
import nc.grool.clinotag.json.JsonTaskMateriel;
import nc.grool.clinotag.json.JsonTaskNotification;
import nc.grool.clinotag.json.JsonTaskString;
import nc.grool.clinotag.outil.Format;

public class MainActivity extends AppCompatActivity {

    public Location location;
    static boolean scanInProgress = false;
    NfcAdapter mAdapter;

    public TextView tvScannedTime = null;

    public TextView tvLocationName = null;

    @Override
    public void onStart() {
        super.onStart();

        mAdapter = NfcAdapter.getDefaultAdapter(this);

        Button btn = this.findViewById(R.id.Notification);

        tvScannedTime = this.findViewById(R.id.scannedTime);
        tvLocationName = this.findViewById(R.id.locationName);

        tvScannedTime.setVisibility(View.GONE);
        tvLocationName.setVisibility(View.GONE);

        if(Globals.isWorking)
        {
            btn.setVisibility(View.GONE);
        } else {
            btn.setVisibility(View.VISIBLE);
        }


        if(mAdapter != null){
            if (mAdapter.isEnabled()) {
                toogleNfc(true);
            }
        }

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
        loadingLists();
    }

    private String onBatteryLevel() {

        IntentFilter ifilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
        Intent batteryStatus = getApplicationContext().registerReceiver(null, ifilter);
        int level = batteryStatus.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
        int scale = batteryStatus.getIntExtra(BatteryManager.EXTRA_SCALE, -1);

        return Integer.toString( (int)(level * 100 / (float)scale) );
    }

    static String hexTagId;

    public void onClickNotification(View v) {

        String req = Globals.urlAPIClinoTag + "Notify/" ;

        try {
            List<Lieu> result = (List<Lieu>) new JsonTaskNotification().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

            if (result != null) {

                Globals.listLieus = result;
                loadingLists();

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
                        if(!scanInProgress){
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

    private void loadingLists() {

        Button btn = this.findViewById(R.id.Notification);

        RecyclerView recyclerView = findViewById(R.id.recyclerViewLieu);
        RecyclerView recyclerViewLocationTask = findViewById(R.id.recyclerViewTrainModeTasks);

        if ( !Globals.cetAgent.trainMode) {

            if (Globals.isWorking) {
                btn.setVisibility(View.GONE);
            } else {
                btn.setVisibility(View.VISIBLE);
            }

            recyclerView.setVisibility(View.VISIBLE);

            if (Globals.listLieus != null) {

                RecyclerViewLieuAdapter adapter = new RecyclerViewLieuAdapter(Globals.listLieus, getApplication());
                recyclerView.setAdapter(adapter);
                recyclerView.setLayoutManager(new LinearLayoutManager(this));
                adapter.notifyDataSetChanged();
            }

        } else {

            recyclerView.setVisibility(View.GONE);

            if (Globals.LocationInProgress != null && Globals.LocationInProgress.lTache != null) {

                tvLocationName.setVisibility(View.VISIBLE);

                SimpleDateFormat sdf = new SimpleDateFormat("HH:mm:ss");
                String currentTime = sdf.format(new Date());
                tvScannedTime.setText(currentTime);

                tvLocationName.setText(Globals.LocationInProgress.nom);

                recyclerViewLocationTask = findViewById(R.id.recyclerViewTrainModeTasks);
                RecyclerViewLocationTaskAdapter adapter = new RecyclerViewLocationTaskAdapter(Globals.LocationInProgress.lTache, getApplication());
                recyclerViewLocationTask.setAdapter(adapter);
                recyclerViewLocationTask.setLayoutManager(new LinearLayoutManager(this));
            }
        }
    }

    private void ReadingTag(String hexTagId) {
        try {
            scanInProgress = true;
            String req = Globals.urlAPIClinoTag + "IdentificationTag/" + hexTagId ;

            try {
                String result = new JsonTaskString().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

                if (result != null && !result.equals(""))
                {
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

                            Globals.LocationInProgress = rLieu;

                            if( !Globals.trainMode)
                            {
                                startActivityForResult(new Intent(getApplicationContext(), PassageActivity.class), 0);
                                Toast.makeText(getApplicationContext(), rLieu.client + "/" + rLieu.nom + " recovered.", Toast.LENGTH_SHORT).show();
                            } else{ // display the tasks when user on train mode

                                loadingLists();
                            }
                        }
                    } else if(result.equals("MATERIEL")) {

                        if(!Globals.trainMode)
                        {
                            // set agent is working part
                            if(Globals.isWorking)
                                Globals.isWorking = false;
                            else
                                Globals.isWorking = true;

                            req = Globals.urlAPIClinoTag + "ScanMateriel/" + hexTagId ;
                            Materiel rMateriel = new JsonTaskMateriel().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();
                            if (rMateriel != null) {
                                //finish();
                                Globals.MaterialInProgress = rMateriel;
                                startActivityForResult(new Intent(getApplicationContext(), UtilisationActivity.class), 0);
                                Toast.makeText(getApplicationContext(), rMateriel.client + "/" + rMateriel.nom + " recovered.", Toast.LENGTH_SHORT).show();
                            }
                        }

                    } else if(result.equals("QTY"))
                    {
                        if(!Globals.trainMode)
                        {
                            req = Globals.urlAPIClinoTag + "ScanLieu/" + hexTagId ;
                            Lieu rLieu = new JsonTaskLieu().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

                            Globals.LocationInProgress = rLieu;

                            // find location from Uid Tag
                            if (rLieu != null) {
                                startActivityForResult(new Intent(getApplicationContext(), QtyActivity.class), 0);
                            }
                        }
                    }
                } else {

                    if(!Globals.trainMode)
                    {
                        DialogInterface.OnClickListener dialogClickListener = (dialog, which) -> {
                            switch (which){
                                case DialogInterface.BUTTON_NEUTRAL:
                                    OnSaveLocation(hexTagId, 9);
                                    break;
                                case DialogInterface.BUTTON_NEGATIVE:
                                    OnSaveHardware(hexTagId, 9);
                                    break;
                            }
                        };

                        AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        builder.setMessage("The tag " + hexTagId + " is unknown, what do you want to record?")
                                .setNeutralButton("Location", dialogClickListener)
                                .setNegativeButton("Hardware", dialogClickListener)
                                .setPositiveButton("Cancel", dialogClickListener)
                                .show();
                    }
                }
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            scanInProgress = false;
        } catch (Exception e) {
            Toast.makeText(getApplicationContext(), "Error reading the tag.", Toast.LENGTH_SHORT).show();
        }
    }

    void OnSaveHardware(String hexTagId, int idClient) {

        final AlertDialog dialogNomMateriel = DialogTexte.creerDialogTexte(MainActivity.this);
        dialogNomMateriel.show();
        dialogNomMateriel.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(v0 -> {

            Globals g = (Globals)getApplication();
            if(g.isNetworkConnected()){
                int result = -100;
                LieuOuMaterielPost materiel = new LieuOuMaterielPost(hexTagId, idClient, DialogTexte.inputText.getText().toString());
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
                    Toast.makeText(getApplicationContext(), "The hardware is associated with the tag.", Toast.LENGTH_SHORT).show();
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

    void OnSaveLocation(String hexTagId, int idClient) {

        final AlertDialog dialogNomLieu = DialogTexte.creerDialogTexte(MainActivity.this);
        dialogNomLieu.show();
        dialogNomLieu.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(v0 -> {

            Globals g = (Globals)getApplication();
            if(g.isNetworkConnected()){
                int result = -100;
                LieuOuMaterielPost lieu = new LieuOuMaterielPost(hexTagId, idClient, DialogTexte.inputText.getText().toString());
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