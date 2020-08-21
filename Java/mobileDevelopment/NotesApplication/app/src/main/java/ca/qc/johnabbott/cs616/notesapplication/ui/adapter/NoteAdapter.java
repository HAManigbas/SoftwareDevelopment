package ca.qc.johnabbott.cs616.notesapplication.ui.adapter;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import java.util.List;

import ca.qc.johnabbott.cs616.notesapplication.R;
import ca.qc.johnabbott.cs616.notesapplication.model.Note;
import ca.qc.johnabbott.cs616.notesapplication.model.NoteDatabaseHandler;

public class NoteAdapter extends RecyclerView.Adapter<NoteViewHolder> {

    private List<Note> data;
    private NoteDatabaseHandler databaseHandler;

    private NoteViewHolder holder;

    public NoteAdapter(List<Note> data, NoteDatabaseHandler databaseHandler) {
        this.data = data;
        this.databaseHandler = databaseHandler;
    }

    @NonNull
    @Override
    public NoteViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View root = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.list_item_notes, parent, false);

        holder = new NoteViewHolder(root, this, databaseHandler);

        return holder;
    }

    @Override
    public void onBindViewHolder(@NonNull NoteViewHolder holder, int position) {
        holder.set(data.get(position), data);
    }

    @Override
    public int getItemCount() {
        return data.size();
    }

}
