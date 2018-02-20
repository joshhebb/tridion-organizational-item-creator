using System;
using Tridion.ContentManager.CoreService.Client;
using TridionItemCreator.Folders;
using TridionItemCreator.Keywords;
using TridionItemCreator.StructureGroups;

namespace TridionItemCreator
{
    /// <summary>
    /// ItemBuilder is the class which is responsible for constructing the content manager organizational items based on the Excel Spreadsheet. 
    /// 
    /// Pass in your CS client, the publication name, the path to the XLSX spreadsheet and then call the various Create(..) methods.
    /// </summary>
    /// <author>Josh Hebb</author>
    public class ItemBuilder
    {
        private readonly string _publicationName;

        public ExcelReader ExcelReader;

        public BaseFolderBuilder FolderBuilder { get; set; }

        public BaseStructureGroupBuilder StructureGroupBuilder { get; set; }

        public BaseKeywordBuilder KeywordBuilder { get; set; }

        public string WebDavBase => Constants.Webdav + _publicationName + Constants.Slash;

        /// <summary>
        /// Create an ItemBuilder which allows you to create organizational items (keywords, folders and structure groups) quickly by passing in an Excel
        /// File with the data. Optionally - you can also pass in a secondary spreadsheet which allows you to provide further information about the items being created
        /// such as linked schemas, metadata information - etc.
        /// </summary>
        /// <param name="client">session-aware core service client</param>
        /// <param name="publicationName">name of the publication to create the item in ie '010 Schemas'</param>
        /// <param name="xlsxPath">The xlsx path of the file (if file in same directory as exe, pass "file-name.exe".</param>
        public ItemBuilder(SessionAwareCoreServiceClient client, string publicationName, string xlsxPath)
        {
            try
            {
                ExcelReader = new ExcelReader(xlsxPath);
            } 
            catch(Exception e)
            {
                throw new Exception($"Something went wrong deserializing .xlsx file(s) {e.Message}");
            }

            _publicationName = publicationName;

            FolderBuilder = new FolderBuilder(client);
            StructureGroupBuilder = new StructureGroupBuilder(client);
            KeywordBuilder = new KeywordBuilder(client);
        }

        /// <summary>
        /// Create the keywords from the xlsx file under the category named passed as input.
        /// </summary>
        /// <param name="categoryName">Name of the Category to create the keywords under (ie 'Products')</param>
        public void CreateKeywords(string categoryName)
        {
            foreach (var keywordPath in ExcelReader.PendingItems)
            {
                var categoryWebdav = WebDavBase + categoryName;
                var keywordWebdav = categoryWebdav + Constants.Slash + keywordPath;

                KeywordBuilder.CategoryWebDav = categoryWebdav;
                KeywordBuilder.KeywordWebDav = keywordPath;

                Console.WriteLine($"Creating keyword at path {keywordWebdav}");
                KeywordBuilder.GetOrCreateOrganizationalItem(keywordWebdav, categoryWebdav);
            }
        }

        /// <summary>
        /// Create the folders from the xlsx file under the folder named passed as input. 
        /// </summary>
        /// <param name="folderName">Name of the root folder to create the folders under. Leave empty to default to "Building Blocks".</param>
        public void CreateFolders(string folderName = null)
        {
            foreach (var folderPath in ExcelReader.PendingItems)
            {
                var folderWebdav = WebDavBase;
                folderWebdav += Constants.BuildingBlocks;

                // Add the folder subpath if passed in.
                folderWebdav += !string.IsNullOrEmpty(folderName)
                    ? Constants.Slash + folderName
                    : string.Empty;
                folderWebdav += Constants.Slash + folderPath;

                FolderBuilder.RootFolderWebDav = WebDavBase + folderName;
                FolderBuilder.CreatedFolderPath = folderPath;

                Console.WriteLine($"Creating folder at path {folderWebdav}");
                FolderBuilder.GetOrCreateOrganizationalItem(folderWebdav);
            }
        }

        /// <summary>
        /// Create the structure groups from the xlsx file under the structure group name passed as input.
        /// </summary>
        /// <param name="structureGroupName">Name of the structure group to create the structure groups under. Leave empty for "Root".</param>
        public void CreateStructureGroups(string structureGroupName = null)
        {
            foreach (var sgPath in ExcelReader.PendingItems)
            {
                var sgWebdav = WebDavBase;
                sgWebdav += Constants.Root;
                sgWebdav += !string.IsNullOrEmpty(structureGroupName)
                    ? Constants.Slash + structureGroupName
                    : string.Empty;
                sgWebdav += Constants.Slash + sgPath;

                StructureGroupBuilder.RootSgWebDav = WebDavBase + structureGroupName;
                StructureGroupBuilder.StructureGroupWebDav = sgPath;

                Console.WriteLine($"Creating structure group at path {sgWebdav}");
                StructureGroupBuilder.GetOrCreateOrganizationalItem(sgWebdav);
            }
        }

    }
}