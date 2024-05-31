using PgpCore;
using System;
using System.Activities;
using System.IO;
using UiPath.Shared.Activities.Localization;
using UiPathTeam.PGPEncryption.Activities.Properties;

namespace UiPathTeam.PGPEncryption.Activities
{
    [LocalizedDisplayName(nameof(Resources.SignClearFile_DisplayName))]
    [LocalizedDescription(nameof(Resources.SignClearFile_Description))]
    public class SignClearFile : SignFile
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.SignClearFile_FileOut_DisplayName))]
        [LocalizedDescription(nameof(Resources.SignClearFile_FileOut_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        [RequiredArgument]
        public override OutArgument<string> FileOut { get; set; }

        #endregion


        #region Constructors

        public SignClearFile()
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

