using Tridion.ContentManager.CoreService.Client;
using TridionItemCreator;

namespace CreateItems
{
    abstract class ConsoleEntry
    { 
        static void Main(string[] args)
        {
            // Parse the input arguments.
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed<CommandLineOptions>(opts =>
                {
                    using (var client = new SessionAwareCoreServiceClient(opts.EndpointName))
                    {
                        var creator = new ItemCreator(client, opts.publicationName, opts.XlsxPath);

                        switch (opts.Type)
                        {
                            case CreationType.Folders:
                            {
                                creator.CreateFolders(opts.FolderName);
                                break;
                            }
                            case CreationType.Keywords:
                            {
                                creator.CreateKeywords(opts.CategoryName);
                                break;
                            }
                            case CreationType.StructureGroups:
                            {
                                creator.CreateStructureGroups(opts.StructureGroupName);
                                break;
                            }
                        }
                    }
                });
        }
    }
}
