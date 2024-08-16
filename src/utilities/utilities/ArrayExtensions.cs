namespace utilities
{
    public static class ArrayExtensions
    {
        public static S[] MapTo<T, S>(this T[] input, Func<T,S> mapper)
        {
            S[] output = new S[input.Length];
            foreach (var item in input)
            {
                output.Append(mapper(item));
            }

            return output;
        }
    }
}
