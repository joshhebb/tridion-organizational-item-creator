using System;
using Tridion.ContentManager.CoreService.Client;

namespace TridionItemCreator.Base
{
    public abstract class BaseItemBuilder<T>
    {
        protected BaseItemBuilder(SessionAwareCoreServiceClient client)
        {
            Client = client;
        }

        public SessionAwareCoreServiceClient Client { get; set; }

        public abstract ItemType GetItemType();

        public abstract IdentifiableObjectData SaveOrganizationalItem(IdentifiableObjectData organizationalItemData, string itemName);

        /// <summary>
        ///     Recursive function which either returns an existing object, or creates and returns a new object. Starts from the
        ///     deepest nested item,
        ///     which will be created cursing upwards until we find already created objects.
        /// </summary>
        /// <param name="itemPath"></param>
        /// <returns></returns>
        public virtual IdentifiableObjectData GetOrCreateOrganizationalItem(string itemPath)
        {
            if (Client.IsExistingObject(itemPath))
            {
                Console.WriteLine($"NOT SAVING - item exists '{itemPath}'");
                return Client.Read(itemPath, new ReadOptions());
            }

            // Get the name of the item being created, and the parent item.
            var itemName = itemPath.Substring(itemPath.LastIndexOf("/") + 1);
            var parentItem = itemPath.Substring(0, itemPath.LastIndexOf("/"));
            
            try
            {
                // Recurse on the parent.
                var parentItemData = GetOrCreateOrganizationalItem(parentItem);

                // Create the item to be saved.
                var createdItem = Client.GetDefaultData(GetItemType(), parentItemData.Id, new ReadOptions());

                return SaveOrganizationalItem(createdItem, itemName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving item {itemPath} :  {ex.Message}");
            }

            // Save & return the item.
            return null;
        }
    }
}