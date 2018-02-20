using Tridion.ContentManager.CoreService.Client;
using TridionItemCreator.Base;

namespace TridionItemCreator.Folders
{
    public abstract class BaseFolderBuilder : BaseItemBuilder<FolderData>
    {
        public string RootFolderWebDav { get; set; }

        public string CreatedFolderPath { get; set; }

        protected BaseFolderBuilder(SessionAwareCoreServiceClient client) : base(client)
        {
            Client = client;
        }

        public override ItemType GetItemType()
        {
            return ItemType.Folder;
        }

        public override IdentifiableObjectData SaveOrganizationalItem(IdentifiableObjectData organizationalItemData, string itemName)
        {
            if (!(organizationalItemData is FolderData)) return null;

            FolderData folderData = (FolderData) organizationalItemData;
            folderData.Title = itemName;

            PreProcess(folderData);
            var result = Client.Save(folderData, new ReadOptions());
            PostProcess(folderData);

            return result;
        }

        /// <summary>
        /// Any pre processing required by implementing classes run before the save of the folder is complete. You can use this method to set any additional data in the folder.
        /// </summary>
        /// <param name="keyword"></param>
        public abstract void PreProcess(IdentifiableObjectData folder);

        /// <summary>
        /// Any post processing required by implementing classes run AFTER the save of the folder is complete. You can use this method to create related items or anything else which you would typically do after creating the item.
        /// </summary>
        /// <param name="keyword"></param>
        public abstract void PostProcess(IdentifiableObjectData folder);

    }
}
