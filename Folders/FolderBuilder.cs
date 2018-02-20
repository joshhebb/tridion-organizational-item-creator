using Tridion.ContentManager.CoreService.Client;

namespace TridionItemCreator.Folders
{
    internal class FolderBuilder : BaseFolderBuilder
    {
        public FolderBuilder(SessionAwareCoreServiceClient client) : base(client)
        {
            Client = client;
        }

        public override void PreProcess(IdentifiableObjectData keyword)
        {

        }

        public override void PostProcess(IdentifiableObjectData keyword)
        {

        }

    }
}