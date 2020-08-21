package ca.qc.johnabbott.cs616.notesapplication.model;

/**
 * Enumeration of note categories, represented as colors.
 * @author Ian Clement (ian.clement@johnabbott.qc.ca)
 */

import ca.qc.johnabbott.cs616.notesapplication.R;

public enum Category {

    PURPLE(R.color.purple),
    ORANGE(R.color.orange),
    YELLOW(R.color.yellow),
    DARK_GREEN(R.color.darkGreen),
    SKY_BLUE(R.color.skyBlue),
    DARK_BLUE(R.color.dark_blue),
    LIGHT_RED(R.color.light_red),
    YELLOW_GREEN(R.color.yellowGreen);

    private int colorId;

    // create a category with a specific color ID.
    Category(int colorId) {
        this.colorId = colorId;
    }

    /**
     * Get the category's color ID.
     * @return
     */
    public int getColorId() {
        return colorId;
    }
}
