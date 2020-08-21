package ca.qc.johnabbott.cs616.notesapplication.ui.list;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Spinner;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import java.text.ParseException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import ca.qc.johnabbott.cs616.notesapplication.R;
import ca.qc.johnabbott.cs616.notesapplication.model.Note;
import ca.qc.johnabbott.cs616.notesapplication.model.NoteDatabaseHandler;
import ca.qc.johnabbott.cs616.notesapplication.sqlite.DatabaseException;
import ca.qc.johnabbott.cs616.notesapplication.ui.adapter.NoteAdapter;

/**
 * A placeholder fragment containing a simple view.
 */
public class NoteListFragment extends Fragment {

    //Possible Sort Methods
    public static final String SORT_BY_TITLE = "Title";
    public static final String SORT_BY_CREATION_DATE = "Creation Date";
    public static final String SORT_BY_LAST_MODIFIED = "Last Modified";
    public static final String SORT_BY_REMINDER = "Reminder";
    public static final String SORT_BY_CATEGORY = "Category";

    private Spinner sortSpinner;
    private RecyclerView notesRecyclerView;
    private List<Note> notes;
    private List<String> sortingMethods;
    private String sortMethod;

    private NoteAdapter adapter_Recycler;
    private ArrayAdapter<String> adapter_Spinner;

    public static NoteDatabaseHandler dbh;

    public NoteListFragment() {
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View root = inflater.inflate(R.layout.fragment_note_list, container, false);

        try {
            dbh = new NoteDatabaseHandler(this.getActivity());
        } catch (ParseException e) {
            e.printStackTrace();
        }


        notesRecyclerView = root.findViewById(R.id.notes_RecyclerView);
        sortSpinner = root.findViewById(R.id.sort_Spinner);
        sortMethod = SORT_BY_TITLE;


        //Possible Sort Methods
        sortingMethods = new ArrayList<>();
        sortingMethods.add(SORT_BY_TITLE);
        sortingMethods.add(SORT_BY_CREATION_DATE);
        sortingMethods.add(SORT_BY_LAST_MODIFIED);
        sortingMethods.add(SORT_BY_REMINDER);
        sortingMethods.add(SORT_BY_CATEGORY);


        // set up the spinner adapter
        adapter_Spinner = new ArrayAdapter<>(getContext(), R.layout.category_sort_notes, R.id.sort_TextView);
        adapter_Spinner.addAll(sortingMethods);
        sortSpinner.setAdapter(adapter_Spinner);

        getNoteList();

        // set up the recycler adapter
        adapter_Recycler = new NoteAdapter(notes, dbh);
        notesRecyclerView.setAdapter(adapter_Recycler);
        notesRecyclerView.setLayoutManager(new GridLayoutManager(getContext(), 2));

        sortSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int pos, long id) {
                // sort the notes based on the item selected in spinner view
                sortMethod = (String)parent.getItemAtPosition(pos);
                sortNotes();
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {

            }
        });

        return  root;
    }


    private void sortNotes(){
        switch (sortMethod){
            case SORT_BY_TITLE:
                // sorts the note by title from a to z
                Collections.sort(notes, new Comparator<Note>() {
                    @Override
                    public int compare(Note note, Note rhs) {
                        return note.getTitle().compareTo(rhs.getTitle());
                    }
                });
                break;
            case SORT_BY_CREATION_DATE:
                // sort the notes by Creation Date (from latest to oldest)
                Collections.sort(notes, new Comparator<Note>(){
                    @Override
                    public int compare(Note note, Note rhs) {
                        return rhs.getCreated().compareTo(note.getCreated());
                    }
                });
                break;
            case SORT_BY_LAST_MODIFIED:
                // sort the notes by Modified Date (from latest to oldest)
                Collections.sort(notes, new Comparator<Note>(){
                    @Override
                    public int compare(Note note, Note rhs) {
                        return rhs.getModified().compareTo(note.getModified());
                    }
                });
                break;
            case SORT_BY_REMINDER:
                // sort the notes by Creation Date (from present to future and null on last)
                Collections.sort(notes, new Comparator<Note>(){
                    @Override
                    public int compare(Note note, Note rhs) {
                        if (rhs.getReminder() == null) {
                            return (note.getReminder() == null) ? 0 : -1;
                        }
                        if (note.getReminder() == null) {
                            return 1;
                        }
                        return note.getReminder().compareTo(rhs.getReminder());
                    }
                });
                break;
            case SORT_BY_CATEGORY:
                // sort the notes by Category
                Collections.sort(notes, new Comparator<Note>(){
                    @Override
                    public int compare(Note note, Note rhs) {
                        return note.getCategory().compareTo(rhs.getCategory());
                    }
                });
                break;
        }

        // update the recycler adapter of the changes made
        adapter_Recycler.notifyDataSetChanged();
    }

    public void updateCurrentNote(Note updated) {

        // update current note to the database
        try {
            dbh.getNoteTable().update(updated);
        } catch (DatabaseException e) {
            e.printStackTrace();
        }

        for (Note curr: notes) {
            // find current note from the list of notes
            if(updated.getId() == curr.getId()){
                // update current note from the note list
                curr.setTitle(updated.getTitle());
                curr.setBody(updated.getBody());
                curr.setHasReminder(updated.isHasReminder());
                curr.setReminder(updated.getReminder());
                curr.setModified(updated.getModified());

                // update recycler adapter about the change
                adapter_Recycler.notifyDataSetChanged();
                System.out.println("updated data set");
                break;
            }
        }

        //resort the notes
        sortNotes();
    }

    private void getNoteList(){
        try {
            notes = dbh.getNoteTable().readAll();
        } catch (DatabaseException e) {
            e.printStackTrace();
        }
    }

    public void addANote(Note toAdd){

        // add new note to the database
        try {
            dbh.getNoteTable().create(toAdd);
        } catch (DatabaseException e) {
            e.printStackTrace();
        }

        // add new note to the note list
        notes.add(toAdd);

        // update recycler view
        adapter_Recycler.notifyDataSetChanged();

        //resort the notes
        sortNotes();
    }

    public void deleteANote(Note toDelete) {
        // delete the new note from the database
        try {
            dbh.getNoteTable().delete(toDelete);
        } catch (DatabaseException e) {
            e.printStackTrace();
        }

        // remove new note from the list
        notes.remove(toDelete);

        // update recycler adapter
        adapter_Recycler.notifyDataSetChanged();
    }

}
