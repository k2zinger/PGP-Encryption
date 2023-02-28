using PgpCore;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace UiPathTeam.PGPEncryption.Activities
{
    [DisplayName("Sign File"), Description("Cryptographically signs a file")]
    public class SignFile : NativeActivity
    {

        #region Properties

        [Category("Input"), Description("The output signed file")]
        [RequiredArgument]
        public InArgument<String> FileOut { get; set; }

        [Category("Input"), Description("The input file to sign")]
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
                try
                {
                    var pgp = new PGP(new EncryptionKeys(new FileInfo(FilePrivateKey.Get(context)), Passphrase.Get(context)));
                    pgp.SignFile(new FileInfo(FileIn.Get(context)), new FileInfo(FileOut.Get(context)), true);
                    if (File.Exists(FileOut.Get(context)))
                    {
                        Result.Set(context, true);
                        Status.Set(context, "File Signing Successful");
                        Console.WriteLine(Status.Get(context));
                    }
                    else
                    {
                        // should never get here...
                        throw new Exception("Unable to sign file");
                    }
                }
                catch (Exception ex)
                {
                    Status.Set(context, "File Signing Failed " + Environment.NewLine + ex.Message);
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