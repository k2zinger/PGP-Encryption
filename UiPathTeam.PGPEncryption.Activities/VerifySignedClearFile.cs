using PgpCore;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace UiPathTeam.PGPEncryption.Activities
{
    [DisplayName("Verify Signed Clear File"), Description("Verify a clear signed file was signed by the matching public key")]
    public class VerifySignedClearFile : VerifySignedFile
    {

        #region Properties

        [Category("Input"), Description("The input clear signed file")]
        [RequiredArgument]
        public override InArgument<String> FileIn { get; set; }

        #endregion

        #region CodeActivity

        public override void ExecuteJob(NativeActivityContext context)
        {
            if (String.IsNullOrEmpty(Status.Get(context)))
            {
                try
                {
                    var pgp = new PGP(new EncryptionKeys(new FileInfo(FilePublicKey.Get(context))));
                    Verified.Set(context, pgp.VerifyClearFile(new FileInfo(FileIn.Get(context))));
                    Status.Set(context, Verified.Get(context) ? "Clear Signed File Verification Successful" : "Clear Signed File Verification Failed");
                    Console.WriteLine(Status.Get(context));
                }
                catch (Exception ex)
                {
                    Status.Set(context, "Clear Signed File Verification Failed " + Environment.NewLine + ex.Message);
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