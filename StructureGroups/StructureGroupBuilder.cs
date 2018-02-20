using Tridion.ContentManager.CoreService.Client;

namespace TridionItemCreator.StructureGroups
{
    internal class StructureGroupBuilder : BaseStructureGroupBuilder
    {
        public StructureGroupBuilder(SessionAwareCoreServiceClient client) : base(client)
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