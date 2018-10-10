using System;

public class SIParseType
{
    //---------------------------------
    // Methods
    //---------------------------------

    #region public static Guid ParseGuid(string text, Guid defaultValueIfNullOrFail)

    public static Guid ParseGuid(string text, Guid defaultValueIfNullOrFail)
    {
        // Create the guid
        Guid guid = defaultValueIfNullOrFail;

        // If we have text
        if (!string.IsNullOrEmpty(text))
        {
            // Create the guid
            try
            {
                guid = new Guid(text);
            }
            catch
            {
                guid = defaultValueIfNullOrFail;
            }
        }

        // Return the guid
        return guid;
    }

    #endregion

    #region public static decimal ParseDecimal(string text, decimal defaultValueIfNullOrFail)

    public static decimal ParseDecimal(string text, decimal defaultValueIfNullOrFail)
    {
        // Create the decimal
        decimal dec = defaultValueIfNullOrFail;

        // If we have text
        if (!string.IsNullOrEmpty(text))
        {
            // Try to parse it
            if (!decimal.TryParse(text, out dec))
            {
                dec = defaultValueIfNullOrFail;
            }
        }

        // Return the dec
        return dec;
    }

    #endregion

    #region public static float ParseFloat(string text, float defaultValueIfNullOrFail)

    public static float ParseFloat(string text, float defaultValueIfNullOrFail)
    {
        // Create the decimal
        float flt = defaultValueIfNullOrFail;

        // If we have text
        if (!string.IsNullOrEmpty(text))
        {
            // Try to parse it
            if (!float.TryParse(text, out flt))
            {
                flt = defaultValueIfNullOrFail;
            }
        }

        // Return the flt
        return flt;
    }

    #endregion

    #region public static int ParseInt(string text, int defaultValueIfNullOrFail)

    public static int ParseInt(string text, int defaultValueIfNullOrFail)
    {
        // Create the decimal
        int val = defaultValueIfNullOrFail;

        // If we have text
        if (!string.IsNullOrEmpty(text))
        {
            // Try to parse it
            if (!int.TryParse(text, out val))
            {
                val = defaultValueIfNullOrFail;
            }
        }

        // Return the dec
        return val;
    }

    #endregion

    #region public static short ParseShort(string text, short defaultValueIfNullOrFail)

    public static short ParseShort(string text, short defaultValueIfNullOrFail)
    {
        // Create the decimal
        short val = defaultValueIfNullOrFail;

        // If we have text
        if (!string.IsNullOrEmpty(text))
        {
            // Try to parse it
            if (!short.TryParse(text, out val))
            {
                val = defaultValueIfNullOrFail;
            }
        }

        // Return the dec
        return val;
    }

    #endregion

    #region public static long ParseLong(string text, long defaultValueIfNullOrFail)

    public static long ParseLong(string text, long defaultValueIfNullOrFail)
    {
        // Create the decimal
        long val = defaultValueIfNullOrFail;

        // If we have text
        if (!string.IsNullOrEmpty(text))
        {
            // Try to parse it
            if (!long.TryParse(text, out val))
            {
                val = defaultValueIfNullOrFail;
            }
        }

        // Return the dec
        return val;
    }

    #endregion

    #region public static string ParseString(string text, string defaultValueIfNullOrFail)

    public static string ParseString(string text, string defaultValueIfNullOrFail)
    {
        // Create the decimal
        string val = defaultValueIfNullOrFail;

        // If we have text
        if (!string.IsNullOrEmpty(text))
        {
            val = text;
        }

        // Return the dec
        return val;
    }

    #endregion
}