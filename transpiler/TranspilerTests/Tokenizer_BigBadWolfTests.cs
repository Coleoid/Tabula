using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tabula
{
    [TestFixture]
    public class Tokenizer_BigBadWolfTests : TokenizerTestBase
    {
        [Test]
        public void Scenario_header()
        {
            var tokens = tokenizer.Tokenize(bigby);

            Assert_TokenSequenceMatches(tokens, 0,
                TokenType.NewLine,
                TokenType.Tag,
                TokenType.Tag,
                TokenType.Tag,
                TokenType.NewLine,
                TokenType.ScenarioLabel
            );

            Assert.That(tokens[5].Text, Does.StartWith("Advanced person"));
        }

        [Test]
        public void para_1()
        {
            var tokens = tokenizer.Tokenize(bigby);

            Assert_TokenSequenceMatches(tokens, 8,
                TokenType.SectionLabel,
                TokenType.NewLine,
                TokenType.CommandUse,
                TokenType.NewLine,
                TokenType.Word,
                TokenType.Word,
                TokenType.Word,
                TokenType.NewLine
            );

            Assert.That(tokens[8].Text, Does.StartWith("Enable duty locations"));
            Assert.That(tokens[10].Text, Does.Match("Global Setting Management"));
            Assert.That(tokens[12].Text, Does.Match("Enable"));
            Assert.That(tokens[13].Text, Does.Match("Duty"));
            Assert.That(tokens[14].Text, Does.Match("Locations"));
        }

        [Test]
        public void para_2()
        {
            var tokens = tokenizer.Tokenize(bigby);

            Assert_TokenSequenceMatches(tokens, 17,
                TokenType.SectionLabel,
                TokenType.NewLine,
                TokenType.CommandAlias,
                TokenType.NewLine
            );

            Assert.That(tokens[17].Text, Does.Match("What we'll call our people in this scenario"));
            Assert.That(tokens[19].Parts[0], Does.Match("#handle"));
            Assert.That(tokens[19].Parts[1], Does.Match("#FullNameLF"));
        }

        [Test]
        public void table_1()
        {
            var tokens = tokenizer.Tokenize(bigby);

            Assert_TokenSequenceMatches(tokens, 21,
                TokenType.TableCellSeparator,
                TokenType.Word,
                TokenType.TableCellSeparator,
                TokenType.Word,
                TokenType.TableCellSeparator,
                TokenType.NewLine,
                TokenType.TableCellSeparator,
                TokenType.Word,
                TokenType.TableCellSeparator,
                TokenType.String,
                TokenType.TableCellSeparator,
                TokenType.NewLine
            );
        }

        private string bigby =
        @"
[Person Search, Duty Assignments, AC-16629]
Scenario: ""Advanced person search with duty assignments""

""Enable duty locations"":
>use: Global Setting Management
Enable Duty Locations

""What we'll call our people in this scenario"":
>alias: #handle => #FullNameLF
| handle | FullNameLF               |
| Lina   | ""Frixell, Rorolina""    |
| Gio    | ""Arland, Giovanni""     |
| Sterky | ""Cranach, Sterkenberg"" |
| Mimi   | ""Schwarzlang, Mimi""    |

""What we'll call the organizations they work for"":
>alias: #TLA => #OrganizationName
| TLA | OrganizationName             |
| HPD | Hometown Police Department   |
| OPD | Otherville Police Department |
| OA  | Otherville Academy           |


""Create our people"":
>use: User Creation
Create Person named #name
| name    |
| #Lina   |
| #Gio    |
| #Sterky |
| #Mimi   |

""Create our organizations"":
>use: Organization FNH Management
Create Organization named #orgName of type ""organization"" under base parent group
| orgName  |
| #HPD     |
| #OPD     |
| #OA      |

""Create list items"":
>use: List Management

Create #itemType list item ""Testopia #item"" with description ""Testopia #item description"" with usage ""Available for new records""
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
>use: Employment Action Edit

Browse to Add Person Employment for #empName
Add primary employment at #orgName with title ""Testopia #empTitle"" employment type ""Testopia #empType"" and appointment type ""Testopia #apptType"" starting on #startDate
| empName  | orgName  | empTitle   | empType  | apptType  | startDate  |
| #Lina    | #HPD     | Lackey     | Civilian | Contract  | 8/15/2008  |
| #Gio     | #HPD     | Lackey     | Civilian | Contract  | 8/15/2008  |
| #Sterky  | #OPD     | Lackey     | Civilian | Contract  | 8/15/2008  |
| #Mimi    | #OA      | Instructor | Reserve  | Part Time | 8/15/2012  |

>alias: ""Add employment actions for #employeeName at #orgName"" => ...
    >use: Employment Action Edit

    Browse to Add Employment Action for #employeeName at #orgName
    Set action to #actionName
    Set employment type to ""Testopia #empType""
    Set appointment type to ""Testopia #apptType""
    Set title to ""Testopia #empTitle""
    Set status to #newStatus
    Set effective date to #effectiveDate
    Set comments to #comment
    Click Save
.

""Add employment actions"":
Add employment actions for #Lina at #HPD
| actionName      | empType  | apptType  | empTitle   | newStatus              | effectiveDate |
| Testopia Update | Employee | Part Time | Instructor | ""On Leave (Active)""    | 10/15/2008    |
| Separation      | Reserve  | Full Time | Chief      | ""Separated (Inactive)"" | 12/15/2008    |

Add employment actions for #Gio at #HPD
| actionName      | empType  | apptType  | empTitle   | newStatus              | effectiveDate |
| Testopia Update | Reserve  | Full Time | Chief      | ""Active (Active)""      | 10/15/2008    |
| Testopia Update | Reserve  | Full Time | Chief      | ""On Leave (Active)""    | 12/15/2008    |

Add employment actions for #Sterky at #OPD
| actionName      | empType  | apptType  | empTitle   | newStatus              | effectiveDate | comment          |
| Testopia Update | Employee | Part Time | Instructor | ""On Leave (Active)""    | 10/15/2008    | Testopia Comment |
| Separation      | Civiilan | Contract  | Lackey     | ""Separated (Inactive)"" | 12/15/2008    |                  |


""Add a comment and duty assignment"":
>use: Person Employment FNH Management

Add employment comment ""Testopia comment"" for #empName at #orgName
Using employment of #empName at #orgName
Add duty assignment at #orgName starting #startDate with status ""Current""
| empName | orgName | startDate  |
| #Lina   | #HPD    | 8/15/2008  |
| #Sterky | #OPD    | 8/15/2008  | 
| #Mimi   | #OA     | 8/15/2012  | 

Using employment of #Lina at #HPD
Add temporary duty assignment at #OPD from ""1/1/2013"" to ""4/1/2013"" with status ""Past""

Using employment of #Sterky at #OPD
Add temporary duty assignment at #HPD from ""1/1/2013"" to ""4/1/2013"" with status ""Past""
Add temporary duty assignment at #OA with status ""Past""


""Search people with many different duty assignment criteria"":
>use: Person Search
>use: Person Search Criteria Workflow

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
    With employment types ""Testopia #EmpType""
    With organizations #Organization
    Click Done in employment criteria popover
Click Search
Verify that page navigated to search results
Search with criteria
Verify that there are #Num results
Verify that results are #Found

=== ""Search Assignment regular/temporary, location, and org"" ===
| Assignment         | Location | Organization | Num | Found          |
| regular            | #HPD     |              | 1   | #Lina          |
| regular, temporary | #HPD     |              | 2   | #Lina, #Sterky |
| regular, temporary |          | #HPD         | 1   | #Lina          |

=== ""Assignment status, before, and after"" ===
| Location | Status | Before   | After    | Num | Found          |
| #HPD     |        |          |          | 2   | #Lina, #Sterky |
| #HPD     | Past   |          |          | 1   |        #Sterky |
| #HPD     |        | 9/1/2008 |          | 1   | #Lina          |
| #HPD     |        |          | 3/1/2013 | 2   | #Lina, #Sterky |

=== ""Assignment date range, location and org"" ===
| Between   | And        | Location | Organization | Num | Found          |
| 9/01/2009 | 10/01/2010 | #HPD     |              | 1   | #Lina          |
| 8/18/2012 | 03/13/2013 | #HPD     |              | 2   | #Lina, #Sterky |
| 5/30/2012 | 05/30/2013 | #HPD     |              | 2   | #Lina, #Sterky |
| 6/01/2013 | 07/01/2013 | #HPD     |              | 1   | #Lina          |
| 6/01/2008 | 08/14/2008 | #HPD     |              | 0   |                |
| 8/08/2012 | 03/13/2013 |          | #HPD         | 1   | #Lina          |

=== ""Assignment date range, location, and undated"" ===
| Between   | And        | Location  | Undated  | Num | Found                 |
| 1/01/2013 | 03/14/2013 | #OA       | Excluded | 1   | #Mimi                 |
| 1/01/2013 | 03/14/2013 | #OA       | Only     | 1   | #Sterky               |
| 8/01/2008 | 05/15/2013 | #OA, #HPD |          | 3   | #Lina, #Sterky, #Mimi |

=== ""Assignment date range, location/org, and employment type"" ===
| Between   | And        | Location   | Organization | EmpType | Num | Found          |
| 1/01/2013 | 03/14/2013 |            |              | Reserve | 2   | #Lina, #Mimi   |
| 1/01/2013 | 03/14/2013 | #HPD, #OPD | #HPD, #OPD   |         | 2   | #Lina, #Sterky |

";
    }


}
