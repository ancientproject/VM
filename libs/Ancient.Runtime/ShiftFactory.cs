namespace ancient.runtime
{
    public class ShiftFactory : IShifter
    {
        private int prev;
        private int index;

        private ShiftFactory() {}

        public static IShifter Create(int bitIndex) => new ShiftFactory {index = bitIndex};

        public int Shift()
        {
            prev = index;
            index -= 4;
            if (index < 0) index = 0;
            return prev;
        }
    }
}