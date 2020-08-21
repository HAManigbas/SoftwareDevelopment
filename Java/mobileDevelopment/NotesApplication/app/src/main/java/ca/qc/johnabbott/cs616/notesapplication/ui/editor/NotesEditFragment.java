package ca.qc.johnabbott.cs616.notesapplication.ui.editor;

import android.app.DatePickerDialog;
import android.app.TimePickerDialog;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CompoundButton;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.TimePicker;
import android.widget.Toast;

import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.fragment.app.Fragment;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import ca.qc.johnabbott.cs616.notesapplication.R;
import ca.qc.johnabbott.cs616.notesapplication.model.Category;
import ca.qc.johnabbott.cs616.notesapplication.model.Collaborator;
import ca.qc.johnabbott.cs616.notesapplication.model.Note;
import ca.qc.johnabbott.cs616.notesapplication.model.User;
import ca.qc.johnabbott.cs616.notesapplication.sqlite.DatabaseException;
import ca.qc.johnabbott.cs616.notesapplication.ui.list.NoteListFragment;
import ca.qc.johnabbott.cs616.notesapplication.ui.util.AddCollaboratorDialogFragment;
import ca.qc.johnabbott.cs616.notesapplication.ui.util.CircleView;
import ca.qc.johnabbott.cs616.notesapplication.ui.util.DatePickerDialogFragment;
import ca.qc.johnabbott.cs616.notesapplication.ui.util.DisplayUsersFragment;
import ca.qc.johnabbott.cs616.notesapplication.ui.util.TimePickerDialogFragment;


/**
 * A placeholder fragment containing a simple view.
 */
public class NotesEditFragment extends Fragment {

    //will only use one instance of calendar for choosing reminder date
    final Calendar calendar= Calendar.getInstance();

    //fields for the views
    private ConstraintLayout mainLayout;
    private LinearLayout popUpLayout;
    private EditText titleEditText;
    private EditText bodyEditText;
    private ImageButton revertBtn;
    private Switch showOptionSwitch;
    private List<CircleView> colorCircleView;
    private TextView reminderTextView;
    private DisplayUsersFragment displayUserFragment;

    //field for cloning the note
    private List<Note> noteList;

    //current note (edit or create)
    private Note currNote;

    //checks if undo button is click to prevent from making new changes
    // and just get the previous look of the note
    private boolean isUndo;

    //checks for first modify, then clone a blank note
    private boolean firstChange;

    //list of all users
    private List<User> users;

    //list of users that are added into the current notw
    private List<User> currNote_Users;

    // usesrs that are not yet added as collaborator
    private List<User> userToDisplay;

    // list of all collaborators
    private List<Collaborator> collaboratorList;

    // current collaborator to add
    private Collaborator collaborator;


    public NotesEditFragment() {
    }

    public Note getCurrNote() {
        System.out.println(currNote.toString());
        return currNote;
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View root = inflater.inflate(R.layout.fragment_notes_app, container, false);

        colorCircleView = new ArrayList<>();
        noteList = new ArrayList<>();

        //default calendar date is today's date
        calendar.setTime(new Date());
        calendar.add(Calendar.DAY_OF_YEAR, 1);

        isUndo = false;
        firstChange = true;

        //gets the id of the views
        mainLayout = root.findViewById(R.id.main_ConstraintLayout);
        popUpLayout = root.findViewById(R.id.popUp_linearLayout);
        showOptionSwitch = root.findViewById(R.id.showOptions_switch);
        titleEditText = root.findViewById(R.id.title_EditText);
        bodyEditText = root.findViewById(R.id.body_EditText);
        revertBtn = root.findViewById(R.id.revert_Button);
        reminderTextView = root.findViewById(R.id.reminder_TextView);


        // read users and collaborators from the database
        try {
            users = NoteListFragment.dbh.getUserTable().readAll();
            collaboratorList = NoteListFragment.dbh.getCollaboratorTable().readAll();
            System.out.println(collaboratorList.size());
        } catch (DatabaseException e) {
            e.printStackTrace();
        }

        collaborator = new Collaborator();
        userToDisplay = new ArrayList<>();
        currNote_Users = new ArrayList<>();

        // set the default blank note
        setNewNote();

        // look for collaborators for the current note
        checkAddedCollaborators();

        // displays the users and the user add option
        displayUserFragment = (DisplayUsersFragment) getChildFragmentManager().findFragmentById(R.id.displayUsers_Fragment);
        displayUserFragment.setUsers(currNote_Users);

        colorCircleView.add((CircleView)root.findViewById(R.id.color1_CircleView));
        colorCircleView.add((CircleView)root.findViewById(R.id.color2_CircleView));
        colorCircleView.add((CircleView)root.findViewById(R.id.color3_CircleView));
        colorCircleView.add((CircleView)root.findViewById(R.id.color4_CircleView));
        colorCircleView.add((CircleView)root.findViewById(R.id.color5_CircleView));
        colorCircleView.add((CircleView)root.findViewById(R.id.color6_CircleView));
        colorCircleView.add((CircleView)root.findViewById(R.id.color7_CircleView));
        colorCircleView.add((CircleView)root.findViewById(R.id.color8_CircleView));


        //set and call the onclick listener of all the circle views
        colorCircleView.get(0).setOnClickListener(new CircleViewClickHandler());
        colorCircleView.get(1).setOnClickListener(new CircleViewClickHandler());
        colorCircleView.get(2).setOnClickListener(new CircleViewClickHandler());
        colorCircleView.get(3).setOnClickListener(new CircleViewClickHandler());
        colorCircleView.get(4).setOnClickListener(new CircleViewClickHandler());
        colorCircleView.get(5).setOnClickListener(new CircleViewClickHandler());
        colorCircleView.get(6).setOnClickListener(new CircleViewClickHandler());
        colorCircleView.get(7).setOnClickListener(new CircleViewClickHandler());


        // hide and unhide the color view and add reminder section
        showOptionSwitch.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                if(isChecked){
                    popUpLayout.setVisibility(View.VISIBLE);
                }else {
                    popUpLayout.setVisibility(View.GONE);
                }
            }
        });


        // add reminder date
        //call on the DatePickerDialogFragment and TimePickerDialogFragment
        reminderTextView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(firstChange){
                    noteList.add(currNote.clone());
                    firstChange = false;
                }
                chooseDate();
            }
        });


        titleEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                //if not doing the undo, text changed is allowed
                if(!isUndo){
                    if(firstChange){
                        noteList.add(currNote.clone());
                        firstChange = false;
                    }
                    currNote.setTitle(titleEditText.getText().toString());
                    currNote.setModified(new Date());
                    noteList.add(currNote.clone());
                    System.out.println(noteList.size() + " Change Title");
                }
            }

            @Override
            public void afterTextChanged(Editable s) {

            }
        });


        bodyEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                //if not doing the undo, text changed is allowed
                if(!isUndo){
                    if(firstChange){
                        noteList.add(currNote.clone());
                        firstChange = false;
                    }
                    currNote.setBody(bodyEditText.getText().toString());
                    currNote.setModified(new Date());
                    noteList.add(currNote.clone());
                    System.out.println(noteList.size() + " Change Body Text");
                }
            }

            @Override
            public void afterTextChanged(Editable s) {

            }
        });


        // shows the previous look, layout of the note
        revertBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                isUndo = true; //set the flag undo to true
                if (noteList.size() != 0) {
                    Note prevNote = noteList.remove(noteList.size() - 1);
                    undoNote(prevNote);
                    System.out.println(noteList.size() + " Undo");
                }
                else
                    System.out.println("Note List Empty");

                isUndo = false; // reset the flag to allow changes again
            }
        });


        // add a collaborator to the current note
        displayUserFragment.setOnAddUserRequestedListener(new DisplayUsersFragment.OnAddUserRequestedListener() {
            @Override
            public void onAddUserRequested() {
                // look for users that are not collaborator
                // those users will be the obe to display on the add collaborator dialog fragment
                for (User u: users) {
                    if(!currNote_Users.contains(u))
                        userToDisplay.add(u);
                }
                addUser();
            }
        });

        return  root;
    }

    private void undoNote(Note note) {
        titleEditText.setText(note.getTitle());
        bodyEditText.setText(note.getBody());

        int catColorID = getResources().getColor(note.getCategory().getColorId(), null);
        mainLayout.setBackgroundColor(catColorID);

        if(note.getReminder() != null)
            reminderTextView.setText(note.getReminder().toString());
        else
            reminderTextView.setText("Add Reminder");
    }

    private void checkAddedCollaborators(){
        for (Collaborator c: collaboratorList) {
            if(c.getNoteId() == currNote.getId()){
                System.out.println("found collaborator");
                for (User u: users) {
                    if(c.getUserId() == u.getId()){
                        if(!currNote_Users.contains(u)){
                            currNote_Users.add(u);
                            System.out.println("added collaborator to current collaborators");
                        }
                    }
                }
            }
        }
    }

    private void addUser(){
        //displays add collaborator dialog fragment
        AddCollaboratorDialogFragment addCollaboratorDialogFragment = new AddCollaboratorDialogFragment(userToDisplay);
        addCollaboratorDialogFragment.show(this.getFragmentManager(), "add collaborators");

        // collaborator is chosen
        addCollaboratorDialogFragment.setOnChosenCollaboratorListener(new AddCollaboratorDialogFragment.OnChosenCollaboratorListener() {
            @Override
            public void onChosenCollaborator(User chosenUser) {

                // add user to display on the display user fragment
                displayUserFragment.add(chosenUser);

                // set the current chosen collaborator
                collaborator.setUserId(chosenUser.getId());
                collaborator.setNoteId(currNote.getId());

                // add collaborator into the database
                try {
                    NoteListFragment.dbh.getCollaboratorTable().create(collaborator);
                } catch (DatabaseException e) {
                    e.printStackTrace();
                }

                // remove the user to the list of unchosen users
                userToDisplay.remove(chosenUser);
            }
        });
    }



    private void chooseDate() {
        final TimePickerDialogFragment timeDialog = TimePickerDialogFragment.create(calendar.getTime(), new TimePickerDialog.OnTimeSetListener() {
            @Override
            public void onTimeSet(TimePicker view, int hourOfDay, int minute) {
                calendar.set(Calendar.HOUR_OF_DAY, hourOfDay);
                calendar.set(Calendar.MINUTE, minute);

                Toast.makeText(getContext(), calendar.getTime().toString(), Toast.LENGTH_LONG).show();

                currNote.setHasReminder(true);
                currNote.setReminder(calendar.getTime());
                currNote.setModified(new Date());
                noteList.add(currNote.clone());

                System.out.println(noteList.size() + " Change Date and Time");

                reminderTextView.setText(calendar.getTime().toString());
            }
        });

        DatePickerDialogFragment dateDialog = DatePickerDialogFragment.create(calendar.getTime(), new DatePickerDialog.OnDateSetListener() {
            @Override
            public void onDateSet(DatePicker view, int year, int month, int dayOfMonth) {
                calendar.set(Calendar.YEAR, year);
                calendar.set(Calendar.MONTH, month);
                calendar.set(Calendar.DAY_OF_MONTH, dayOfMonth);

                timeDialog.show(getFragmentManager(), "timePicker");
            }
        });

        dateDialog.show(getFragmentManager(), "datePicker");

    }

    private void changeColor(CircleView cv){
        int chosenColor = cv.getColor();

        Category[] categories = Category.values();
        Resources res = getResources();

        for (Category cat: categories) {

            int catColorID = res.getColor(cat.getColorId(), null);

            if (catColorID == chosenColor){
                System.out.println(cat.name());
                currNote.setCategory(cat);
                currNote.setModified(new Date());
            }
        }

        mainLayout.setBackgroundColor(cv.getColor());
        noteList.add(currNote.clone());
        System.out.println(noteList.size() + " Change Color");
    }

    public void share(){
        String note = noteList.get(noteList.size() - 1).toString();

        Intent sharingIntent = new Intent(android.content.Intent.ACTION_SEND);
        sharingIntent.setType("text/plain");
        sharingIntent.putExtra(android.content.Intent.EXTRA_SUBJECT,"Create Note");
        sharingIntent.putExtra(android.content.Intent.EXTRA_TEXT, note);
        startActivity(Intent.createChooser(sharingIntent, "Sharing Option"));
    }


    /*
    * set the current note to edit
    * */
    public void setCurrNote(Note toEdit){
        Resources res = getResources();

        currNote = toEdit;
        noteList.add(currNote.clone());

        firstChange = false;

        Category cat = currNote.getCategory();
        int catColorID = res.getColor(cat.getColorId(), null);

        titleEditText.setText(currNote.getTitle());
        bodyEditText.setText(currNote.getBody());
        mainLayout.setBackgroundColor(catColorID);

        if(currNote.isHasReminder()){
            reminderTextView.setText(currNote.getReminder().toString());
        }

        checkAddedCollaborators();
    }

    private class CircleViewClickHandler implements View.OnClickListener{
        @Override
        public void onClick(View view) {
            if(firstChange){
                noteList.add(currNote.clone());
                firstChange = false;
            }
            changeColor((CircleView) view);
        }
    }

    public void setNewNote(){
        //default layout of the note
        currNote = new Note();
        currNote.setTitle(bodyEditText.getText().toString());
        currNote.setBody(titleEditText.getText().toString());
        currNote.setCategory(Category.values()[0]);
        currNote.setHasReminder(false);
        currNote.setCreated(new Date());
    }

}
