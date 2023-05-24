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
import android.widget.ScrollView;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.material.dialog.MaterialAlertDialogBuilder;
import com.google.gson.Gson;
import com.urovo.sdk.rfcard.RFCardHandlerImpl;
import com.urovo.sdk.rfcard.listener.RFSearchListener;
import com.urovo.sdk.rfcard.utils.Constant;
import com.urovo.sdk.utils.BytesUtil;

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
//    NfcAdapter mAdapter;
    public RFCardHandlerImpl rfReader;

    @Override
    public void onStart() {
        super.onStart();

//        mAdapter = NfcAdapter.getDefaultAdapter(this);

        Button btn = this.findViewById(R.id.Notification);
//        TextView txtInstructions = this.findViewById(R.id.txtInstructions);

        if(Globals.isWorking == true)
        {
            btn.setVisibility(View.GONE);
        } else {
            btn.setVisibility(View.VISIBLE);
        }

        startSearchCard();

//        if(mAdapter == null){
//            txtInstructions.setText("Press the image.");
//            txtInstructions.setTextColor(getResources().getColor(R.color.white));
//        } else {
//            if (!mAdapter.isEnabled()) {
//                txtInstructions.setText("NFC is not enabled/available.");
//                txtInstructions.setTextColor(getResources().getColor(R.color.rougeDoux));
//            } else {
//                txtInstructions.setText("Scan a name or material tag.");
//                txtInstructions.setTextColor(getResources().getColor(R.color.black));
//                toogleNfc(true);
//            }
//        }

        chargement();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        rfReader = RFCardHandlerImpl.getInstance();
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
//    void toogleNfc(Boolean enable) {
//        if(enable){
//            final ToneGenerator tg = new ToneGenerator(AudioManager.STREAM_MUSIC, ToneGenerator.MAX_VOLUME);
//
//            mAdapter.enableReaderMode(this, new NfcAdapter.ReaderCallback() {
//                @Override
//                public void onTagDiscovered(final Tag tag) {
//                    runOnUiThread(() -> {
//                        if(!scanEnCours){
//                            hexTagId = Format.bytesToHexString(tag.getId()).substring(2).toUpperCase();
//                            Toast.makeText(getApplicationContext(), hexTagId, Toast.LENGTH_SHORT).show();
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
//      adapter.setOnItemClickListener(onItemClickListener);
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
                            Toast.makeText(getApplicationContext(), rMateriel.client + "/" + rMateriel.nom + " recovered.", Toast.LENGTH_SHORT).show();
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
                    builder.setMessage("The tag " + hexTagId + " is unknown, what do you want to record ?")
                            .setNeutralButton("Place", dialogClickListener)
                            .setNegativeButton("Material", dialogClickListener)
                            .setPositiveButton("Cancel", dialogClickListener)
                            .show();
                }
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            scanEnCours = false;
        } catch (Exception e) {
            Toast.makeText(getApplicationContext(), "Error reading tag.", Toast.LENGTH_SHORT).show();
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
                .setTitle("Customer selection")
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
                Toast.makeText(getApplicationContext(), R.string.noconnection, Toast.LENGTH_LONG).show();
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
                .setTitle("Customer selection")
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
                Toast.makeText(getApplicationContext(), R.string.noconnection, Toast.LENGTH_LONG).show();
            }
        });
    }
}