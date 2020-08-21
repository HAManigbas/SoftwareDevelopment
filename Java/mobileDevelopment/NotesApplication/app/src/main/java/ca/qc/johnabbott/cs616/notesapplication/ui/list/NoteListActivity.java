package ca.qc.johnabbott.cs616.notesapplication.ui.list;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.coordinatorlayout.widget.CoordinatorLayout;

import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.snackbar.Snackbar;


import ca.qc.johnabbott.cs616.notesapplication.R;
import ca.qc.johnabbott.cs616.notesapplication.model.Note;
import ca.qc.johnabbott.cs616.notesapplication.ui.editor.NotesEditActivity;

public class NoteListActivity extends AppCompatActivity {

    private NoteListFragment noteListFragment;
    private CoordinatorLayout noteListCoordinatorLayout;

    // check if creating a new note
    private boolean isNewNote;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_note_list);
        Toolbar toolbar = findViewById(R.id.editNote_Toolbar);
        setSupportActionBar(toolbar);

        noteListFragment = (NoteListFragment) getSupportFragmentManager().findFragmentById(R.id.noteList_fragment);
        noteListCoordinatorLayout = findViewById(R.id.noteList_CoordinatorLayout);

        isNewNote = false;

        FloatingActionButton createNoteFloatingActionButton = findViewById(R.id.createNote_floatingActionButton);
        createNoteFloatingActionButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                isNewNote = true;

                // add a new note, open notes edit activity with blank note
                Intent intent = new Intent(getBaseContext(), NotesEditActivity.class);
                intent.putExtra(NotesEditActivity.params.create_note, "Create New Note");
                startActivityForResult(intent, 1);
            }
        });
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        // receives result from the notes edit activity
        if(resultCode == Activity.RESULT_OK){
            if(data.getParcelableExtra(NotesEditActivity.results.final_note) != null){
                final Note note = data.getParcelableExtra(NotesEditActivity.results.final_note);
                if(isNewNote){
                    // new note created
                    noteListFragment.addANote(note);
                    System.out.println("New Note Added");

                    Snackbar.make(noteListCoordinatorLayout, "Note Created", Snackbar.LENGTH_LONG)
                            .setAction("UNDO", new View.OnClickListener() {
                                @Override
                                public void onClick(View v) {
                                    noteListFragment.deleteANote(note);
                                }
                            })
                            .setActionTextColor(getResources().getColor(R.color.light_red, null))
                            .show();
                }
                else {
                    // current note edited
                    noteListFragment.updateCurrentNote(note);
                    System.out.println("Note Edited");
                }
            }
        }
    }

}
