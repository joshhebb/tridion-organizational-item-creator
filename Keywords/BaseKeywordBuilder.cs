using System;
using System.Linq;
using Tridion.ContentManager.CoreService.Client;
using TridionItemCreator.Base;

namespace TridionItemCreator.Keywords
{
    public abstract class BaseKeywordBuilder : BaseItemBuilder<KeywordData>
    {
        public string CategoryWebDav { get; set; }

        public string KeywordWebDav { get; set; }

        protected BaseKeywordBuilder(SessionAwareCoreServiceClient client) : base(client)
        {
            Client = client;
        }

        public override ItemType GetItemType()
        {
            return ItemType.Keyword;
        }

        /// <summary>
        ///     Override the recursive method to create items specifically for keywords because they're stored under items, and not
        ///     organizational folders or structure groups.
        /// </summary>
        /// <param name="itemPath"></param>
        /// <param name="categoryWebdav"></param>
        /// <returns></returns>
        public IdentifiableObjectData GetOrCreateOrganizationalItem(string itemPath, string categoryWebdav)
        {
            // if the item we're searching for is the category then do nothing & return it
            if (itemPath.Equals(categoryWebdav))
            {
                return Client.Read(categoryWebdav, new ReadOptions());
            }

            // if the keyword already exists, then return it. if not - we need to create it
            var keywordName = itemPath.Split('/').Last();
            if (Client.IsExistingObject(categoryWebdav + Constants.Slash + keywordName + Constants.KeywordExt))
            {
                Console.WriteLine($"NOT SAVING '{keywordName}' - already exists. ({categoryWebdav + Constants.Slash + keywordName + Constants.KeywordExt})");
                return Client.Read(categoryWebdav + Constants.Slash + keywordName + Constants.KeywordExt, new ReadOptions());
            }

            // get the name of the item being created, and the parent item.
            var itemName = itemPath.Substring(itemPath.LastIndexOf("/") + 1);
            var parentItem = itemPath.Substring(0, itemPath.LastIndexOf("/"));

            // Recurse on the parent.
            var parentItemData = GetOrCreateOrganizationalItem(parentItem, categoryWebdav);

            KeywordData createdItem;
            if (parentItemData is CategoryData)
            {
                // Create the item to be saved.
                createdItem = (KeywordData) Client.GetDefaultData(GetItemType(), parentItemData.Id, new ReadOptions());
            }
            else
            {
                // Create the item to be saved.
                createdItem = (KeywordData) Client.GetDefaultData(GetItemType(), categoryWebdav, new ReadOptions());
                createdItem.ParentKeywords = new[] { new LinkToKeywordData { IdRef = parentItemData.Id } };
            }

            try
            {
                return SaveOrganizationalItem(createdItem, itemName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving item {itemPath} :  {ex.Message}");
            }

            // Save & return the item.
            return null;
        }

        public override IdentifiableObjectData SaveOrganizationalItem(IdentifiableObjectData organizationalItemData, string itemName)
        {
            if (!(organizationalItemData is KeywordData)) return null;

            KeywordData keywordData = (KeywordData) organizationalItemData;
            keywordData.Title = itemName;
            keywordData.Key = itemName;

            PreProcess(keywordData);
            var result = Client.Save(keywordData, new ReadOptions());
            PostProcess(keywordData);

            return result;
        }

        /// <summary>
        /// Any pre processing required by implementing classes run before the save of the keyword is complete. You can use this method to set any additional data in the keyword.
        /// </summary>
        /// <param name="keyword"></param>
        public abstract void PreProcess(IdentifiableObjectData keyword);

        /// <summary>
        /// Any post processing required by implementing classes run AFTER the save of the keyword is complete. You can use this method to create related items or anything else which you would typically do after creating the item.
        /// </summary>
        /// <param name="keyword"></param>
        public abstract void PostProcess(IdentifiableObjectData keyword);
    }
}