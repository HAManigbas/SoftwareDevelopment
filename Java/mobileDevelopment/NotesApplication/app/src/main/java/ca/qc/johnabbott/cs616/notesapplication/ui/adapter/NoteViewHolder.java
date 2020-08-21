package ca.qc.johnabbott.cs616.notesapplication.ui.adapter;

import android.app.DatePickerDialog;
import android.app.TimePickerDialog;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Rect;
import android.view.ActionMode;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.widget.DatePicker;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.TimePicker;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.recyclerview.widget.RecyclerView;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import ca.qc.johnabbott.cs616.notesapplication.R;
import ca.qc.johnabbott.cs616.notesapplication.model.Category;
import ca.qc.johnabbott.cs616.notesapplication.model.Note;
import ca.qc.johnabbott.cs616.notesapplication.model.NoteDatabaseHandler;
import ca.qc.johnabbott.cs616.notesapplication.sqlite.DatabaseException;
import ca.qc.johnabbott.cs616.notesapplication.ui.editor.NotesEditActivity;
import ca.qc.johnabbott.cs616.notesapplication.ui.list.NoteListActivity;
import ca.qc.johnabbott.cs616.notesapplication.ui.list.NoteListFragment;
import ca.qc.johnabbott.cs616.notesapplication.ui.util.DatePickerDialogFragment;
import ca.qc.johnabbott.cs616.notesapplication.ui.util.TimePickerDialogFragment;

public class NoteViewHolder extends RecyclerView.ViewHolder{

    public static final SimpleDateFormat DATE_FORMAT = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

    private final View root;
    private final NoteListActivity activity;
    private final Calendar calendar = Calendar.getInstance();

    private Note note;

    private TextView titleTextView;
    private TextView bodyTextView;
    private TextView reminderDateTextView;
    private LinearLayout noteLinearLayout;
    private ConstraintLayout noteListConstraintLayout;

    private NoteAdapter noteAdapter;
    private NoteDatabaseHandler noteDatabaseHandler;
    private List<Note> data_Notes;



    public NoteViewHolder(@NonNull View root, NoteAdapter noteAdapter, NoteDatabaseHandler databaseHandler) {
        super(root);

        activity = (NoteListActivity) root.getContext();
        this.root = root;

        //to have access of the recycler view adapter
        this.noteAdapter = noteAdapter;

        // to have access on the database handler
        this.noteDatabaseHandler = databaseHandler;

        // save the views on the fields for modifying and updating
        titleTextView = root.findViewById(R.id.titleNote_TextView);
        bodyTextView = root.findViewById(R.id.bodyNote_TextView);
        reminderDateTextView = root.findViewById(R.id.reminderDateNote_TextView);
        noteLinearLayout = root.findViewById(R.id.notes_LinearLayout);
        noteListConstraintLayout = root.findViewById(R.id.noteList_ConstraintLayout);

        noteLinearLayout.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                if(motionEvent.getAction() == MotionEvent.ACTION_UP){
                    noteTouched(motionEvent);
                }
                return true;
            }
        });

    }

    public void set(Note n, List<Note> data){
        //to have access of the data inside the database
        // for CRUD operations
        this.data_Notes = data;

        // current note
        note = n;

        Resources res = root.getResources();
        Category cat = note.getCategory();

        // update the title and body text view
        titleTextView.setText(note.getTitle());
        bodyTextView.setText(note.getBody());

        // update the reminder text view
        setReminderDateTextView();

        // set the background color for each note depending on their category
        switch (cat){
            case ORANGE:
                noteListConstraintLayout.setBackgroundColor(res.getColor(R.color.orange, null));
                break;
            case PURPLE:
                noteListConstraintLayout.setBackgroundColor(res.getColor(R.color.purple, null));
                break;
            case YELLOW:
                noteListConstraintLayout.setBackgroundColor(res.getColor(R.color.yellow, null));
                break;
            case SKY_BLUE:
                noteListConstraintLayout.setBackgroundColor(res.getColor(R.color.skyBlue, null));
                break;
            case DARK_BLUE:
                noteListConstraintLayout.setBackgroundColor(res.getColor(R.color.dark_blue, null));
                break;
            case LIGHT_RED:
                noteListConstraintLayout.setBackgroundColor(res.getColor(R.color.light_red, null));
                break;
            case DARK_GREEN:
                noteListConstraintLayout.setBackgroundColor(res.getColor(R.color.darkGreen, null));
                break;
            case YELLOW_GREEN:
                noteListConstraintLayout.setBackgroundColor(res.getColor(R.color.yellowGreen, null));
                break;
        }
    }


    private void noteTouched(final MotionEvent motionEvent){
        ActionMode actionMode = activity.startActionMode(new ActionMode.Callback2() {
            @Override
            public boolean onCreateActionMode(ActionMode actionMode, Menu menu) {
                actionMode.getMenuInflater().inflate(R.menu.menu_list_floating, menu);
                return true;
            }

            @Override
            public boolean onPrepareActionMode(ActionMode actionMode, Menu menu) {
                return false;
            }

            @Override
            public boolean onActionItemClicked(ActionMode actionMode, MenuItem menuItem) {
                switch (menuItem.getItemId()){
                    case R.id.edit_MenuItem:
                        // create new intent to open notes edit activity with the current note to edit
                        Intent intent = new Intent(activity.getBaseContext(), NotesEditActivity.class);
                        intent.putExtra(NotesEditActivity.params.initial_note, note);
                        activity.startActivityForResult(intent, 1);
                        actionMode.finish();
                        break;
                    case R.id.reminder_MenuItem:
                        changeReminder();
                        actionMode.finish();
                        break;
                    case R.id.trash_MenuItem:
                        // delete current note from the database
                        try {
                            noteDatabaseHandler.getNoteTable().delete(note);
                        } catch (DatabaseException e) {
                            e.printStackTrace();
                        }

                        // delete current note from the list of notes
                        data_Notes.remove(note);

                        // update the adapter of the changes made
                        noteAdapter.notifyDataSetChanged();

                        actionMode.finish();
                        break;
                    case R.id.close_MenuItem:
                        // close floating menu
                        actionMode.finish();
                        break;
                }

                return true;
            }

            @Override
            public void onDestroyActionMode(ActionMode actionMode) {

            }

            @Override
            public void onGetContentRect(ActionMode mode, View view, Rect outRect) {
                outRect.set((int) motionEvent.getRawX(),
                        (int) motionEvent.getRawY(),
                        (int) motionEvent.getRawX(),
                        (int) motionEvent.getRawY());
            }
        }, ActionMode.TYPE_FLOATING);
    }

    private void changeReminder(){
        if(note.isHasReminder()){
            // note has reminder
            // date set to the current note's reminder
            calendar.setTime(note.getReminder());
        }
        else {
            // note has no reminder
            // set date to tomorrow's date
            calendar.setTime(new Date());
            calendar.add(Calendar.DAY_OF_YEAR, 1);
        }

        final TimePickerDialogFragment timeDialog = TimePickerDialogFragment.create(calendar.getTime(), new TimePickerDialog.OnTimeSetListener() {
            @Override
            public void onTimeSet(TimePicker view, int hourOfDay, int minute) {
                // save chosen time
                calendar.set(Calendar.HOUR_OF_DAY, hourOfDay);
                calendar.set(Calendar.MINUTE, minute);

                // save the date and time of reminder
                Date reminderDate = calendar.getTime();

                // update the current note
                note.setReminder(reminderDate);
                note.setHasReminder(true);
                note.setModified(new Date());


                //update database of the change made on the current note
                try {
                    noteDatabaseHandler.getNoteTable().update(note);
                } catch (DatabaseException e) {
                    e.printStackTrace();
                }

                // update adapter of the changes made
                noteAdapter.notifyDataSetChanged();

                // update the reminder text view
                setReminderDateTextView();

                Toast.makeText(root.getContext(), reminderDate.toString(), Toast.LENGTH_LONG).show();

            }
        });

        DatePickerDialogFragment dateDialog = DatePickerDialogFragment.create(calendar.getTime(), new DatePickerDialog.OnDateSetListener() {
            @Override
            public void onDateSet(DatePicker view, int year, int month, int dayOfMonth) {
                // save the chosen date
                calendar.set(Calendar.YEAR, year);
                calendar.set(Calendar.MONTH, month);
                calendar.set(Calendar.DAY_OF_MONTH, dayOfMonth);

                timeDialog.show(activity.getSupportFragmentManager(), "timePicker");
            }
        });

        dateDialog.show(activity.getSupportFragmentManager(), "datePicker");
    }

    private void setReminderDateTextView(){
        Resources res = root.getResources();
        Category cat = note.getCategory();

        reminderDateTextView.setText(note.getReminder() == null ? "" : DATE_FORMAT.format(note.getReminder()));

        if(note.getReminder() != null){
            reminderDateTextView.setBackgroundColor(res.getColor(R.color.reminder, null));
        }
        else {
            reminderDateTextView.setBackgroundColor(res.getColor(cat.getColorId(), null));
        }
    }
}
