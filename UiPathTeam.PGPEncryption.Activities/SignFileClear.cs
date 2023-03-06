using PgpCore;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Security;

namespace UiPathTeam.PGPEncryption.Activities
{
    [DisplayName("Sign Clear File"), Description("Clear signs a file using the provided private key and passphrase so that it is human readable")]
    public class SignClearFile : SignFile
    {

        #region Properties

        
        [Category("Input"), Description("The clear signed output file")]
        [RequiredArgument]
        public override InArgument<String> FileOut { get; set; }

        #endregion

        #region HelperMethods

        public override void ExecuteJob(NativeActivityContext context)
        {
            if (String.IsNullOrEmpty(Status.Get(context)))
            {
                try
                {
                    var pgp = new PGP(new EncryptionKeys(new FileInfo(FilePrivateKey.Get(context)), Password));
                    pgp.ClearSignFile(new FileInfo(FileIn.Get(context)), new FileInfo(FileOut.Get(context)));
                    if (File.Exists(FileOut.Get(context)))
                    {
                        Result.Set(context, true);
                        Status.Set(context, "Clear Signing File Successful");
                        Console.WriteLine(Status.Get(context));
                    }
                    else
                    {
                        // should never get here...
                        throw new Exception("Unable to clear sign file");
                    }
                }
                catch (Exception ex)
                {
                    Status.Set(context, "Clear Signing File Failed " + Environment.NewLine + ex.Message);
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