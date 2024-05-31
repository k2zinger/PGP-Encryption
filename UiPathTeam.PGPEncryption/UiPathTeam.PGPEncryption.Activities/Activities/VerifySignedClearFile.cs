using PgpCore;
using System;
using System.Activities;
using System.IO;
using UiPath.Shared.Activities.Localization;
using UiPathTeam.PGPEncryption.Activities.Properties;

namespace UiPathTeam.PGPEncryption.Activities
{
    [LocalizedDisplayName(nameof(Resources.VerifySignedClearFile_DisplayName))]
    [LocalizedDescription(nameof(Resources.VerifySignedClearFile_Description))]
    public class VerifySignedClearFile : VerifySignedFile
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.VerifySignedClearFile_FileIn_DisplayName))]
        [LocalizedDescription(nameof(Resources.VerifySignedClearFile_FileIn_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public override InArgument<string> FileIn { get; set; }

        #endregion


        #region Constructors

        public VerifySignedClearFile()
        {
        }

        #endregion

        #region HelperMethods

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

