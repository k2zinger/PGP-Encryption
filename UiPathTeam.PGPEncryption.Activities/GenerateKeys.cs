using PgpCore;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace UiPathTeam.PGPEncryption.Activities
{
    [DisplayName("Generate Keys"), Description("Generate a new public and private key for the provided Identity and passphrase")]
    public class GenerateKeys : NativeActivity
    {

        #region Properties

        [Category("Input"), Description("File path to store the Public Key. The public key is the key that other people use to encrypt a message that only you can open")]
        [RequiredArgument]
        public InArgument<String> FilePublicKey { get; set; }

        [Category("Input"), Description("File path to store the Private Key. The private key is the key that allows you to decrypt the messages sent to you based on your public key")]
        [RequiredArgument]
        public InArgument<String> FilePrivateKey { get; set; }

        [Category("Input"), Description("Passphrase is a word or phrase used to encrypt your private key on your machine")]
        public InArgument<String> Passphrase { get; set; }

        [Category("Input"), Description("Recipient email address")]
        [RequiredArgument]
        public InArgument<String> Identity { get; set; }

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
                Utilities.ValidateDirectory(FilePublicKey.Get(context), "FilePublicKey", Overwrite.Get(context));

                Utilities.ValidateDirectory(FilePrivateKey.Get(context), "FilePrivateKey", Overwrite.Get(context));

                Utilities.ValidatePassphrase(Passphrase.Get(context));

                if (String.IsNullOrEmpty(Identity.Get(context)) || !System.Text.RegularExpressions.Regex.IsMatch(Identity.Get(context), "(?i)[A-Z0-9+_.-]+@(?:.*).(?:.*)"))
                {
                    throw new Exception("Valid Identity (email) required: " + Identity.Get(context));
                }
                else
                {
                    Console.WriteLine("Identity: " + Identity.Get(context));
                }

                Status.Set(context, "");
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
                var pgp = new PGP();

                try
                {
                    pgp.GenerateKey(FilePublicKey.Get(context), FilePrivateKey.Get(context), Identity.Get(context), Passphrase.Get(context));
                    if (File.Exists(FilePublicKey.Get(context)) && File.Exists(FilePrivateKey.Get(context)))
                    {
                        Result.Set(context, true);
                        Status.Set(context, "Key Generation Successful");
                        Console.WriteLine(Status.Get(context));
                    }
                    else
                    {
                        // should never get here...
                        throw new Exception("Unable to generate keys");
                    }
                }
                catch (Exception ex)
                {
                    Status.Set(context, "Key Pair Generation Failed " + Environment.NewLine + ex.Message);
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