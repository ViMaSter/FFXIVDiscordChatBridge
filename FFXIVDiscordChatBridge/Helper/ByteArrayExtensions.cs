namespace FFXIVDiscordChatBridge.Helper;

static class ByteArrayExtensions
{
    static readonly int[] Empty = new int[0];

    public static int[] Locate(this byte[] self, byte?[] candidate)
    {
        if (IsEmptyLocate(self, candidate))
            return Empty;

        var list = new List<int>();

        for (int i = 0; i < self.Length; i++)
        {
            if (!self.IsMatch(i, candidate))
                continue;

            list.Add(i);
        }

        return list.Count == 0 ? Empty : list.ToArray();
    }

    static bool IsMatch(this byte[] array, int position, byte?[] candidate)
    {
        if (candidate.Length > (array.Length - position))
            return false;

        for (int i = 0; i < candidate.Length; i++)
        {
            if (candidate[i] == null)
                continue;

            if (array[position + i] != candidate[i])
                return false;
        }

        return true;
    }

    static bool IsEmptyLocate(byte[] array, byte?[] candidate)
    {
        return array == null
               || candidate == null
               || array.Length == 0
               || candidate.Length == 0
               || candidate.Length > array.Length;
    }
}