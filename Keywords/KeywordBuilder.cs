using Tridion.ContentManager.CoreService.Client;

namespace TridionItemCreator.Keywords
{
    internal class KeywordBuilder : BaseKeywordBuilder
    {
        public KeywordBuilder(SessionAwareCoreServiceClient client) : base(client)
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