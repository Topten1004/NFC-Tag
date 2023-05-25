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

import java.util.concurrent.ExecutionException;

import nc.grool.clinotag.composant.HttpsTrustManager;
import nc.grool.clinotag.dto.Agent;
import nc.grool.clinotag.json.JsonTaskAgent;

public class LoginActivity extends AppCompatActivity {

    TextView labCodeLogin;
    String codeSaisie = "";
    CountDownTimer cdtSaisie = null;
    static boolean bloqueSaisie = false;


    @RequiresApi(api = Build.VERSION_CODES.O)
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        Globals.idConstructeur = Build.SERIAL.toUpperCase();

        HttpsTrustManager.allowAllSSL();
        labCodeLogin = (TextView) findViewById(R.id.labCodeLogin);
        setTitle("ClinoTag - Login");

        Globals g = (Globals)getApplication();
        String codeAgent = g.lirePref("codeAgent");
        if(codeAgent != null){
            String req = Globals.urlAPIClinoTag + "AgentLogin/" + codeAgent;
            try {
                if(!g.isNetworkConnected()){
                    Toast.makeText(getApplicationContext(), "Internet connection required", Toast.LENGTH_SHORT).show();
                    return;
                }
                Agent result = new JsonTaskAgent().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();
                if (result != null) {
                    finish();
                    Globals.cetAgent = result;
                    startActivityForResult(new Intent(getApplicationContext(), MainActivity.class), 0);
                } else {
                    Toast.makeText(getApplicationContext(), "Unknown code", Toast.LENGTH_SHORT).show();
                }
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }

    public void SaisieCode(View v) {
        if(bloqueSaisie) return;

        if(cdtSaisie != null)cdtSaisie.cancel();
        cdtSaisie = new CountDownTimer(2500, 300) {
            public void onTick(long millisUntilFinished) {}
            public void onFinish() {
                bloqueSaisie = false;
                codeSaisie = "";
                labCodeLogin.setText("");
                this.start();
            }
        }.start();

        Button btn = (Button) findViewById(v.getId());
        codeSaisie = codeSaisie + btn.getText();
        labCodeLogin.append("*");

        if(codeSaisie.length() == 5){
            bloqueSaisie = true;
            String req = Globals.urlAPIClinoTag + "AgentLogin/" + codeSaisie;
            try {
                Globals g = (Globals)getApplication();
                if(!g.isNetworkConnected()){
                    Toast.makeText(getApplicationContext(), "Internet connection required", Toast.LENGTH_SHORT).show();
                    return;
                }
                Agent result = new JsonTaskAgent().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();
                if (result != null) {
                    finish();
                    Globals.cetAgent = result;
                    g.ecrirePref("codeAgent", codeSaisie);
                    startActivityForResult(new Intent(getApplicationContext(), MainActivity.class), 0);
                } else {
                    Toast.makeText(getApplicationContext(), "Unknown code", Toast.LENGTH_SHORT).show();
                }
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            codeSaisie = "";
            labCodeLogin.setText("");
            bloqueSaisie = false;
        }
    }
}