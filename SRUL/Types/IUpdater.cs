namespace SRUL
{
    public interface IUpdater
    {
        // Compare checksum interface
        bool CompareChecksum(string fileName, string checksum);
        bool CompareChecksum(string fileName, string checksum, string checksumType);
        
        // Download interface
        bool DownloadFile(string fileName, string url);
        
        // Extract interface
        bool ExtractFile(string fileName, string destination);
        
        // Get checksum interface
        string GetChecksum(string fileName);
        
        // Get checksum type interface
        string GetChecksumType(string fileName);
        
        // Get file size interface
        long GetFileSize(string fileName);
        
        // Get file size interface
        long GetFileSize(string fileName, string checksum);
        
        // Get file size interface
        long GetFileSize(string fileName, string checksum, string checksumType);
    }
}