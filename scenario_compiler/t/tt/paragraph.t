use Test;
use Tabula::Grammar-Testing;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Paragraph" );
my $context = $actions.Context;
say "\n";

#if False
{   diag "Basic Paragraph output";
    $context.file-name = "SampleScenario.scn";

    my $advice-fixture = Fixture.new(class-name => "AdviceWorkflow", instance-name => "Advice");
    $advice-fixture.add-method("Some_Thing");
    $advice-fixture.add-method("This_is_a_step");

    # $context.RegisterLibrary($advice-fixture);
    $context.add-fixture($advice-fixture);


    my $check-fixture = Fixture.new(class-name => "CheckWorkflow");
    $check-fixture.add-method("Am_I_Done");
    # $context.RegisterLibrary($check-fixture);

    my $paragraph-source = q:to/EOP/;
    this is a step
    ? Am I done
    EOP
    my $parse = parser( "Paragraph", $paragraph-source );

    is $parse.made, q:to/EO_testo/, "paragraph output is a public method";
            public void paragraph_from_001_to_002()
            {
                Do(() =>     Advice.This_is_a_step(),     "SampleScenario.scn:1", @"Advice.This_is_a_step()" );
                Unfound(     "? Am I done",     "SampleScenario.scn:2" );
            }
    EO_testo

    #
    # is $context.problems.elems, 1, "unfound step is listed in \@problems";
    # is $context.problems[0], "SampleScenario.scn:2:  Did not find step to match '? Am I done' in libraries in scope.", "...with a sensible message";
    #
    #
    # $context.AddLibraryToScope($check-fixture);
    # $parse = parser( "Paragraph", $paragraph-source );
    #
    # is $parse.made, q:to/EO_testo/, "workflow methods found when library added to scope";
    #         public void paragraph_from_001_to_002()
    #         {
    #             Do(() =>     Advice.This_is_a_step(),     "SampleScenario.scn:1", @"Advice.This_is_a_step()" );
    #             Do(() =>     Check.Am_I_Done(),     "SampleScenario.scn:2", @"Check.Am_I_Done()" );
    #         }
    # EO_testo

}

done-testing;
