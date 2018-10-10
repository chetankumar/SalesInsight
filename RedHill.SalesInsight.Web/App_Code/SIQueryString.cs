using System;
using System.Collections.Generic;
using System.Web;
using RedHill.SalesInsight.API.Exceptions;

public class SIQueryString
{
    //---------------------------------
    // Strong Typed Methods
    //---------------------------------

    #region public static Guid GetGuid(HttpRequest request, string paramName)

    public static Guid GetGuid(HttpRequest request, string paramName)
    {
        // Validate first
        ValidateRequestParam(request, paramName);

        // Return the results
        return new Guid(request[paramName]);
    }

    #endregion

    #region public static Guid GetGuid(HttpRequest request, string paramName, Guid defaultValue)

    public static Guid GetGuid(HttpRequest request, string paramName, Guid defaultValue)
    {
        return SIParseType.ParseGuid(request[paramName], defaultValue);
    }

    #endregion


    #region public static decimal GetDecimal(HttpRequest request, string paramName)

    public static decimal GetDecimal(HttpRequest request, string paramName)
    {
        // Validate first
        ValidateRequestParam(request, paramName);

        // Return the results
        return decimal.Parse(request[paramName]);
    }

    #endregion

    #region public static decimal GetDecimal(HttpRequest request, string paramName, decimal defaultValue)

    public static decimal GetDecimal(HttpRequest request, string paramName, decimal defaultValue)
    {
        return SIParseType.ParseDecimal(request[paramName], defaultValue);
    }

    #endregion


    #region public static int GetInt(HttpRequest request, string paramName)

    public static int GetInt(HttpRequest request, string paramName)
    {
        // Validate first
        ValidateRequestParam(request, paramName);

        // Return the results
        return int.Parse(request[paramName]);
    }

    #endregion

    #region public static int GetInt(HttpRequest request, string paramName, int defaultValue)

    public static int GetInt(HttpRequest request, string paramName, int defaultValue)
    {
        return SIParseType.ParseInt(request[paramName], defaultValue);
    }

    #endregion


    #region public static short GetShort(HttpRequest request, string paramName)

    public static short GetShort(HttpRequest request, string paramName)
    {
        // Validate first
        ValidateRequestParam(request, paramName);

        // Return the results
        return short.Parse(request[paramName]);
    }

    #endregion

    #region public static short GetShort(HttpRequest request, string paramName, short defaultValue)

    public static short GetShort(HttpRequest request, string paramName, short defaultValue)
    {
        return SIParseType.ParseShort(request[paramName], defaultValue);
    }

    #endregion


    #region public static long GetLong(HttpRequest request, string paramName)

    public static long GetLong(HttpRequest request, string paramName)
    {
        // Validate first
        ValidateRequestParam(request, paramName);

        // Return the results
        return long.Parse(request[paramName]);
    }

    #endregion

    #region public static long GetLong(HttpRequest request, string paramName, long defaultValue)

    public static long GetLong(HttpRequest request, string paramName, long defaultValue)
    {
        return SIParseType.ParseLong(request[paramName], defaultValue);
    }

    #endregion


    #region public static string GetString(HttpRequest request, string paramName)

    public static string GetString(HttpRequest request, string paramName)
    {
        // Validate first
        ValidateRequestParam(request, paramName);

        // Return the results
        return request[paramName];
    }

    #endregion

    #region public static string GetString(HttpRequest request, string paramName, string defaultValue)

    public static string GetString(HttpRequest request, string paramName, string defaultValue)
    {
        return SIParseType.ParseString(request[paramName], defaultValue);
    }

    #endregion


    #region public static string[] GetStringCollection(HttpRequest request, string paramName, char delimiter)

    public static string[] GetStringCollection(HttpRequest request, string paramName, char delimiter)
    {
        // Validate first
        ValidateRequestParam(request, paramName);

        // Return the results
        return GetStringCollection(request, paramName, delimiter, null);
    }

    #endregion

    #region public static string[] GetStringCollection(HttpRequest request, string paramName, char delimiter, string[] defaultValues)

    public static string[] GetStringCollection(HttpRequest request, string paramName, char delimiter, string[] defaultValues)
    {
        // Get the param
        string paramValues = SIParseType.ParseString(request[paramName], null);

        // If we got it
        if(!string.IsNullOrEmpty(paramValues))
        {
            // Parse it
            return paramValues.Split(delimiter);
        }

        // Return the default
        return defaultValues;
    }

    #endregion


    #region public static int[] GetIntCollection(HttpRequest request, string paramName, char delimiter)

    public static int[] GetIntCollection(HttpRequest request, string paramName, char delimiter)
    {
        // Validate first
        ValidateRequestParam(request, paramName);

        // Return the results
        return GetIntCollection(request, paramName, delimiter, null);
    }

    #endregion

    #region public static int[] GetIntCollection(HttpRequest request, string paramName, char delimiter, int[] defaultValues)

    public static int[] GetIntCollection(HttpRequest request, string paramName, char delimiter, int[] defaultValues)
    {
        // Get the param
        string paramValues = SIParseType.ParseString(request[paramName], null);

        // If we got it
        if (!string.IsNullOrEmpty(paramValues))
        {
            // Create the array
            List<int> collection = new List<int>();

            // Parse it
            string[] splitParamValues = paramValues.Split(delimiter);

            // Iterate
            foreach(string splitParamValue in splitParamValues)
            {
                collection.Add(int.Parse(splitParamValue));
            }

            // Return
            return collection.ToArray();
        }
        
        // Return the default
        return defaultValues;
    }

    #endregion

    //---------------------------------
    // Helper Methods
    //---------------------------------

    #region public static void ValidateRequestParam(HttpRequest request, string paramName)

    public static void ValidateRequestParam(HttpRequest request, string paramName)
    {
        // If the param is empty or null 
        if (string.IsNullOrEmpty(request[paramName]))
        {
            throw
            (
                new SIException
                (
                    true,
                    string.Format("The request parameter \"{0}\" does not exist.", paramName),
                    "The request does not contain the required parameters."
                )
            );
        }
    }

    #endregion
}
