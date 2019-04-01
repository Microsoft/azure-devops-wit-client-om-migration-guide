
This guide is to help you migrate your .NET code from using [WIT Client OM](https://www.nuget.org/packages/Microsoft.TeamFoundationServer.ExtendedClient) to our REST based [.Net Client Libraries](https://docs.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=azure-devops). Below is table of the common work item tracking scenerios with links back to its respected API documentation.

# Common Scenerios

| Scenerio                                          | WIT Client OM                                                                                                                                                                                                                                                         | REST based                                                                                                                                                            |
| ------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Get list of work items                            | [WorkItemStore.Query](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140399%28v%3dvs.120%29)                                                                                                                                    | [Work Items - List](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/list?view=azure-devops-rest-5.0)                                          |
| Get single work item                              | [WorkItemStore.GetWorkItem](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140391%28v%3dvs.120%29)                                                                                                                              | [Work Items - Get Work Item](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/get%20work%20item?view=azure-devops-rest-5.0)                    |
| Create new work item                              | [WorkItem](<https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb179831(v%3dvs.120)>)                                                                                                                                                 | [Work Items - Create](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/create?view=azure-devops-rest-5.0)                                      |
| Update existing work item                         | [WorkItem.Fields](<https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb164805(v%3dvs.120)>)                                                                                                                                          | [Work Items - Update - Update a field](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0#update_a_field)      |
| Validate a work item                              | [WorkItem.IsValid()](<https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140421(v%3dvs.120)>),<br/>[WorkItem.Validate()](<https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140427(v%3dvs.120)>) | [Work Items - Update - Validate only](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0#validate_only_update) |
| Create a link to an existing work item            | [WorkItem.WorkItemLinks.Add](<https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140132(v%3dvs.120)>)                                                                                                                               | [Work Item - Update - Add a link](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0#add_a_link)               |
| Add a comment                                     | [WorkItem.History](<https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb164807(v%3dvs.120)>)                                                                                                                                         | [Work Item - Update - Update a field](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0#update_a_field)       |
| Create a hyperlink                                | [WorkItem.Links.Add()](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140133%28v%3dvs.120%29)                                                                                                                                   | [Work Item - Update - Add a hyperlink](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0#add_a_hyperlink)     |
| Add an attachment                                 | [WorkItem.Attachments.Add()](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb164795%28v%3dvs.120%29)                                                                                                                             | [Work Item - Update - Add an attachment](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0#add_an_attachment) |
| Query work items using WIQL                       | [WorkItemStore.Query()](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140399%28v%3dvs.120%29)                                                                                                                                  | [Wiql - Query by Wiql](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/wiql/query%20by%20wiql?view=azure-devops-rest-5.0)                                  |
| Run an existing query to get a list of work items | [WorkItemStore.Query()](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140399%28v%3dvs.120%29)                                                                                                                                  | [Wiql - Query by Id](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/wiql/query%20by%20id?view=azure-devops-rest-5.0)                                      |
| Get list of work item types for your project      | [Category.WorkItemTypes](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/ff733906%28v%3dvs.120%29)                                                                                                                                 | [Work Item Types - List](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20types/list?view=azure-devops-rest-5.1)                              |
| Get work item type details                        | [Category.WorkItemTypes](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/ff733906%28v%3dvs.120%29)                                                                                                                                 | [Work Item Types - Get](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20types/get?view=azure-devops-rest-5.0)                                |
| Get list of fields for a work item type           | [WorkItemType.FieldDefinitions](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb164788%28v%3dvs.120%29)                                                                                                                          | [Work Item Types Field - List](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20types%20field/list?view=azure-devops-rest-5.0)                |
| Get field details                                 | [WorkItemType.FieldDefinitions](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb164788%28v%3dvs.120%29)                                                                                                                          | [Work Item Types Field - Get](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20types%20field/get?view=azure-devops-rest-5.0)                  |

# Resources

- [Migration guide with code samples](https://github.com/Microsoft/azure-devops-wit-client-om-migration-guide)
- [Work Item Tracking REST API documentation](https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/?view=azure-devops-rest-5.0)

# Support

Looking for a help on a scenerio that we missed? If so, please create an new issue and we will add the scenerio directly into the repo.

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
