use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;


my (&parser, $actions) = curry-parser-emitting-Testopia("Scenario");
my $context = $actions.Context;
my $binder = $actions.Binder;
$context.file-name = "ScenarioFilename.scn";

#if False
{   diag "We emit a C# class corresponding to our scenario";

    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    EOS

    my $expected-class-output = q:to/EOC/;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Acadis.SystemUtilities;
    using Acadis.Constants.Accounting;
    using Acadis.Constants.Admin;
    using Acadis.Domain.Model;

    namespace Tabula
    {
        public class ScenarioFilename_generated
            : GeneratedScenarioBase, IGeneratedScenario
        {
            public ScenarioFilename_generated()
                : base()
            {
                ScenarioLabel = @"ScenarioFilename.scn:  ""This and That""";
            }

            public void ExecuteScenario()
            {
            }
        }
    }
    EOC

    my $parse = parser( "empty scenario", $scenario );
    my $output-class = $parse.made;

    is $output-class, $expected-class-output, "empty scenario creates compilable empty class";
}

#if False
{   diag "The class has tables, paragraphs, and and an execution plan matching the scenario";

    my $class = Fixture-Class.new(class-name => 'ActionWorkflow', namespace => 'ScenarioContext');
    $class.add-method("Action_one_with__argument(string flavor)");
    $class.add-method("Action_two_with__and__(string color, string irrelevant)");

    $binder.add-class($class);

    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    >use: action
    Action one with #flavor argument
    Action two with #color and #attitude
    [ flavor | color | attitude ]
    | spicy  | red   | sassy    |
    | zesty  | green | lively   |

    EOS

    my $expected-class-output = q:to/EOC/;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Acadis.SystemUtilities;
    using Acadis.Constants.Accounting;
    using Acadis.Constants.Admin;
    using Acadis.Domain.Model;

    namespace Tabula
    {
        public class ScenarioFilename_generated
            : GeneratedScenarioBase, IGeneratedScenario
        {
            public ScenarioContext.ActionWorkflow Action;

            public ScenarioFilename_generated()
                : base()
            {
                ScenarioLabel = @"ScenarioFilename.scn:  ""This and That""";
                Action = new ScenarioContext.ActionWorkflow();
            }

            public void ExecuteScenario()
            {
                Run_para_over_table( paragraph_from_003_to_005, table_from_006_to_008 );
            }

            public void paragraph_from_003_to_005()
            {
                Do(() =>     Action.Action_one_with__argument(var["flavor"]),     "ScenarioFilename.scn:4", @"Action.Action_one_with__argument(var[""flavor""])" );
                Do(() =>     Action.Action_two_with__and__(var["color"], var["attitude"]),     "ScenarioFilename.scn:5", @"Action.Action_two_with__and__(var[""color""], var[""attitude""])" );
            }

            public Table table_from_006_to_008()
            {
                return new Table {
                    Header = new List<string>     { "flavor", "color", "attitude" },
                    Data = new List<List<string>> {
                        new List<string>          { "spicy", "red", "sassy" },
                        new List<string>          { "zesty", "green", "lively" },
                    }
                };
            }
        }
    }
    EOC

    my $parse = parser( "empty scenario", $scenario );
    my $output-class = $parse.made;

    is $output-class, $expected-class-output, "simple scenario with paragraph and table works";
}


done-testing;
