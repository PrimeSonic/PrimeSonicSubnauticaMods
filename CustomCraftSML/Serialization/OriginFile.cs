namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;

    internal class OriginFile
    {
        private static readonly Dictionary<string, OriginFile> KnownFiles = new Dictionary<string, OriginFile>();

        internal static OriginFile GetOriginFile(string fileName)
        {
            if (KnownFiles.TryGetValue(fileName, out OriginFile file))
            {
                return file;
            }
            else
            {
                var newEntry = new OriginFile(fileName);
                KnownFiles.Add(fileName, newEntry);
                return newEntry;
            }
        }

        public readonly string FileName;

        public OriginFile(string fileName)
        {
            FileName = fileName;
        }

        public override string ToString() => $"WorkingFiles:{FileName}";
    }
}
