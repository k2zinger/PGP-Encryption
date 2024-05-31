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
    [LocalizedDisplayName(nameof(Resources.VerifyKeyValidity_DisplayName))]
    [LocalizedDescription(nameof(Resources.VerifyKeyValidity_Description))]
    public class VerifyKeyValidity : ContinuableAsyncNativeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.VerifyKeyValidity_FilePublicKey_DisplayName))]
        [LocalizedDescription(nameof(Resources.VerifyKeyValidity_FilePublicKey_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        [RequiredArgument]
        public InArgument<string> FilePublicKey { get; set; }

        [LocalizedDisplayName(nameof(Resources.VerifyKeyValidity_Verified_DisplayName))]
        [LocalizedDescription(nameof(Resources.VerifyKeyValidity_Verified_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<bool> Verified { get; set; }

        [LocalizedDisplayName(nameof(Resources.VerifyKeyValidity_Status_DisplayName))]
        [LocalizedDescription(nameof(Resources.VerifyKeyValidity_Status_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<string> Status { get; set; }

        #endregion


        #region Constructors

        public VerifyKeyValidity()
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
                Utilities.ValidatePublicKey(FilePublicKey.Get(context));
            }
            catch (Exception ex)
            {
                Verified.Set(context, false);
                Status.Set(context, ex.Message);
                throw new Exception(Status.Get(context));
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

