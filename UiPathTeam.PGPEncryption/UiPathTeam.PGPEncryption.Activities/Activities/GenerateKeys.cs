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
    [LocalizedDisplayName(nameof(Resources.GenerateKeys_DisplayName))]
    [LocalizedDescription(nameof(Resources.GenerateKeys_Description))]
    public class GenerateKeys : ContinuableAsyncNativeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.GenerateKeys_FilePublicKey_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateKeys_FilePublicKey_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FilePublicKey { get; set; }

        [LocalizedDisplayName(nameof(Resources.GenerateKeys_FilePrivateKey_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateKeys_FilePrivateKey_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FilePrivateKey { get; set; }

        [LocalizedDisplayName(nameof(Resources.GenerateKeys_Passphrase_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateKeys_Passphrase_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [OverloadGroup("Passphrase")]
        public InArgument<string> Passphrase { get; set; }

        [LocalizedDisplayName(nameof(Resources.GenerateKeys_SecurePassphrase_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateKeys_SecurePassphrase_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [OverloadGroup("SecurePassphrase")]
        public InArgument<SecureString> SecurePassphrase { get; set; }

        [LocalizedDisplayName(nameof(Resources.GenerateKeys_KeySize_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateKeys_KeySize_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> KeySize { get; set; }

        [LocalizedDisplayName(nameof(Resources.GenerateKeys_Identity_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateKeys_Identity_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> Identity { get; set; }

        [LocalizedDisplayName(nameof(Resources.GenerateKeys_Overwrite_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateKeys_Overwrite_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool> Overwrite { get; set; }

        [LocalizedDisplayName(nameof(Resources.GenerateKeys_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateKeys_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Result { get; set; }

        [LocalizedDisplayName(nameof(Resources.GenerateKeys_Status_DisplayName))]
        [LocalizedDescription(nameof(Resources.GenerateKeys_Status_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Status { get; set; }

        [Browsable(false)]
        protected string Password;

        #endregion


        #region Constructors

        public GenerateKeys()
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
                Utilities.ValidateDirectory(FilePublicKey.Get(context), "FilePublicKey", Overwrite.Get(context));

                Utilities.ValidateDirectory(FilePrivateKey.Get(context), "FilePrivateKey", Overwrite.Get(context));

                Password = Utilities.ValidatePassphrase(Passphrase.Get(context), SecurePassphrase.Get(context));

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
                try
                {
                    var pgp = new PGP();
                    pgp.GenerateKey(new FileInfo(FilePublicKey.Get(context)), new FileInfo(FilePrivateKey.Get(context)), Identity.Get(context), Password, KeySize.Get(context));
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

