using NUnit.Framework;
using System.Collections.Generic;
using Tabula.Parse;
using Tabula.CST;

namespace Tabula
{
    public class TranspilerUnitTestBase
    {
        protected Parser _parser;
        protected Tokenizer _tokenizer;

        [SetUp]
        public virtual void SetUp()
        {
            _parser = new Parser();
            _tokenizer = new Tokenizer();
        }

        public ParserState StateFromString(string inputText)
        {
            var tokens = _tokenizer.Tokenize(inputText);
            return new ParserState(tokens);
        }

        protected void Assert_TokenSequenceMatches(List<Token> tokens, int fromPosition, params TokenType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                var tokenIndex = i + fromPosition;
                Assert.That(tokens.Count, Is.GreaterThan(tokenIndex),
                    $"There are only {tokens.Count} tokens, when we expect to test types from indexes {fromPosition} to {fromPosition + types.Length - 1}.");

                Assert.That(tokens[tokenIndex].Type, Is.EqualTo(types[i]),
                    $"The token at position {i + fromPosition} did not match expectations.");
            }
        }


        protected string BigBadWolf =
        @"
[Person Search, Duty Assignments, AC-16629]
Scenario: ""Advanced person search with duty assignments""

""We need duty locations active to search on them"":
use: Global Setting Management
Enable Duty Locations

""What we'll call our people in this scenario"":
set: #handle => #FullNameLF
| handle | FullNameLF             |
| Lina   | 'Frixell, Rorolina'    |
| Gio    | 'Arland, Giovanni'     |
| Sterky | 'Cranach, Sterkenberg' |
| Mimi   | 'Schwarzlang, Mimi'    |

""What we'll call the organizations they work for"":
set: #TLA => #OrganizationName
| TLA | OrganizationName             |
| HPD | Hometown Police Department   |
| OPD | Otherville Police Department |
| OA  | Otherville Academy           |


""Create our people"":
use: User Creation
Create Person named #name
| name    |
| #Lina   |
| #Gio    |
| #Sterky |
| #Mimi   |

""Create our organizations"":
use: Organization FNH Management
Create Organization named #orgName of type ""organization"" under base parent group
| orgName  |
| #HPD     |
| #OPD     |
| #OA      |

""Create list items"":
use: List Management

Create '#itemType' list item 'Tabula #item' with description 'Tabula #item description' with usage 'Available for new records'
| itemType         | item       |
| EmploymentAction | Update     |
| EmploymentType   | Civilian   |
| EmploymentType   | Employee   |
| AppointmentType  | Reserve    |
| AppointmentType  | Part Time  |
| AppointmentType  | Full Time  |
| TitleRank        | Chief      |
| TitleRank        | Instructor |
| TitleRank        | Lackey     |

""Add employments"":
use: Employment Action Edit

Browse to Add Person Employment for #empName
Add primary employment at #orgName with title `Tabula #empTitle` employment type `Tabula #empType` and appointment type ""Tabula #apptType"" starting on #startDate
| empName  | orgName  | empTitle   | empType  | apptType  | startDate  |
| #Lina    | #HPD     | Lackey     | Civilian | Contract  | 8/15/2008  |
| #Gio     | #HPD     | Lackey     | Civilian | Contract  | 8/15/2008  |
| #Sterky  | #OPD     | Lackey     | Civilian | Contract  | 8/15/2008  |
| #Mimi    | #OA      | Instructor | Reserve  | Part Time | 8/15/2012  |

alias: ""Add employment actions for #employeeName at #orgName"" => ...
    use: Employment Action Edit

    Browse to Add Employment Action for #employeeName at #orgName
    Set action to #actionName
    Set employment type to ""Tabula #empType""
    Set appointment type to ""Tabula #apptType""
    Set title to ""Tabula #empTitle""
    Set status to #newStatus
    Set effective date to #effectiveDate
    Set comments to #comment
    Click Save
.

""Add employment actions"":
Add employment actions for #Lina at #HPD
| actionName      | empType  | apptType  | empTitle   | newStatus                | effectiveDate |
| Tabula Update   | Employee | Part Time | Instructor | ""On Leave (Active)""    | 10/15/2008    |
| Separation      | Reserve  | Full Time | Chief      | ""Separated (Inactive)"" | 12/15/2008    |

Add employment actions for #Gio at #HPD
| actionName      | empType  | apptType  | empTitle   | newStatus                | effectiveDate |
| Tabula Update | Reserve  | Full Time | Chief      | ""Active (Active)""      | 10/15/2008    |
| Tabula Update | Reserve  | Full Time | Chief      | ""On Leave (Active)""    | 12/15/2008    |

Add employment actions for #Sterky at #OPD
| actionName      | empType  | apptType  | empTitle   | newStatus                | effectiveDate | comment          |
| Tabula Update   | Employee | Part Time | Instructor | ""On Leave (Active)""    | 10/15/2008    | Tabula Comment   |
| Separation      | Civiilan | Contract  | Lackey     | ""Separated (Inactive)"" | 12/15/2008    |                  |


""Add a comment and duty assignment"":
use: Person Employment FNH Management

Add employment comment ""Tabula comment"" for #empName at #orgName
Using employment of #empName at #orgName
Add duty assignment at #orgName starting #startDate with status ""Current""
| empName | orgName | startDate  |
| #Lina   | #HPD    | 8/15/2008  |
| #Sterky | #OPD    | 8/15/2008  | 
| #Mimi   | #OA     | 8/15/2012  | 

""Add temporary duty assignments"":
use: Person Employment FNH Management
Using employment of #Lina at #HPD
Add temporary duty assignment at #OPD from 1/1/2013 to 4/1/2013 with status ""Past""

Using employment of #Sterky at #OPD
Add temporary duty assignment at #HPD from 1/1/2013 to 4/1/2013 with status ""Past""
Add temporary duty assignment at #OA with status ""Past""


""Search people with many different duty assignment criteria"":
use: Person Search
use: Person Search Criteria Workflow

Browse to page
Click Switch to advanced mode
Add new employment criterion to expression
    With #Assignment duty assignments
    With duty locations #Location
    With duty assignment statuses #Status 
    With duty assignment dates before #Before
    With duty assignment dates after #After
    With duty assignment dates between #Between and #And
    With undated assignments #Undated
    With employment criteria effective on a specific date #EffectiveDate
    With employment effective date #EffectiveDate
    With employment types ""Tabula #EmpType""
    With organizations #Organization
    Click Done in employment criteria popover
Click Search
Verify that page navigated to search results
Search with criteria
Verify that there are #Num results
Verify that results are ""Bob""

""Search Assignment regular/temporary, location, and org"":
| Assignment         | Location | Organization | Num | Found          |
| regular            | #HPD     |              | 1   | #Lina          |
| regular, temporary | #HPD     |              | 2   | #Lina, #Sterky |
| regular, temporary |          | #HPD         | 1   | #Lina          |

""Assignment status, before, and after"":
| Location | Status | Before   | After    | Num | Found          |
| #HPD     |        |          |          | 2   | #Lina, #Sterky |
| #HPD     | Past   |          |          | 1   |        #Sterky |
| #HPD     |        | 9/1/2008 |          | 1   | #Lina          |
| #HPD     |        |          | 3/1/2013 | 2   | #Lina, #Sterky |

""Assignment date range, location and org"":
| Between   | And        | Location | Organization | Num | Found          |
| 9/01/2009 | 10/01/2010 | #HPD     |              | 1   | #Lina          |
| 8/18/2012 | 03/13/2013 | #HPD     |              | 2   | #Lina, #Sterky |
| 5/30/2012 | 05/30/2013 | #HPD     |              | 2   | #Lina, #Sterky |
| 6/01/2013 | 07/01/2013 | #HPD     |              | 1   | #Lina          |
| 6/01/2008 | 08/14/2008 | #HPD     |              | 0   |                |
| 8/08/2012 | 03/13/2013 |          | #HPD         | 1   | #Lina          |

""Assignment date range, location, and undated"":
| Between   | And        | Location  | Undated  | Num | Found                 |
| 1/01/2013 | 03/14/2013 | #OA       | Excluded | 1   | #Mimi                 |
| 1/01/2013 | 03/14/2013 | #OA       | Only     | 1   | #Sterky               |
| 8/01/2008 | 05/15/2013 | #OA, #HPD |          | 3   | #Lina, #Sterky, #Mimi |

""Assignment date range, location/org, and employment type"":
| Between   | And        | Location   | Organization | EmpType | Num | Found          |
| 1/01/2013 | 03/14/2013 |            |              | Reserve | 2   | #Lina, #Mimi   |
| 1/01/2013 | 03/14/2013 | #HPD, #OPD | #HPD, #OPD   |         | 2   | #Lina, #Sterky |

";
    }
}
