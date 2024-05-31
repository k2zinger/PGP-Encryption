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
    [LocalizedDisplayName(nameof(Resources.EncryptAndSignFile_DisplayName))]
    [LocalizedDescription(nameof(Resources.EncryptAndSignFile_Description))]
    public class EncryptAndSignFile : ContinuableAsyncNativeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptAndSignFile_FileOut_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptAndSignFile_FileOut_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        [RequiredArgument]
        public OutArgument<string> FileOut { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptAndSignFile_FileIn_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptAndSignFile_FileIn_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FileIn { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptAndSignFile_FilePublicKey_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptAndSignFile_FilePublicKey_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FilePublicKey { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptAndSignFile_FilePrivateKey_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptAndSignFile_FilePrivateKey_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FilePrivateKey { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptAndSignFile_Passphrase_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptAndSignFile_Passphrase_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [OverloadGroup("Passphrase")]
        public InArgument<string> Passphrase { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptAndSignFile_SecurePassphrase_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptAndSignFile_SecurePassphrase_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [OverloadGroup("SecurePassphrase")]
        public InArgument<SecureString> SecurePassphrase { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptAndSignFile_Overwrite_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptAndSignFile_Overwrite_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool> Overwrite { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptAndSignFile_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptAndSignFile_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Result { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptAndSignFile_Status_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptAndSignFile_Status_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Status { get; set; }

        [Browsable(false)]
        protected string Password;

        #endregion


        #region Constructors

        public EncryptAndSignFile()
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
                    pgp.EncryptFileAndSign(new FileInfo(FileIn.Get(context)), new FileInfo(FileOut.Get(context)), true, true);
                    if (File.Exists(FileOut.Get(context)))
                    {
                        Result.Set(context, true);
                        Status.Set(context, "File Encryption and Signing Successful");
                        Console.WriteLine(Status.Get(context));
                    }
                    else
                    {
                        // should never get here...
                        throw new Exception("Unable to encrypt and sign file");
                    }
                }
                catch (Exception ex)
                {
                    Status.Set(context, "File Encryption and Signing Failed " + Environment.NewLine + ex.Message);
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

