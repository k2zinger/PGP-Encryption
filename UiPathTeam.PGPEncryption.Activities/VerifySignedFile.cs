﻿using PgpCore;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace UiPathTeam.PGPEncryption.Activities
{
    [DisplayName("Verify Signed File"), Description("Verify a file was signed by the matching public key")]
    public class VerifySignedFile : NativeActivity
    {

        #region Properties

        [Category("Input"), Description("The input signed file")]
        [RequiredArgument]
        public virtual InArgument<String> FileIn { get; set; }

        [Category("Input"), Description("File path to read in the Public Key")]
        [RequiredArgument]
        public InArgument<String> FilePublicKey { get; set; }

        [Category("Output"), Description("True if Signature matches with Public Key")]
        public OutArgument<Boolean> Verified { get; set; }

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
                Utilities.ValidateFileIn(FileIn.Get(context));

                Utilities.ValidatePublicKey(FilePublicKey.Get(context));
            }
            catch (Exception ex)
            {
                Status.Set(context, ex.Message);
                Console.WriteLine(ex);
            }
        }

        public virtual void ExecuteJob(NativeActivityContext context)
        {
            if (String.IsNullOrEmpty(Status.Get(context)))
            {
                try
                {
                    var pgp = new PGP(new EncryptionKeys(new FileInfo(FilePublicKey.Get(context))));
                    Verified.Set(context, pgp.VerifyFile(new FileInfo(FileIn.Get(context))));
                    Status.Set(context, Verified.Get(context) ? "Signed File Verification Successful" : "Signed File Verification Failed");
                    Console.WriteLine(Status.Get(context));
                }
                catch (Exception ex)
                {
                    Status.Set(context, "SignedFile Verification Failed " + Environment.NewLine + ex.Message);
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