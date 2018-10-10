using System.IO;
using System.Runtime.Serialization;

namespace RedHill.SalesInsight.DAL.Utilities
{
    public class SIDeepCopy
    {
        #region public static T DeepCopy<T>(T objectToCopy)

        public static T DeepCopy<T>(T objectToCopy)
        {
            // Get the stream
            using(MemoryStream ms = new MemoryStream())
            {
                // Create the serializer
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));

                // Write the object to memory
                serializer.WriteObject(ms, objectToCopy);

                // Set the pos
                ms.Position = 0;

                // Read the object
                return (T)serializer.ReadObject(ms);
            }
        }

        #endregion
    }
}