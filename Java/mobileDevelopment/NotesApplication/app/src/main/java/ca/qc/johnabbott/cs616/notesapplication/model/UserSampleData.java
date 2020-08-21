package ca.qc.johnabbott.cs616.notesapplication.model;

import android.content.Context;
import android.content.res.Resources;
import android.graphics.BitmapFactory;

import java.util.ArrayList;
import java.util.List;

import ca.qc.johnabbott.cs616.notesapplication.R;

public class UserSampleData {
    private static List<User> userList;

    public static List<User> getUsers(Context context){
        Resources res = context.getResources();

        userList = new ArrayList<>();

        userList.add(new User()
                    .setName("Ian Clement")
                    .setAvatar(BitmapFactory.decodeResource(res, R.drawable.ian))
                    .setEmail("ian@johnabbott.com")
        );

        userList.add(new User()
                .setName("Usef Faghihi")
                .setAvatar(BitmapFactory.decodeResource(res, R.drawable.usef))
                .setEmail("usef@johnabbott.com")
        );

        userList.add(new User()
                .setName("Sandy Bultena")
                .setAvatar(BitmapFactory.decodeResource(res, R.drawable.sandy))
                .setEmail("sandy@johnabbott.com")
        );

        userList.add(new User()
                .setName("Aref Mourtada")
                .setAvatar(BitmapFactory.decodeResource(res, R.drawable.aref))
                .setEmail("aref@johnabbott.com")
        );

        userList.add(new User()
                .setName("Jim Matthews")
                .setAvatar(BitmapFactory.decodeResource(res, R.drawable.jim))
                .setEmail("jim@johnabbott.com")
        );

        return userList;
    }
}
