package nc.grool.clinotag.composant;

import android.annotation.SuppressLint;
import android.content.Context;
import android.content.DialogInterface;
import android.content.res.ColorStateList;
import android.graphics.Color;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.Switch;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.widget.AppCompatButton;
import androidx.cardview.widget.CardView;
import androidx.recyclerview.widget.RecyclerView;

import java.util.Collections;
import java.util.List;

import nc.grool.clinotag.Globals;
import nc.grool.clinotag.R;
import nc.grool.clinotag.dto.Lieu;
import nc.grool.clinotag.dto.Tache;

public class RecyclerViewLieuAdapter extends RecyclerView.Adapter<RecyclerViewLieuAdapter.ViewHolderSite> {

    private List<Lieu> lieus = Collections.emptyList();
    private View.OnClickListener mOnItemClickListener;

    Context context;

    public RecyclerViewLieuAdapter(List<Lieu> list, Context context) {
        this.lieus = list;
        this.context = context;
    }

    @Override
    public ViewHolderSite onCreateViewHolder(ViewGroup parent, int viewType) {
        //Inflate the layout, initialize the View Holder
        View v = LayoutInflater.from(parent.getContext()).inflate(R.layout.row_lieu_layout, parent, false);
        return new ViewHolderSite(v);
    }

    @SuppressLint("ResourceAsColor")
    @Override
    public void onBindViewHolder(@NonNull ViewHolderSite holder, int position) {
        holder.btnLieu.setTag(lieus.get(position).idLieu);
        holder.btnLieu.setText(lieus.get(position).nom);

//      holder.btnLieu.setAlpha((float) 1);
        if(lieus.get(position).actionType == 1) {
            holder.btnLieu.setBackgroundTintList(ColorStateList.valueOf(Color.parseColor("#8775F8")));
        }
        else if(lieus.get(position).actionType == 2) {
            holder.btnLieu.setBackgroundTintList(ColorStateList.valueOf(Color.parseColor("#FF0000")));
        }
    }

    @Override
    public int getItemCount() {
        //returns the number of elements the RecyclerView will display
        return lieus.size();
    }

    public void setOnItemClickListener(View.OnClickListener itemClickListener) {
        mOnItemClickListener = itemClickListener;
    }

    public class ViewHolderSite extends RecyclerView.ViewHolder {

        CardView cv;
        AppCompatButton btnLieu;

        public ViewHolderSite(View itemView) {
            super(itemView);

            cv = itemView.findViewById(R.id.cardViewDistributionLieu);
            btnLieu = itemView.findViewById(R.id.lieuItem);

            itemView.setTag(this);
            itemView.setOnClickListener(mOnItemClickListener);
        }

    }

    @Override
    public void onAttachedToRecyclerView(RecyclerView recyclerView) {
        super.onAttachedToRecyclerView(recyclerView);
    }

    // Insert a new item to the RecyclerView on a predefined position
    public void insert(int position, Lieu data) {
        lieus.add(position, data);
        notifyItemInserted(position);
    }
}