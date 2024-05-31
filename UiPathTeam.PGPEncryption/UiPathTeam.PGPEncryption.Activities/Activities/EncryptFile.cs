using PgpCore;
using System;
using System.Activities;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using UiPathTeam.PGPEncryption.Activities.Properties;

namespace UiPathTeam.PGPEncryption.Activities
{
    [LocalizedDisplayName(nameof(Resources.EncryptFile_DisplayName))]
    [LocalizedDescription(nameof(Resources.EncryptFile_Description))]
    public class EncryptFile : ContinuableAsyncNativeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptFile_FileOut_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptFile_FileOut_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        [RequiredArgument]
        public OutArgument<string> FileOut { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptFile_FileIn_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptFile_FileIn_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FileIn { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptFile_FilePublicKey_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptFile_FilePublicKey_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FilePublicKey { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptFile_Overwrite_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptFile_Overwrite_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool> Overwrite { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptFile_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptFile_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Result { get; set; }

        [LocalizedDisplayName(nameof(Resources.EncryptFile_Status_DisplayName))]
        [LocalizedDescription(nameof(Resources.EncryptFile_Status_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Status { get; set; }

        #endregion


        #region Constructors

        public EncryptFile()
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
                    pgp.EncryptFile(new FileInfo(FileIn.Get(context)), new FileInfo(FileOut.Get(context)), true, true);
                    if (File.Exists(FileOut.Get(context)))
                    {
                        Result.Set(context, true);
                        Status.Set(context, "File Encryption Successful");
                        Console.WriteLine(Status.Get(context));
                    }
                    else
                    {
                        // should never get here...
                        throw new Exception("Unable to encrypt file");
                    }
                }
                catch (Exception ex)
                {
                    Status.Set(context, "File Encryption Failed " + Environment.NewLine + ex.Message);
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

