use Test;
use Tabula::Grammar-Testing;

my (&parser, $context) = curry-parser-emitting-Testopia( "Paragraph" );
say "\n";

#if False
{   diag "Basic Paragraph output";

    my $lib = StepLibrary.new(class-name => "AdviceWorkflow", instance-name => "Advice");
    $lib.steps{'something'} = "Some_Thing";
    $lib.steps{'thisisastep'} = "This_is_a_step";
    $context.RegisterLibrary($lib);
    $context.AddLibraryToScope($lib);

    my $check-lib = StepLibrary.new(class-name => "CheckWorkflow");
    $check-lib.steps{'amidone'} = "Am_I_Done";
    $context.RegisterLibrary($check-lib);

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

    is $context.problems.elems, 1, "unfound step is a problem";
    is $context.problems[0], "SampleScenario.scn:2:  Did not find step to match '? Am I done' in libraries in scope.", "...with a sensible message";


    $context.AddLibraryToScope($check-lib);
    $parse = parser( "Paragraph", $paragraph-source );

    is $parse.made, q:to/EO_testo/, "workflow methods found when library added to scope";
            public void paragraph_from_001_to_002()
            {
                Do(() =>     Advice.This_is_a_step(),     "SampleScenario.scn:1", @"Advice.This_is_a_step()" );
                Do(() =>     Check.Am_I_Done(),     "SampleScenario.scn:2", @"Check.Am_I_Done()" );
            }
        EO_testo

}
