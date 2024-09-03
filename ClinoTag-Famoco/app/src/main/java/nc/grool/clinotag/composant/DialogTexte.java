package nc.grool.clinotag.composant;

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

public class DialogTexte {

    public static EditText inputText;
    

    @RequiresApi(api = Build.VERSION_CODES.JELLY_BEAN_MR1)
    public static AlertDialog creerDialogTexte(Context _context) {
        String titre = "Tag registration";
        AlertDialog dialogText = new MaterialAlertDialogBuilder(_context)
                .setTitle(titre)
                .setMessage("Name")
                .setPositiveButton("Register", null)
                .setNegativeButton("Cancel", null)
                .create();

        LinearLayout layout = new LinearLayout(_context);
        layout.setOrientation(LinearLayout.VERTICAL);

        inputText = new EditText(_context);
        inputText.setInputType(InputType.TYPE_CLASS_TEXT);
        inputText.setFilters(new InputFilter[] { new InputFilter.LengthFilter(99) });

        inputText.setSelection(inputText.getText().length());
        inputText.setTextAlignment(View.TEXT_ALIGNMENT_CENTER);
        layout.addView(inputText);
        dialogText.setView(layout);

        return dialogText;
    }
}
