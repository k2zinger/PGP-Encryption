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
    [LocalizedDisplayName(nameof(Resources.SignFile_DisplayName))]
    [LocalizedDescription(nameof(Resources.SignFile_Description))]
    public class SignFile : ContinuableAsyncNativeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.SignFile_FileOut_DisplayName))]
        [LocalizedDescription(nameof(Resources.SignFile_FileOut_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        [RequiredArgument]
        public virtual OutArgument<string> FileOut { get; set; }

        [LocalizedDisplayName(nameof(Resources.SignFile_FileIn_DisplayName))]
        [LocalizedDescription(nameof(Resources.SignFile_FileIn_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FileIn { get; set; }

        [LocalizedDisplayName(nameof(Resources.SignFile_FilePrivateKey_DisplayName))]
        [LocalizedDescription(nameof(Resources.SignFile_FilePrivateKey_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FilePrivateKey { get; set; }

        [LocalizedDisplayName(nameof(Resources.SignFile_Passphrase_DisplayName))]
        [LocalizedDescription(nameof(Resources.SignFile_Passphrase_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [OverloadGroup("Passphrase")]
        public InArgument<string> Passphrase { get; set; }

        [LocalizedDisplayName(nameof(Resources.SignFile_SecurePassphrase_DisplayName))]
        [LocalizedDescription(nameof(Resources.SignFile_SecurePassphrase_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [OverloadGroup("SecurePassphrase")]
        public InArgument<SecureString> SecurePassphrase { get; set; }

        [LocalizedDisplayName(nameof(Resources.SignFile_Overwrite_DisplayName))]
        [LocalizedDescription(nameof(Resources.SignFile_Overwrite_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool> Overwrite { get; set; }

        [LocalizedDisplayName(nameof(Resources.SignFile_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.SignFile_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public virtual OutArgument<bool> Result { get; set; }

        [LocalizedDisplayName(nameof(Resources.SignFile_Status_DisplayName))]
        [LocalizedDescription(nameof(Resources.SignFile_Status_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Status { get; set; }

        [Browsable(false)]
        protected string Password;

        #endregion


        #region Constructors

        public SignFile()
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

        public virtual void ValidateParameters(NativeActivityContext context)
        {
            try
            {
                Utilities.ValidateDirectory(FileOut.Get(context), "FileOut", Overwrite.Get(context));

                Utilities.ValidateFileIn(FileIn.Get(context));

                Utilities.ValidatePrivateKey(FilePrivateKey.Get(context));

                Password = Utilities.ValidatePassphrase(Passphrase.Get(context), SecurePassphrase.Get(context));
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
                    var pgp = new PGP(new EncryptionKeys(new FileInfo(FilePrivateKey.Get(context)), Password));
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