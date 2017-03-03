use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Symbol" );
my $context = $actions.Context;

my $fixture = Fixture-Class.new(class-name => 'AdviceWorkflow', namespace => 'foo');
$fixture.add-method('This_is_a_step()');
$fixture.add-method('Please_dont__the__(String verb, String noun)');

$context.file-name = "SampleScenario.scn";
$context.add-fixture($fixture);

#if False
{   diag "Finding calls matching step text";

    my $parse = parser( "Symbol: Word", 'this' );
    ok $parse.defined, "recognizes bare word as Word";
    is $parse.made, 'this', 'symbol:Word should not be quoted';

    $parse = parser( "Symbol: String", '"why not?"' );
    ok $parse.defined, "recognizes quoted string as String";
    is $parse.made, '"why not?"', 'symbol:String should be quoted';

    $parse = parser( "Symbol: String", '"why, or why not?"' );
    ok $parse.defined, "recognizes quoted string with comma as single String";
    is $parse.made, '"why, or why not?"', 'symbol:String includes comma';

    $parse = parser( "Symbol: Number", '514.02' );
    ok $parse.defined, "recognizes number with decimal as Number";
    is $parse.made, '"514.02"', 'symbol:Number is quoted';
}

done-testing;
