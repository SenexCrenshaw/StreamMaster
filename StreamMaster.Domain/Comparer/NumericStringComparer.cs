namespace StreamMaster.Domain.Comparer
{
    public class NumericStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            bool isXNumeric = int.TryParse(x, out int xNum);
            bool isYNumeric = int.TryParse(y, out int yNum);

            if (isXNumeric && isYNumeric)
            {
                // Both are numbers, compare numerically
                return xNum.CompareTo(yNum);
            }
            else if (!isXNumeric && !isYNumeric)
            {
                // Both are non-numbers, compare alphabetically
                return x.CompareTo(y);
            }
            else if (isXNumeric)
            {
                // x is a number, y is not, x should come first
                return -1;
            }
            else // y is a number, x is not
            {
                // y is a number, x is not, y should come first
                return 1;
            }
        }
    }

}
