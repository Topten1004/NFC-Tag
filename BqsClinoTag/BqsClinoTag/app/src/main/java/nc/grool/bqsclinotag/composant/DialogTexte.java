package nc.grool.bqsclinotag.composant;

import android.content.Context;
import android.os.Build;
import android.text.InputFilter;
import android.text.InputType;
import android.view.View;
import android.widget.EditText;
import android.widget.LinearLayout;

import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AlertDialog;

import com.google.android.material.dialog.MaterialAlertDialogBuilder;
import com.google.android.material.slider.Slider;

public class DialogTexte {

    public static EditText inputTexte;
    

    @RequiresApi(api = Build.VERSION_CODES.JELLY_BEAN_MR1)
    public static AlertDialog creerDialogTexte(Context _context) {
        String titre = "Enregistrement du tag";
        AlertDialog dialogTexte = new MaterialAlertDialogBuilder(_context)
                .setTitle(titre)
                .setMessage("Nom")
                .setPositiveButton("Enregistrer", null)
                .setNegativeButton("Annuler", null)
                .create();

        LinearLayout layout = new LinearLayout(_context);
        layout.setOrientation(LinearLayout.VERTICAL);

        inputTexte = new EditText(_context);
        inputTexte.setInputType(InputType.TYPE_CLASS_TEXT);
        inputTexte.setFilters(new InputFilter[] { new InputFilter.LengthFilter(99) });

        inputTexte.setSelection(inputTexte.getText().length());
        inputTexte.setTextAlignment(View.TEXT_ALIGNMENT_CENTER);
        layout.addView(inputTexte);
        dialogTexte.setView(layout);

        return dialogTexte;
    }
}
