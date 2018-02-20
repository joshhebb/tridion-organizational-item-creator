using System.Text.RegularExpressions;
using Tridion.ContentManager.CoreService.Client;
using TridionItemCreator.Base;

namespace TridionItemCreator.StructureGroups
{
    public abstract class BaseStructureGroupBuilder : BaseItemBuilder<StructureGroupData>
    {
        public string RootSgWebDav { get; set; }

        public string StructureGroupWebDav { get; set; }

        protected BaseStructureGroupBuilder(SessionAwareCoreServiceClient client) : base(client)
        {
            Client = client;
        }

        public override ItemType GetItemType()
        {
            return ItemType.StructureGroup;
        }

        public override IdentifiableObjectData SaveOrganizationalItem(IdentifiableObjectData organizationalItemData, string itemName)
        {
            if (!(organizationalItemData is StructureGroupData)) return null;

            var structureGroupData = (StructureGroupData) organizationalItemData;
            structureGroupData.Title = itemName;
            structureGroupData.Directory = Regex.Replace(itemName, @"[^A-Za-z0-9_\.~]+", "-");

            PreProcess(structureGroupData);
            var result = Client.Save(structureGroupData, new ReadOptions());
            PostProcess(structureGroupData);

            return result;
        }

        /// <summary>
        /// Any pre processing required by implementing classes run before the save of the structure group is complete. You can use this method to set any additional data in the folder.
        /// </summary>
        /// <param name="keyword"></param>
        public abstract void PreProcess(IdentifiableObjectData structureGroup);

        /// <summary>
        /// Any post processing required by implementing classes run AFTER the save of the structure group is complete. You can use this method to create related items or anything else which you would typically do after creating the item.
        /// </summary>
        /// <param name="keyword"></param>
        public abstract void PostProcess(IdentifiableObjectData structureGroup);
    }
}