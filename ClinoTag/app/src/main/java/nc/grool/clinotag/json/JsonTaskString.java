package nc.grool.clinotag.json;

import android.os.AsyncTask;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

public class JsonTaskString extends AsyncTask<String, String, String> {

    public JsonTaskString() {
        super();
    }

    protected String doInBackground(String... params) {

        HttpURLConnection connection = null;
        BufferedReader reader = null;

        try {
            String response = "";
            URL url = new URL(params[0]);
            connection = (HttpURLConnection) url.openConnection();
//            connection.setRequestProperty("Authorization", "Bearer " + params[1]);
//            connection.setConnectTimeout(1000);
//            connection.setReadTimeout(1000);
            connection.connect();

            InputStream stream = connection.getInputStream();
            reader = new BufferedReader(new InputStreamReader(stream));
            StringBuffer buffer = new StringBuffer();
            String line = "";
            while ((line = reader.readLine()) != null) buffer.append(line);

            boolean authOk = buffer.length() < 15;
            if(!authOk) authOk = !buffer.substring(0, 15).equals("<!DOCTYPE html>");
            if(authOk){
//                Type t = new TypeToken<String>(){}.getType();
//                Gson gson = new GsonBuilder()
//                        .setLenient()
//                        .create();

                return String.valueOf(buffer);
            }
        } catch (MalformedURLException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            if (connection != null) connection.disconnect();
            try {
                if (reader != null) reader.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        return null;
    }

    @Override
    protected void onPostExecute(String result) {

        super.onPostExecute(result);

    }
}
