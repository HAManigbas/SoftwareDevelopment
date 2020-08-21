package ca.qc.johnabbott.cs616.notesapplication;

import android.content.Context;

import androidx.test.InstrumentationRegistry;
import androidx.test.runner.AndroidJUnit4;

import org.junit.Test;
import org.junit.runner.RunWith;

import java.text.ParseException;
import java.util.Date;

import ca.qc.johnabbott.cs616.notesapplication.model.Category;
import ca.qc.johnabbott.cs616.notesapplication.model.Note;
import ca.qc.johnabbott.cs616.notesapplication.model.NoteDatabaseHandler;
import ca.qc.johnabbott.cs616.notesapplication.sqlite.DatabaseException;

import static org.junit.Assert.*;

/**
 * Instrumented test, which will execute on an Android device.
 *
 * @see <a href="http://d.android.com/tools/testing">Testing documentation</a>
 */
@RunWith(AndroidJUnit4.class)
public class ExampleInstrumentedTest {

    private Context appContext;

    @Test
    public void useAppContext() {
        // Context of the app under test.
        appContext = InstrumentationRegistry.getInstrumentation().getTargetContext();

        assertEquals("ca.qc.johnabbott.cs616.notesapplication", appContext.getPackageName());
    }

    @Test
    public void testDatabase() throws ParseException, DatabaseException {
        appContext = InstrumentationRegistry.getInstrumentation().getTargetContext();

        NoteDatabaseHandler dbh = new NoteDatabaseHandler(appContext);
        long id = dbh.getNoteTable().create(new Note()
                .setTitle("Test Database")
                .setBody("testing")
                .setReminder(new Date())
                .setHasReminder(true)
                .setCategory(Category.PURPLE)
                .setCreated(new Date())
        );

        dbh.getNoteTable().deleteByKey(id);
    }
}
