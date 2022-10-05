public class Tools {
    public static T[] Shuffle<T>(T[] array, System.Random rg) {
        for(int i = 0; i < array.Length - 1; i++) {
            int randomIndex = rg.Next(i, array.Length);

            (array[randomIndex], array[i]) = (array[i], array[randomIndex]);
        }

        return array;
    }
}