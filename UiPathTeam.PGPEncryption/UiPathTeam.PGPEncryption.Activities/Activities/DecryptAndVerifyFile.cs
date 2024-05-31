using PgpCore;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPathTeam.PGPEncryption.Activities.Properties;

namespace UiPathTeam.PGPEncryption.Activities
{
    [LocalizedDisplayName(nameof(Resources.DecryptAndVerifyFile_DisplayName))]
    [LocalizedDescription(nameof(Resources.DecryptAndVerifyFile_Description))]
    public class DecryptAndVerifyFile : ContinuableAsyncNativeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.DecryptAndVerifyFile_FileOut_DisplayName))]
        [LocalizedDescription(nameof(Resources.DecryptAndVerifyFile_FileOut_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        [RequiredArgument]
        public OutArgument<string> FileOut { get; set; }

        [LocalizedDisplayName(nameof(Resources.DecryptAndVerifyFile_FileIn_DisplayName))]
        [LocalizedDescription(nameof(Resources.DecryptAndVerifyFile_FileIn_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FileIn { get; set; }

        [LocalizedDisplayName(nameof(Resources.DecryptAndVerifyFile_FilePublicKey_DisplayName))]
        [LocalizedDescription(nameof(Resources.DecryptAndVerifyFile_FilePublicKey_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FilePublicKey { get; set; }

        [LocalizedDisplayName(nameof(Resources.DecryptAndVerifyFile_FilePrivateKey_DisplayName))]
        [LocalizedDescription(nameof(Resources.DecryptAndVerifyFile_FilePrivateKey_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FilePrivateKey { get; set; }

        [LocalizedDisplayName(nameof(Resources.DecryptAndVerifyFile_Passphrase_DisplayName))]
        [LocalizedDescription(nameof(Resources.DecryptAndVerifyFile_Passphrase_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [OverloadGroup("Passphrase")]
        public InArgument<string> Passphrase { get; set; }

        [LocalizedDisplayName(nameof(Resources.DecryptAndVerifyFile_SecurePassphrase_DisplayName))]
        [LocalizedDescription(nameof(Resources.DecryptAndVerifyFile_SecurePassphrase_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [OverloadGroup("SecurePassphrase")]
        public InArgument<SecureString> SecurePassphrase { get; set; }

        [LocalizedDisplayName(nameof(Resources.DecryptAndVerifyFile_Overwrite_DisplayName))]
        [LocalizedDescription(nameof(Resources.DecryptAndVerifyFile_Overwrite_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool> Overwrite { get; set; }

        [LocalizedDisplayName(nameof(Resources.DecryptAndVerifyFile_Verified_DisplayName))]
        [LocalizedDescription(nameof(Resources.DecryptAndVerifyFile_Verified_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Verified { get; set; }

        [LocalizedDisplayName(nameof(Resources.DecryptAndVerifyFile_Status_DisplayName))]
        [LocalizedDescription(nameof(Resources.DecryptAndVerifyFile_Status_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Status { get; set; }

        [Browsable(false)]
        protected string Password;

        #endregion


        #region Constructors

        public DecryptAndVerifyFile()
        {
        }

        #endregion


        #region Protected Methods

        protected override Task<Action<NativeActivityContext>> ExecuteAsync(NativeActivityContext context, CancellationToken cancellationToken)
        {
            ValidateParameters(context);

            return Task.Run(() => new Action<NativeActivityContext>(ExecuteJob));
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

