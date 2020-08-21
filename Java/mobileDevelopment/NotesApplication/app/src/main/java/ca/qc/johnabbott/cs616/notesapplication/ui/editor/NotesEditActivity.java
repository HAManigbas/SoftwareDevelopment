package ca.qc.johnabbott.cs616.notesapplication.ui.editor;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;


import ca.qc.johnabbott.cs616.notesapplication.R;
import ca.qc.johnabbott.cs616.notesapplication.model.Note;


public class NotesEditActivity extends AppCompatActivity {

    public static class params{
        public static final String initial_note = "Initial_Note";
        public static final String create_note = "New_Note";
    }

    public static class results{
        public static final String final_note = "Final_Note";
    }

    private NotesEditFragment fragment;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_notes_app);
        Toolbar toolbar = findViewById(R.id.editNote_Toolbar);
        setSupportActionBar(toolbar);

        // add back arrow to toolbar
        if (getSupportActionBar() != null){
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
            getSupportActionBar().setDisplayShowHomeEnabled(true);
        }

        fragment = (NotesEditFragment) getSupportFragmentManager().findFragmentById(R.id.fragment);

        Intent intent = getIntent();

        if(intent.getParcelableExtra(params.initial_note) != null){
            // get the note to edit
            Note curr = intent.getParcelableExtra(params.initial_note);
            fragment.setCurrNote(curr);
        }
        else if(intent.getParcelableExtra(params.create_note) != null){
            // on create a new note
            fragment.setNewNote();
        }

        toolbar.setNavigationOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                backButtonPressed();
            }
        });
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_notes_app, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        fragment.share();

        return super.onOptionsItemSelected(item);
    }

    private void backButtonPressed(){
        Intent intent = getIntent();

        // don't send resulting note if the activity is untouched
        if(fragment.getCurrNote().getModified() != null)
                intent.putExtra(results.final_note, fragment.getCurrNote());

        setResult(Activity.RESULT_OK, intent);
        finish();
    }

}
