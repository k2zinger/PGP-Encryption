﻿using PgpCore;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Security;

namespace UiPathTeam.PGPEncryption.Activities
{
    [DisplayName("Decrypt and Verify File"), Description("Decrypt a file using the provided private key and passphrase, then verify the file was signed by the matching public key")]
    public class DecryptAndVerifyFile : NativeActivity
    {

        #region Properties

        [Category("Input"), Description("The output decrypted and verified file")]
        [RequiredArgument]
        public InArgument<String> FileOut { get; set; }

        [Category("Input"), Description("The input encrypted and signed file")]
        [RequiredArgument]
        public InArgument<String> FileIn { get; set; }

        [Category("Input"), Description("File path to read in the Public Key")]
        [RequiredArgument]
        public InArgument<String> FilePublicKey { get; set; }

        [Category("Input"), Description("File path to read in the Private Key")]
        [RequiredArgument]
        public InArgument<String> FilePrivateKey { get; set; }

        [Category("Input"), Description("Passphrase is a word or phrase used to decrypt your private key on your machine")]
        [OverloadGroup("Passphrase")]
        public InArgument<String> Passphrase { get; set; }

        [Category("Input"), Description("SecureString Passphrase is a word or phrase used to decrypt your private key on your machine")]
        [OverloadGroup("SecurePassphrase")]
        public InArgument<SecureString> SecurePassphrase { get; set; }

        [Category("Input"), Description("Overwrite output files.  Default: False")]
        public InArgument<Boolean> Overwrite { get; set; }

        [Category("Output"), Description("Result of operation.  True if successful")]
        public OutArgument<Boolean> Verified { get; set; }

        [Category("Output"), Description("Status codes or errors that were encountered during the Process")]
        public OutArgument<String> Status { get; set; }

        private string Password;

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

                Utilities.ValidatePublicKey(FilePublicKey.Get(context));

                Utilities.ValidatePrivateKey(FilePrivateKey.Get(context));

                Password = Utilities.ValidatePassphrase(Passphrase.Get(context), SecurePassphrase.Get(context));
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
                    var pgp = new PGP(new EncryptionKeys(new FileInfo(FilePublicKey.Get(context)), new FileInfo(FilePrivateKey.Get(context)), Password));
                    pgp.DecryptFileAndVerify(new FileInfo(FileIn.Get(context)), new FileInfo(FileOut.Get(context)));
                    if (File.Exists(FileOut.Get(context)))
                    {
                        Verified.Set(context, true);
                        Status.Set(context, "File Decryption and Verification Successful");
                        Console.WriteLine(Status.Get(context));
                    }
                    else
                    {
                        // should never get here...
                        throw new Exception("Unable to decrypt and verify file");
                    }
                }
                catch (Exception ex)
                {
                    Status.Set(context, "File Decryption and Verification Failed " + Environment.NewLine + ex.Message);
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