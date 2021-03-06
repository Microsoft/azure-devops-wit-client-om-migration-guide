﻿using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MigrationGuide
{
    public class WITClientOMScenarios : IClientScenarios
    {
        private WorkItemStore WIStore { get; }
        private TfsTeamProjectCollection TPC { get; }
        private Project TeamProject { get; }
        private HashSet<int> WorkItemsAdded { get; }
        private readonly string DefaultCategoryReferenceName = "Microsoft.RequirementCategory";
        private Category DefaultWorkItemTypeCategory { get; }
        private WorkItemType DefaultWorkItemType { get; }

        public WITClientOMScenarios(string collectionUri, string project)
        {
            TPC = new TfsTeamProjectCollection(new Uri(collectionUri));
            WIStore = TPC.GetService<WorkItemStore>();
            TeamProject = WIStore.Projects[project];
            WorkItemsAdded = new HashSet<int>();
            DefaultWorkItemTypeCategory = TeamProject.Categories[DefaultCategoryReferenceName];
            DefaultWorkItemType = DefaultWorkItemTypeCategory.DefaultWorkItemType;
        }

        // Client OM Method: WorkItem()
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb133130(v%3dvs.120)
        // REST Equivalent: POST https://dev.azure.com/{organization}/{project}/_apis/wit/workitems/${type}?api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/create?view=azure-devops-rest-5.0
        [Obsolete]
        public void CreateWorkItem()
        {
            // Construct a WorkItem
            WorkItemType workItemType = TeamProject.WorkItemTypes[DefaultWorkItemType.Name];

            WorkItem wi = new WorkItem(workItemType)
            {
                // Set the values of the required fields.
                Title = "Work Item Created Using WIT OM",                
            };

            wi["System.AssignedTo"] = TPC.AuthorizedIdentity.DisplayName;

            // Save the WorkItem.
            wi.Save();
            WorkItemsAdded.Add(wi.Id);
            Console.WriteLine($"Created a work item with id:'{wi.Id}' and title: '{wi.Title}'");
            Console.WriteLine();
        }

        // Client OM Method: WorkItemStore.GetWorkItem()
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140391%28v%3dvs.120%29
        // REST Equivalent: GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitems/{id}?api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/get%20work%20item?view=azure-devops-rest-5.0
        [Obsolete]
        public void GetWorkItem()
        {
            var wi = WIStore.GetWorkItem(WorkItemsAdded.First());
            Console.WriteLine($"Opened a work item with id: '{wi.Id}' and title: '{wi.Title}'");
            Console.WriteLine();
        }

        // Client OM Method: WorkItemStore.Query()
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140399%28v%3dvs.120%29
        // REST Equivalent: GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitems?ids={ids}&api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/list?view=azure-devops-rest-5.0
        [Obsolete]
        public void GetWorkItems()
        {
            WorkItem wi = new WorkItem(DefaultWorkItemType)
            {
                Title = "Work Item Created Using WIT OM"
            };
            wi.Save();
            WorkItemsAdded.Add(wi.Id);
            wi.Close();

            var workItemsList = string.Join(",", WorkItemsAdded);
            // Can get multiple work items by using a wiql query
            string wiql = $"Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.Id] In ({workItemsList})";
            WorkItemCollection wiqlQueryResults = WIStore.Query(wiql);
            Console.WriteLine($"Wiql query searching for work items in ({workItemsList}) returned {wiqlQueryResults.Count} values");
            Console.WriteLine();

            // Alternatively, you can specify the ids as a collection, and a wiql for columns to return
            string columnWiql = $"Select [System.Id], [System.Title], [System.State] From WorkItems ";
            WorkItemCollection idAndColumnWiqlResults = WIStore.Query(WorkItemsAdded.ToArray(), columnWiql);
            Console.WriteLine($"Query searching for work items in ({workItemsList}) returned {idAndColumnWiqlResults.Count} values");
            Console.WriteLine();
        }

        // Client OM Method: WorkItem.Fields["{FieldName}"].Value = newValue;
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb164805(v%3dvs.120)
        // REST Equivalent: PATCH https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0
        [Obsolete]
        public void UpdateExistingWorkItem()
        {
            var wi = WIStore.GetWorkItem(WorkItemsAdded.First());
            var originalTitle = wi.Title;
            var originalDescription = wi["System.Description"];

            var changedTitle = "Changed Work Item Title";
            var changedDescription = "Changed Description";
            wi.Title = changedTitle;
            wi["System.Description"] = changedDescription;

            wi.Save();
            Console.WriteLine($"Updated Existing Work Item: '{wi.Id}'. Work Item title was: '{originalTitle}', but is now '{wi.Title}'");
            Console.WriteLine($"Updated Existing Work Item: '{wi.Id}'. Work Item description was: '{originalDescription}', but is now '{wi["System.Description"] }'");
            Console.WriteLine();
        }

        // Client OM Method: WorkItem.IsValid(), WorkItem.Validate()
        // Client OM Documentation:
        //      IsValid() - https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140421(v%3dvs.120)
        //      Validate() - https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140427(v%3dvs.120)
        // REST Equivalent: PATCH https://dev.azure.com/{organization}/{project}/_apis/wit/workitems/{id}?validateOnly={validateOnly}&api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0
        [Obsolete]
        public void ValidateWorkItem()
        {
            // Construct a WorkItem, but don't set the required Title field.
            WorkItemType workItemType = TeamProject.WorkItemTypes[DefaultWorkItemType.Name];

            WorkItem wi = new WorkItem(workItemType);

            // Check to see if the work item is valid (client-side) using IsValid()
            Console.WriteLine($"WorkItem is valid: {wi.IsValid()}");

            // Attempt to save an invalid work item; we expect for this to throw an exception.
            try
            {
                wi.Save();
            }
            catch (ValidationException)
            {
                Console.WriteLine($"WorkItem '{wi.Id}' threw a ValidationException on Save() because it wasn't valid.");
            }

            // Identify the invalid fields on the work item, using WorkItem.Validate()
            var invalidFields = wi.Validate();
            if (invalidFields.Count > 0)
            {
                Console.WriteLine($"WorkItem contains the following invalid fields:");
                foreach (Field field in invalidFields)
                {
                    Console.WriteLine($"Field: '{field.Name}', Status: '{field.Status}'");
                }

                Console.WriteLine();
            }

            // Fix the invalid field
            wi.Title = "Corrected the title field";

            // Check to see if the work item is valid, and save
            if (wi.IsValid())
            {
                try
                {
                    wi.Save();
                    Console.WriteLine($"Successfully saved work item: '{wi.Id}': '{wi.Title}'");
                }
                catch (ValidationException)
                {
                    Console.WriteLine($"WorkItem '{wi.Id}' threw a ValidationException on Save() because it wasn't valid.");
                }
            }
        }

        // Client OM Method: WorkItem.WorkItemLinks.Add
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140132(v%3dvs.120)
        // REST Equivalent:
        //      HTTP: PATCH https://dev.azure.com/fabrikam/_apis/wit/workitems/{id}?api-version=5.0
        //      Request Body: 
        /*          [
                      {
                        "op": "test",
                        "path": "/rev",
                        "value": 3
                      },
                      {
                        "op": "add",
                        "path": "/relations/-",
                        "value": {
                          "rel": "System.LinkTypes.Dependency-forward",
                          "url": "https://dev.azure.com/fabrikam/_apis/wit/workItems/300",
                          "attributes": {
                            "comment": "Making a new link for the dependency"
                          }
                        }
                      }
                    ]
         */
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0#add_a_link
        [Obsolete]
        public void LinkExistingWorkItem()
        {
            // Get the first item 
            var firstWorkItem = WIStore.GetWorkItem(WorkItemsAdded.First());

            // Create a new work item
            var secondWorkItem = new WorkItem(DefaultWorkItemType)
            {
                Title = "Second work item created"
            };

            secondWorkItem.Save();

            // Need to know the type of link type ends available to the project
            var linkTypeEnds = WIStore.WorkItemLinkTypes.LinkTypeEnds;

            // Create a new work item type link with the specified type and the work item to point to
            // Access WorkItemLinkTypeEnd by specifying ImmutableName as index
            var relatedLinkTypeEnd = linkTypeEnds["System.LinkTypes.Related-Forward"];
            var workItemLink = new WorkItemLink(relatedLinkTypeEnd, secondWorkItem.Id);

            // Add the work item link to the desired work item and save
            firstWorkItem.WorkItemLinks.Add(workItemLink);
            firstWorkItem.Save();

            Console.WriteLine($"Added a link from existing work item '{secondWorkItem.Id}' to '{firstWorkItem.Id}.' Work Item: '{firstWorkItem.Id}' contains a link to '{secondWorkItem.Id}': {firstWorkItem.Links.Contains(workItemLink)}");
            firstWorkItem.Links.Contains(workItemLink);
            Console.WriteLine();
        }

        // Client OM Method: WorkItem.History or WorkItem["System.History"]
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb164807(v%3dvs.120)
        // REST Equivalent:
        //      HTTP: PATCH https://dev.azure.com/fabrikam/_apis/wit/workitems/{id}?api-version=5.0
        /*      Request Body:
         *          [
                      {
                        "op": "add",
                        "path": "/fields/System.History",
                        "value": "Added a new comment"
                      }
                    ]
         */
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0#update_a_field
        [Obsolete]
        public void AddComment()
        {
            var wi = WIStore.GetWorkItem(WorkItemsAdded.First());

            // The value for field 'System.History' at the current revision is always an empty string.
            var currentHistoryValue = wi.History;
            Console.WriteLine($"WorkItem History is empty at the current revision: '{String.IsNullOrEmpty(currentHistoryValue)}'");

            var commentToAdd = "Added a new comment";
            wi.History = commentToAdd;
            wi.Save();

            // In the WIT model, when the history field is saved, the recent change is persisted in the previous revision and current revision becomes empty.
            Console.WriteLine($"Updated Existing Work Item: '{wi.Id}'. Added comment: '{wi.Revisions[wi.Revisions.Count - 1].Fields[CoreField.History].Value}' to last revision");
            Console.WriteLine();
        }

        // Client OM Method: WorkItem.Links.Add(new HyperLink("https://www.microsoft.com"));
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140133%28v%3dvs.120%29
        // REST Equivalent:
        //      HTTP: PATCH https://dev.azure.com/fabrikam/_apis/wit/workitems/{id}?api-version=5.0 
        //      Request Body: 
        /*          [
                      {
                        "op": "test",
                        "path": "/rev",
                        "value": 5
                      },
                      {
                        "op": "add",
                        "path": "/fields/System.History",
                        "value": "Linking to a blog article for context"
                      },
                      {
                        "op": "add",
                        "path": "/relations/-",
                        "value": {
                          "rel": "Hyperlink",
                          "url": "http://blogs.msdn.com/b/bharry/archive/2014/05/12/a-new-api-for-visual-studio-online.aspx"
                        }
                      }
                    ]
         */
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0#add_a_hyperlink
        [Obsolete]
        public void AddHyperLink()
        {
            var wi = WIStore.GetWorkItem(WorkItemsAdded.First());
            Hyperlink hlink = new Hyperlink("https://www.microsoft.com")
            {
                Comment = "This is a hyperlink to microsoft.com"
            };
            wi.Links.Add(hlink);
            wi.Save();
            Console.WriteLine($"Updated Existing Work Item: '{wi.Id}'. Added hyperlink: '{hlink.Location}'");
            Console.WriteLine();
        }

        // Client OM Method: WorkItem.Attachments.Add(new Attachment("Spec.txt")
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb164795%28v%3dvs.120%29
        // REST Equivalent:
        //      HTTP: PATCH https://dev.azure.com/fabrikam/_apis/wit/workitems/{id}?api-version=5.0
        //      Request Body:
        /*          [
                      {
                        "op": "test",
                        "path": "/rev",
                        "value": 3
                      },
                      {
                        "op": "add",
                        "path": "/fields/System.History",
                        "value": "Adding the necessary spec"
                      },
                      {
                        "op": "add",
                        "path": "/relations/-",
                        "value": {
                          "rel": "AttachedFile",
                          "url": "https://dev.azure.com/fabrikam/_apis/wit/attachments/098a279a-60b9-40a8-868b-b7fd00c0a439?fileName=Spec.txt",
                          "attributes": {
                            "comment": "Spec for the work"
                          }
                        }
                      }
                    ]
         * 
         */
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-5.0#add_an_attachment
        [Obsolete]
        public void AddAttachment()
        {
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using (FileStream fstream = File.Create(filePath))
            {
                using (StreamWriter swriter = new StreamWriter(fstream))
                {
                    swriter.Write("Sample attachment text");
                }
            }

            var wi = WIStore.GetWorkItem(WorkItemsAdded.First());
            Attachment newAttachment = new Attachment(filePath);
            wi.Attachments.Add(newAttachment);
            wi.Save();
            Console.WriteLine($"Updated Existing Work Item: '{wi.Id}'. Added attachment: '{newAttachment.Name}'");
            Console.WriteLine();
        }

        // Client OM Method: WorkItemStore.Query()
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140399%28v%3dvs.120%29
        // REST Equivalent: POST https://dev.azure.com/{organization}/{project}/{team}/_apis/wit/wiql?api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/wiql/query%20by%20wiql?view=azure-devops-rest-5.0
        [Obsolete]
        public void QueryByWiql()
        {
            string queryByTypeWiql = $"Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = '{DefaultWorkItemType.Name}' and [System.TeamProject] = '{TeamProject.Name}'";
            WorkItemCollection typeQueryResults = WIStore.Query(queryByTypeWiql);
            Console.WriteLine($"The wiql query returned {typeQueryResults.Count} results:");
            foreach (WorkItem result in typeQueryResults)
            {
                Console.WriteLine($"WorkItem Id: '{result.Id}' Title: '{result.Title}'");
            }
            Console.WriteLine();
        }

        // Client OM Method: WorkItemStore.Query()
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb140399%28v%3dvs.120%29
        // REST Equivalent: GET https://dev.azure.com/{organization}/{project}/{team}/_apis/wit/wiql/{id}?api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/wiql/query%20by%20id?view=azure-devops-rest-5.0
        [Obsolete]
        public void QueryById()
        {
            // Querying by ID doesn't really exist in the WIT model like it does in REST.
            // In this case, we are looking up the query definition by its ID and then creating a new Query with its corresponding wiql

            // Get an existing query and associated ID for proof of concept. If we already have a query guid, we can skip this block.
            QueryHierarchy queryHierarchy = TeamProject.QueryHierarchy;
            var queryFolder = queryHierarchy as QueryFolder;
            QueryItem queryItem = queryFolder?.Where(f => f.IsPersonal).FirstOrDefault();
            QueryFolder myQueriesFolder = queryItem as QueryFolder;

            if (myQueriesFolder != null && myQueriesFolder.Count > 0)
            {
                var queryId = myQueriesFolder.First().Id; // Replace this value with your query id

                // Get the query definition
                var queryDefinitionById = WIStore.GetQueryDefinition(queryId);
                var context = new Dictionary<string, string>() { { "project", TeamProject.Name } };

                // Obtain query results using the query definition's QueryText
                Query obj = new Query(this.WIStore, queryDefinitionById.QueryText, context);
                WorkItemCollection queryResults = obj.RunQuery();
                Console.WriteLine($"Query with name: '{queryDefinitionById.Name}' and id: '{queryDefinitionById.Id}' returned {queryResults.Count} results:");
                if (queryResults.Count > 0)
                {
                    foreach (WorkItem result in queryResults)
                    {
                        Console.WriteLine($"WorkItem Id: '{result.Id}' Title: '{result.Title}'");
                    }
                }
                else
                {
                    Console.WriteLine($"Query with name: '{queryDefinitionById.Name}' and id: '{queryDefinitionById.Id}' did not return any results.");
                    Console.WriteLine($"Try assigning work items to yourself or following work items and run the sample again.");
                }
            }

            else
            {
                Console.WriteLine("My Queries haven't been populated yet. Open up the Queries page in the browser to populate these, and then run the sample again.");
            }

            Console.WriteLine();
        }

        // Client OM Method: Project.Categories
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/ff735836%28v%3dvs.120%29
        // REST Equivalent: GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitemtypecategories?api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20type%20categories/list?view=azure-devops-rest-5.0
        [Obsolete]
        public void GetWorkItemCategories()
        {
            CategoryCollection categories = TeamProject.Categories;
            var categoriesCount = categories.Count;
            Console.WriteLine($"Project: '{TeamProject.Name}' has the following {categoriesCount} categories:");
            foreach (var category in categories)
            {
                Console.WriteLine(category.Name);
            }
            Console.WriteLine();
        }

        // Client OM Method: CategoryCollection Item
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/ff735137(v%3dvs.120)
        // REST Equivalent: GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitemtypecategories/{category}?api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20type%20categories/get?view=azure-devops-rest-5.0
        [Obsolete]
        public void GetWorkItemCategory()
        {
            CategoryCollection categories = TeamProject.Categories;

            // Can get a category via index, reference name, or Enumerable methods on Categories collection.
            var categoriesCount = categories.Count;
            var categoryReferenceName = DefaultCategoryReferenceName;
            var lastCategory = categories[categoriesCount - 1];
            Console.WriteLine($"Category at index: '{categoriesCount - 1}' in Project: '{TeamProject.Name}' has name: '{lastCategory.Name}'");
            var namedCategory = categories[categoryReferenceName];
            Console.WriteLine($"Category with reference name: '{categoryReferenceName}' in Project: '{TeamProject.Name}' has name: '{namedCategory.Name}'");
            var firstCategory = categories.First();
            Console.WriteLine($"Category accessed via First() in Project: '{TeamProject.Name}' has name: '{firstCategory.Name}'");
            Console.WriteLine();
        }

        // Client OM Method: Category.WorkItemTypes
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/ff733906%28v%3dvs.120%29
        // REST Equivalent: GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitemtypes?api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20types/list?view=azure-devops-rest-5.0
        [Obsolete]
        public void GetWorkItemTypes()
        {
            var types = TeamProject.WorkItemTypes;
            var typesCount = types.Count;
            Console.WriteLine($"Project: '{TeamProject.Name}' has the following {typesCount} types:");
            foreach (WorkItemType type in types)
            {
                Console.WriteLine($"Name: '{type.Name}' - Description: '{type.Description}'");
            }
            Console.WriteLine();
        }

        // Client OM Method: Category.WorkItemTypes
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/ff733906%28v%3dvs.120%29
        // REST Equivalent: GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitemtypes/{type}?api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20types/get?view=azure-devops-rest-5.0
        [Obsolete]
        public void GetWorkItemType()
        {
            var categoryWorkItemTypes = DefaultWorkItemTypeCategory.WorkItemTypes.ToList();

            // Can get a type via index or Enumerable methods on WorkItemTypes collection.
            var typesCount = categoryWorkItemTypes.Count();
            var lastType = categoryWorkItemTypes[typesCount - 1];
            Console.WriteLine($"Type at index: '{typesCount - 1}' in Project: '{TeamProject.Name}' for Category: '{DefaultWorkItemTypeCategory.Name}' is: '{lastType.Name}'");
            var firstType = categoryWorkItemTypes.First();
            Console.WriteLine($"Type accessed via via First() on Category: '{DefaultWorkItemTypeCategory.Name}' in Project: '{TeamProject.Name}' is: '{firstType.Name}'");
            Console.WriteLine();
        }

        // Client OM Method: WorkItemType.FieldDefinitions
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb164788%28v%3dvs.120%29
        // REST Equivalent: GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitemtypes/{type}/fields?api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20types%20field/list?view=azure-devops-rest-5.0
        [Obsolete]
        public void GetWorkItemTypeFields()
        {
            var workItemTypeFieldDefinitions = DefaultWorkItemType.FieldDefinitions;
            var fieldDefinitionsCount = workItemTypeFieldDefinitions.Count;

            Console.WriteLine($"Work Item Type: '{DefaultWorkItemType.Name}' in Category: '{DefaultWorkItemTypeCategory.Name}' in Project: '{TeamProject.Name}' has the following {fieldDefinitionsCount} Field Definitions:");
            foreach (FieldDefinition fd in workItemTypeFieldDefinitions)
            {
                Console.WriteLine(fd.Name);
            }

            Console.WriteLine();
        }

        // Client OM Method: WorkItemType.FieldDefinitions
        // Client OM Documentation: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2013/bb164788%28v%3dvs.120%29
        // REST Equivalent: GET https://dev.azure.com/{organization}/{project}/_apis/wit/workitemtypes/{type}/fields/{field}?api-version=5.0
        // REST Documentation: https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20types%20field/get?view=azure-devops-rest-5.0
        [Obsolete]
        public void GetWorkItemTypeField()
        {
            var workItemType = DefaultWorkItemType;
            var workItemTypeFieldDefinitions = workItemType.FieldDefinitions;
            var fieldDefinitionsCount = workItemTypeFieldDefinitions.Count;

            // Can access field definitions by index, or reference name
            var referenceNameField = "System.Title";
            var indexedField = fieldDefinitionsCount - 1;
            var referenceNameFieldDefinition = workItemTypeFieldDefinitions[referenceNameField];
            var lastFieldDefinition = workItemTypeFieldDefinitions[indexedField];

            Console.WriteLine($"Obtained work item type field information for '{referenceNameField}' in Category: '{DefaultWorkItemTypeCategory.Name}' in Project: '{TeamProject.Name}' using WIT Client OM.");
            Console.WriteLine($"\tField definition for '{referenceNameField}' has the following properties - Name: '{referenceNameFieldDefinition.Name}', Reference Name: '{referenceNameFieldDefinition.ReferenceName}', Field Type: '{referenceNameFieldDefinition.FieldType.ToString()}'");
            Console.WriteLine($"Obtained work item type field information for field at index '{indexedField}' in Category: '{DefaultWorkItemTypeCategory.Name}' in Project: '{TeamProject.Name}' using WIT Client OM.");
            Console.WriteLine($"\tField definition for '{indexedField}' has the following properties - Name: '{lastFieldDefinition.Name}', Reference Name: '{lastFieldDefinition.ReferenceName}', Field Type: '{lastFieldDefinition.FieldType.ToString()}'");
            Console.WriteLine();
        }
    }
}
