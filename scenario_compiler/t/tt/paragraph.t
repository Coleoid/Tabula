use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Paragraph" );
my $context = $actions.Context;
$context.file-name = "SampleScenario.scn";
say "\n";

#if False
{   diag "Paragraph step resolution depending on in-scope fixtures";

    my $advice-fixture = Fixture-Class.new(class-name => "AdviceWorkflow", instance-name => "Advice", namespace => 'foo');
    $advice-fixture.add-method("Some_Thing()");
    $advice-fixture.add-method("This_is_a_step()");

    my $another-fixture = Fixture-Class.new(class-name => "AnotherWorkflow", instance-name => "Another", namespace => 'foo');
    $another-fixture.add-method("Step_from_AnotherFixture()");

    my $check-fixture = Fixture-Class.new(class-name => "CheckWorkflow", namespace => 'foo');
    $check-fixture.add-method("Am_I_Done()");

    my $overriding-fixture = Fixture-Class.new(class-name => "OverridingWorkflow", instance-name => "Overriding", namespace => 'foo');
    $overriding-fixture.add-method("THIS__IS__A__STEP()");
    $overriding-fixture.add-method("STEP_FROM_ANOTHERFIXTURE()");

    $context.add-fixture($advice-fixture);

    my $paragraph-source = q:to/EOP/;
    this is a step
    step from another fixture
    ? Am I done
    EOP

    my $found-one-method = q:to/EO_testo/;
            public void paragraph_from_001_to_003()
            {
                Do(() =>     Advice.This_is_a_step(),     "SampleScenario.scn:1", @"Advice.This_is_a_step()" );
                Unfound(     "step from another fixture",     "SampleScenario.scn:2" );
                Unfound(     "? Am I done",     "SampleScenario.scn:3" );
            }
    EO_testo

    my $found-two-methods = q:to/EO_testo/;
            public void paragraph_from_001_to_003()
            {
                Do(() =>     Advice.This_is_a_step(),     "SampleScenario.scn:1", @"Advice.This_is_a_step()" );
                Do(() =>     Another.Step_from_AnotherFixture(),     "SampleScenario.scn:2", @"Another.Step_from_AnotherFixture()" );
                Unfound(     "? Am I done",     "SampleScenario.scn:3" );
            }
    EO_testo

    my $found-two-different-methods = q:to/EO_testo/;
            public void paragraph_from_001_to_003()
            {
                Do(() =>     Overriding.THIS__IS__A__STEP(),     "SampleScenario.scn:1", @"Overriding.THIS__IS__A__STEP()" );
                Do(() =>     Overriding.STEP_FROM_ANOTHERFIXTURE(),     "SampleScenario.scn:2", @"Overriding.STEP_FROM_ANOTHERFIXTURE()" );
                Unfound(     "? Am I done",     "SampleScenario.scn:3" );
            }
    EO_testo

    my $found-three-methods = q:to/EO_testo/;
            public void paragraph_from_001_to_003()
            {
                Do(() =>     Advice.This_is_a_step(),     "SampleScenario.scn:1", @"Advice.This_is_a_step()" );
                Do(() =>     Another.Step_from_AnotherFixture(),     "SampleScenario.scn:2", @"Another.Step_from_AnotherFixture()" );
                Do(() =>     Check.Am_I_Done(),     "SampleScenario.scn:3", @"Check.Am_I_Done()" );
            }
    EO_testo

    my $parse = parser( "Paragraph", $paragraph-source );
    is $parse.made, $found-one-method, "paragraph output is a public method";

    $context.add-fixture($another-fixture);
    $parse = parser( "Paragraph", $paragraph-source );
    is $parse.made, $found-two-methods, "a second fixure in the same scope finds a second step";

    $context.open-scope('should not hide our Advice fixture');
    $parse = parser( "Paragraph", $paragraph-source );
    is $parse.made, $found-two-methods, "new scope does not hide outer scope fixtures";

    $context.add-fixture($check-fixture);
    $parse = parser( "Paragraph", $paragraph-source );
    is $parse.made, $found-three-methods, "the third step is now found in the child scope";

    $context.close-scope();
    $parse = parser( "Paragraph", $paragraph-source );
    is $parse.made, $found-two-methods, "third fixture went out of scope so we don't find third step";

    $context.open-scope('can override parent scope steps with a child scope');
    $context.add-fixture($overriding-fixture);
    $parse = parser( "Paragraph", $paragraph-source );
    is $parse.made, $found-two-different-methods, "can override parent scope steps with a child scope";

    $context.close-scope();
    $parse = parser( "Paragraph", $paragraph-source );
    is $parse.made, $found-two-methods, "overriding fixture went out of scope so we find the original methods";

    $context.add-fixture($overriding-fixture);
    $parse = parser( "Paragraph", $paragraph-source );
    is $parse.made, $found-two-different-methods, "last fixture added wins signature collisions";
}

# if False
{   diag "Paragraph labels";

    my $paragraph-source = q:to/EOP/;
    "Find fun things":
    step this way
    do a thing
    ? Am I done
    EOP

    my $labeled-para = q:to/EO_testo/;
            public void paragraph_from_001_to_004()
            {
                Label(  "Find fun things" );
                Unfound(     "step this way",     "SampleScenario.scn:2" );
                Unfound(     "do a thing",     "SampleScenario.scn:3" );
                Unfound(     "? Am I done",     "SampleScenario.scn:4" );
            }
    EO_testo

    my $parse = parser( "Paragraph", $paragraph-source );
    is $parse.made, $labeled-para, "label represented in generated method";

}

done-testing;
