package nc.grool.clinotag;

import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;

import java.util.concurrent.ExecutionException;

import nc.grool.clinotag.composant.DialogTexte;
import nc.grool.clinotag.composant.HttpsTrustManager;
import nc.grool.clinotag.dto.Agent;
import nc.grool.clinotag.dto.LieuOuMaterielPost;
import nc.grool.clinotag.dto.QtyPost;
import nc.grool.clinotag.json.JsonTaskAgent;
import nc.grool.clinotag.json.JsonTaskIntegerPost;

public class QtyActivity extends AppCompatActivity {

    TextView labQtyCount;

    TextView labLieuName;
    String QtyCount = "";

    @RequiresApi(api = Build.VERSION_CODES.O)
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_qty);

        Globals.idConstructeur = Build.SERIAL.toUpperCase();

        HttpsTrustManager.allowAllSSL();
        labQtyCount = (TextView) findViewById(R.id.InputQtyCount);
        labQtyCount.setText("");

        labLieuName = (TextView) findViewById(R.id.labLieuName);
        labLieuName.setText(Globals.LieuEnCours.nom);

        QtyCount = "";
        setTitle("ClinoTag - QTY");
    }

    public void onClose(View v) {
        startActivityForResult(new Intent(getApplicationContext(), MainActivity.class), 0);
    }

    public  void onRemove(View v) {
        QtyCount = QtyCount.substring(0, QtyCount.length()-1);
        labQtyCount.setText(QtyCount);
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