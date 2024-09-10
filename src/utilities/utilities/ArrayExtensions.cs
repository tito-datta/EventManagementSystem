namespace Utilities.Array;

public static class ArrayExtensions
{
    /// <summary>
    /// Maps an array of one type to an array of another type using the provided mapper function.
    /// </summary>
    /// <typeparam name="T">The type of the input array.</typeparam>
    /// <typeparam name="S">The type of the output array.</typeparam>
    /// <param name="input">The input array to be mapped.</param>
    /// <param name="mapper">The function to map elements of type T to type S.</param>
    /// <returns>An array of mapped elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown if input or mapper is null.</exception>
    public static S[] MapTo<T, S>(this T[] input, Func<T, S> mapper)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        if (mapper is null) throw new ArgumentNullException(nameof(mapper));

        S[] output = new S[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            output[i] = mapper(input[i]);
        }
        return output;
    }
}

