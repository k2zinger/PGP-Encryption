using System;
using System.IO;
using System.Security;

namespace UiPathTeam.PGPEncryption.Activities
{
    public static class Utilities
    {
        #region Validations

        public static void ValidateDirectory(string FilePath, string name, bool Overwrite)
        {
            if (String.IsNullOrEmpty(FilePath))
            {
                throw new Exception($"Valid {name} required: {FilePath}");
            }
            else if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
            {
                throw new Exception($"{name} directory does not exist: {Path.GetDirectoryName(FilePath)}");
            }
            else if (!Overwrite && File.Exists(FilePath))
            {
                throw new Exception($"{name} already exists: {FilePath}");
            }
            else
            {
                if (Overwrite && File.Exists(FilePath))
                {
                    Console.WriteLine("File exists, attempting to delete: " + FilePath);
                    try
                    {
                        File.Delete(FilePath);
                        if (File.Exists(FilePath))
                        {
                            // should not happen
                            throw new Exception("File not deleted");
                        }
                        Console.WriteLine("File deleted: " + FilePath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to delete file: " + ex.Message);
                    }
                }
                Console.WriteLine($"{name}: {FilePath}");
            }
        }

        public static void ValidateFileIn(string FileIn)
        {
            if (String.IsNullOrEmpty(FileIn) || !File.Exists(FileIn))
            {
                throw new Exception("Valid FileIn required: " + FileIn);
            }
            else
            {
                Console.WriteLine("FileIn: " + FileIn);
            }
        }

        public static void ValidatePublicKey(string FilePublicKey)
        {
            if (String.IsNullOrEmpty(FilePublicKey) || !File.Exists(FilePublicKey))
            {
                throw new Exception("Valid FilePublicKey required: " + FilePublicKey);
            }
            else
            {
                Console.WriteLine("FilePublicKey: " + FilePublicKey);
            }
        }

        public static void ValidatePrivateKey(string FilePrivateKey)
        {
            if (String.IsNullOrEmpty(FilePrivateKey) || !File.Exists(FilePrivateKey))
            {
                throw new Exception("Valid FilePrivateKey required: " + FilePrivateKey);
            }
            else
            {
                Console.WriteLine("FilePrivateKey: " + FilePrivateKey);
            }
        }

        public static string ValidatePassphrase(string Passphrase, SecureString SecurePassphrase)
        {
            if (SecurePassphrase != null && SecurePassphrase.Length > 0)
            {
                Console.WriteLine("Using SecurePassphrase");
                return new System.Net.NetworkCredential(string.Empty, SecurePassphrase).Password;
            }
            else if (String.IsNullOrEmpty(Passphrase))
            {
                Console.WriteLine("Passphrase not specified");
                return "";
            }
            else
            {
                Console.WriteLine("Using Passphrase");
                return Passphrase;
            }
        }

        #endregion

    }
}