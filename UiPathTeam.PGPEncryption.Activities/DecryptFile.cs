using PgpCore;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace UiPathTeam.PGPEncryption.Activities
{
    [DisplayName("Decrypt File"), Description("Decrypt a file using the matching private key and passphrase")]
    public class DecryptFile : NativeActivity
    {

        #region Properties

        [Category("Input"), Description("The output decrypted file")]
        [RequiredArgument]
        public InArgument<String> FileOut { get; set; }

        [Category("Input"), Description("The input encrypted file")]
        [RequiredArgument]
        public InArgument<String> FileIn { get; set; }

        [Category("Input"), Description("File path to read in the Private Key")]
        [RequiredArgument]
        public InArgument<String> FilePrivateKey { get; set; }

        [Category("Input"), Description("Passphrase is a word or phrase used to decrypt your private key on your machine")]
        public InArgument<String> Passphrase { get; set; }

        [Category("Input"), Description("Overwrite output files.  Default: False")]
        public InArgument<Boolean> Overwrite { get; set; }

        [Category("Output"), Description("Result of operation.  True if successful")]
        public OutArgument<Boolean> Result { get; set; }

        [Category("Output"), Description("Status codes or errors that were encountered during the Process")]
        public OutArgument<String> Status { get; set; }

        #endregion

        #region CodeActivity

        protected override void Execute(NativeActivityContext context)
        {
            ValidateParameters(context);

            ExecuteJob(context);
        }

        #endregion

        #region HelperMethods

        public void ValidateParameters(NativeActivityContext context)
        {
            try
            {
                Utilities.ValidateDirectory(FileOut.Get(context), "FileOut", Overwrite.Get(context));

                Utilities.ValidateFileIn(FileIn.Get(context));

                Utilities.ValidatePrivateKey(FilePrivateKey.Get(context));

                Utilities.ValidatePassphrase(Passphrase.Get(context));
            }
            catch (Exception ex)
            {
                Status.Set(context, ex.Message);
                Console.WriteLine(ex);
            }
        }

        public void ExecuteJob(NativeActivityContext context)
        {
            if (String.IsNullOrEmpty(Status.Get(context)))
            {
                var pgp = new PGP(new EncryptionKeys(new FileInfo(FilePrivateKey.Get(context)), Passphrase.Get(context)));

                try
                {
                    pgp.DecryptFile(new FileInfo(FileIn.Get(context)), new FileInfo(FileOut.Get(context)));
                    if (File.Exists(FileOut.Get(context)))
                    {
                        Result.Set(context, true);
                        Status.Set(context, "File Decryption Successful");
                        Console.WriteLine(Status.Get(context));
                    }
                    else
                    {
                        // should never get here...
                        throw new Exception("Unable to decrypt file");
                    }
                }
                catch (Exception ex)
                {
                    Status.Set(context, "File Decryption Failed " + Environment.NewLine + ex.Message);
                    throw new Exception(Status.Get(context));
                }
            }
            else
            {
                throw new Exception(Status.Get(context));
            }
        }

        #endregion
    }
}