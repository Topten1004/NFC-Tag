package nc.grool.clinotag.composant;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Base64;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.Switch;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;
import androidx.cardview.widget.CardView;
import androidx.recyclerview.widget.RecyclerView;

import java.util.Collections;
import java.util.List;

import nc.grool.clinotag.Globals;
import nc.grool.clinotag.dto.Tache;
import nc.grool.clinotag.R;

public class RecyclerViewLocationTaskAdapter extends RecyclerView.Adapter<RecyclerViewLocationTaskAdapter.ViewHolderSite> {

    private List<Tache> taches = Collections.emptyList();
    private View.OnClickListener mOnItemClickListener;

    Context context;

    public RecyclerViewLocationTaskAdapter(List<Tache> list, Context context) {
        this.taches = list;
        this.context = context;
    }

    @Override
    public ViewHolderSite onCreateViewHolder(ViewGroup parent, int viewType) {
        // Inflate the layout, initialize the View Holder
        View v = LayoutInflater.from(parent.getContext()).inflate(R.layout.row_location_task_layout, parent, false);
        return new ViewHolderSite(v);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolderSite holder, int position) {
        // Set task details
        holder.labTache.setTag(taches.get(position).idTache);
        holder.labTache.setText(taches.get(position).nom);
        holder.switchTache.setTag(taches.get(position).idTacheLieu);

        // Check if photo data is available and decode it to Bitmap
        String base64Photo = taches.get(position).photo;
        if (base64Photo != null && !base64Photo.isEmpty()) {
            Bitmap bitmap = base64ToBitmap(base64Photo);
            if (bitmap != null) {
                holder.image.setImageBitmap(bitmap);
            } else {
            }
        } else {
        }

        // Handle switch state change
        holder.switchTache.setOnCheckedChangeListener((buttonView, isChecked) -> {
            if (Globals.PassageInProgress != null && Globals.PassageInProgress.lTache != null) {
                for (Tache t : Globals.PassageInProgress.lTache) {
                    if (t.idTacheLieu == (Integer) buttonView.getTag()) {
                        t.fait = isChecked;
                        return;
                    }
                }
            }
        });

        // Handle task name click for showing description
        holder.labTache.setOnClickListener(view -> {
            if (Globals.PassageInProgress != null && Globals.PassageInProgress.lTache != null) {
                for (Tache t : Globals.PassageInProgress.lTache) {
                    if (t.idTache == (Integer) view.getTag() && t.description != null) {
                        AlertDialog.Builder builder = new AlertDialog.Builder(view.getContext());
                        builder.setTitle(t.nom)
                                .setMessage(t.description)
                                .setPositiveButton("Ok", null)
                                .show();
                        return;
                    }
                }
            }
        });
    }

    @Override
    public int getItemCount() {
        // Returns the number of elements the RecyclerView will display
        return taches.size();
    }

    public void setOnItemClickListener(View.OnClickListener itemClickListener) {
        mOnItemClickListener = itemClickListener;
    }

    public class ViewHolderSite extends RecyclerView.ViewHolder {

        CardView cv;
        TextView labTache;
        Switch switchTache;
        ImageView image;

        public ViewHolderSite(View itemView) {
            super(itemView);

            cv = itemView.findViewById(R.id.cardViewTaskDistribution);
            labTache = itemView.findViewById(R.id.lb_task_name);
            switchTache = itemView.findViewById(R.id.sw_check);
            image = itemView.findViewById(R.id.img_task_image);

            itemView.setTag(this);
            itemView.setOnClickListener(mOnItemClickListener);
        }
    }

    @Override
    public void onAttachedToRecyclerView(RecyclerView recyclerView) {
        super.onAttachedToRecyclerView(recyclerView);
    }

    // Insert a new item to the RecyclerView at a predefined position
    public void insert(int position, Tache data) {
        taches.add(position, data);
        notifyItemInserted(position);
    }

    // Convert base64 string to Bitmap
    public Bitmap base64ToBitmap(String base64String) {
        try {
            byte[] decodedBytes = Base64.decode(base64String, Base64.DEFAULT);
            return BitmapFactory.decodeByteArray(decodedBytes, 0, decodedBytes.length);
        } catch (IllegalArgumentException e) {
            e.printStackTrace(); // Log the error for debugging
            return null; // Return null if decoding fails
        }
    }
}
