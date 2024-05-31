using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using UiPathTeam.PGPEncryption.Activities.Design.Designers;
using UiPathTeam.PGPEncryption.Activities.Design.Properties;

namespace UiPathTeam.PGPEncryption.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(VerifyKeyValidity), categoryAttribute);
            builder.AddCustomAttributes(typeof(VerifyKeyValidity), new DesignerAttribute(typeof(VerifyKeyValidityDesigner)));
            builder.AddCustomAttributes(typeof(VerifyKeyValidity), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(VerifySignedFile), categoryAttribute);
            builder.AddCustomAttributes(typeof(VerifySignedFile), new DesignerAttribute(typeof(VerifySignedFileDesigner)));
            builder.AddCustomAttributes(typeof(VerifySignedFile), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(VerifySignedClearFile), categoryAttribute);
            builder.AddCustomAttributes(typeof(VerifySignedClearFile), new DesignerAttribute(typeof(VerifySignedClearFileDesigner)));
            builder.AddCustomAttributes(typeof(VerifySignedClearFile), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(SignFile), categoryAttribute);
            builder.AddCustomAttributes(typeof(SignFile), new DesignerAttribute(typeof(SignFileDesigner)));
            builder.AddCustomAttributes(typeof(SignFile), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(SignClearFile), categoryAttribute);
            builder.AddCustomAttributes(typeof(SignClearFile), new DesignerAttribute(typeof(SignClearFileDesigner)));
            builder.AddCustomAttributes(typeof(SignClearFile), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(EncryptFile), categoryAttribute);
            builder.AddCustomAttributes(typeof(EncryptFile), new DesignerAttribute(typeof(EncryptFileDesigner)));
            builder.AddCustomAttributes(typeof(EncryptFile), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(DecryptFile), categoryAttribute);
            builder.AddCustomAttributes(typeof(DecryptFile), new DesignerAttribute(typeof(DecryptFileDesigner)));
            builder.AddCustomAttributes(typeof(DecryptFile), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(GenerateKeys), categoryAttribute);
            builder.AddCustomAttributes(typeof(GenerateKeys), new DesignerAttribute(typeof(GenerateKeysDesigner)));
            builder.AddCustomAttributes(typeof(GenerateKeys), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(EncryptAndSignFile), categoryAttribute);
            builder.AddCustomAttributes(typeof(EncryptAndSignFile), new DesignerAttribute(typeof(EncryptAndSignFileDesigner)));
            builder.AddCustomAttributes(typeof(EncryptAndSignFile), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(DecryptAndVerifyFile), categoryAttribute);
            builder.AddCustomAttributes(typeof(DecryptAndVerifyFile), new DesignerAttribute(typeof(DecryptAndVerifyFileDesigner)));
            builder.AddCustomAttributes(typeof(DecryptAndVerifyFile), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
