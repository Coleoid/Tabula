use Test;
use Tabula::Fixture-Class;

my $book = Fixture-Class.new(class-name => 'AdviceWorkflow');

my $sig = 'This_is_a_step()';
$book.add-method($sig);
my $page = $book.pages<thisisastep()>;
ok $page, "we stored the page at the expected key location";
is $page.definition, $sig,
    "stores a method with no arguments";

my $please = 'Please_dont__the__(String verb, string noun)';
$book.add-method($please);
is $book.pages<pleasedontthe(ss)>.definition, $please,
    "stores a method with string arguments";

is $book.key-from-step('This is a Step'), 'thisisastep()',
    "gets a key from step text with no args";


my $found = $book.find-step-method('This is a Step');
ok $found, "finds a method with no arguments";
is $found.definition, $page.definition, "finds the _right_ method";

my $unfound = $book.find-step-method('there is no such thing');
nok $unfound, 'properly returns no page for no such method';

done-testing;
