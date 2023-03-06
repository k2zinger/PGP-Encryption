using PgpCore;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;

namespace UiPathTeam.PGPEncryption.Activities
{
    [DisplayName("Verify Key Validity"), Description("Verify the validity period of a PGP Public Key File")]
    public class VerifyKeyValidity : NativeActivity
    {

        #region Properties

        [Category("Input"), Description("File path to read in the Public Key")]
        [RequiredArgument]
        public InArgument<String> FilePublicKey { get; set; }

        [Category("Output"), Description("True if Public Key is valid")]
        public OutArgument<Boolean> Verified { get; set; }

        [Category("Output"), Description("Public Key expiration date or errors that were encountered during the Process")]
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
                Utilities.ValidatePublicKey(FilePublicKey.Get(context));
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
                    var pgp = new PGP(new EncryptionKeys(new FileInfo(FilePublicKey.Get(context))));
                    long ValiditySeconds = pgp.EncryptionKeys.MasterKey.GetValidSeconds();
                    DateTimeOffset expiration = new DateTimeOffset(pgp.EncryptionKeys.MasterKey.CreationTime.AddSeconds(ValiditySeconds));
                    int expire = expiration.CompareTo(DateTimeOffset.Now);

                    if (ValiditySeconds == 0)
                    {
                        Verified.Set(context, true);
                        Status.Set(context, "Public Key never expires");
                    }
                    else if (expire > 0)
                    {
                        Verified.Set(context, true);
                        Status.Set(context, "Public Key expires on: " + expiration.ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    else //if (expire < 0)
                    {
                        Verified.Set(context, false);
                        Status.Set(context, "Public Key expired on: " + expiration.ToString("yyyy-MM-dd hh:mm:ss"));
                    }

                    Console.WriteLine(Status.Get(context));
                }
                catch (Exception ex)
                {
                    Status.Set(context, "Public Key validition verification Failed " + Environment.NewLine + ex.Message);
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