using System.Text.RegularExpressions;

namespace VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Utils
{
    internal class FileUtilities
    {

        public static string FileTransformation(string fileName)
        {
            //Set file name transform rules:

            fileName.Replace(" ", "-"); //Replace spaces in file name with hyphens

            //Remove special characters from file name
            fileName = Regex.Replace(fileName, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);

            //Remove multiple hyphens from file name
            fileName = Regex.Replace(fileName, "-{2,}", "-", RegexOptions.Compiled);

            //add suffix to file name before file extension
           // fileName = fileName.Insert(fileName.LastIndexOf('.'), "-INPUT");

            //add prefix to file name with date and time
            fileName = $"{DateTime.Now:yyyyMMddHHmmss}-{fileName}";

            //convert file name to lower case
            fileName = fileName.ToLower();

            //return transformed file name
            return fileName;
        }
    }
}
