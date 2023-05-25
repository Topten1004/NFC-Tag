package nc.grool.clinotag.outil;

import android.content.Context;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FilenameFilter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;

public class Fichier {

    public static String lecture(Context context, String fileName) {
        try {
            FileInputStream fis = context.openFileInput(fileName);
            InputStreamReader isr = new InputStreamReader(fis);
            BufferedReader bufferedReader = new BufferedReader(isr);
            StringBuilder sb = new StringBuilder();
            String line;
            while ((line = bufferedReader.readLine()) != null) {
                sb.append(line);
            }
            return sb.toString();
        } catch (FileNotFoundException fileNotFound) {
            return null;
        } catch (IOException ioException) {
            return null;
        }
    }

    public static boolean ecriture(Context context, String fileName, String jsonString){
        try {
            suppression(context, fileName);
            FileOutputStream fos = context.openFileOutput(fileName, Context.MODE_PRIVATE);
            if (jsonString != null) {
                fos.write(jsonString.getBytes());
            }
            fos.close();
            return true;
        } catch (FileNotFoundException fileNotFound) {
            return false;
        } catch (IOException ioException) {
            return false;
        }

    }

//    public static boolean modification(Context context, String fileName, String jsonString){
//        try {
//            FileOutputStream fos = context.openFileOutput(fileName,Context.MODE_PRIVATE);
//            if (jsonString != null) {
//                fos.write(jsonString.getBytes());
//            }
//            fos.close();
//            return true;
//        } catch (FileNotFoundException fileNotFound) {
//            return false;
//        } catch (IOException ioException) {
//            return false;
//        }
//
//    }

    private static void suppression(Context context, String fileName){
        File f = fichierExiste(context, fileName);
        if( f != null){
            f.delete();
        }
    }

    private static File fichierExiste(Context context, String fileName) {
        String path = context.getFilesDir().getAbsolutePath() + "/" + fileName;
        File file = new File(path);
        return file;
    }

//    public static boolean clearFile(String fileName) throws FileNotFoundException {
//        PrintWriter pw = new PrintWriter(fileName);
//        pw.close();
//    }

    public static List<File> bSyncFichier(String pathname, final String ext)
    {
        pathname += "/files";
        List<File> lS = new ArrayList<>();
        File file = new File(pathname);

        if(file.listFiles(new FilenameFilter() {
            public boolean accept(File dir, String name) {
                return name.toLowerCase().endsWith(ext);
            }
        }) == null) return lS;

        for(File f :file.listFiles()) {
            if( f.getName().endsWith(ext) ) {
                lS.add( f );
            }
        }
        return lS;
    }

    public static List<File> trouveFichier(String pathname, final String ext)
    {
        pathname += "/files";
        List<File> lS = new ArrayList<>();
        File file = new File(pathname);

        if(file.listFiles(new FilenameFilter() {
            public boolean accept(File dir, String name) {
                return name.toLowerCase().endsWith(ext);
            }
        }) == null) return lS;

        for(File f :file.listFiles()) {
            if( f.getName().endsWith(ext) ) {
                lS.add( f );
            }
        }
        return lS;
    }

    public static void supprimeFichiers(String pathname, final String ext)
    {
        pathname += "/files";
        File file = new File(pathname);
        if( file.listFiles() != null){
            for(File f : file.listFiles()) {
                if( f.getName().endsWith(ext) ) {
                    f.delete();
                }
            }
        }
    }

}
