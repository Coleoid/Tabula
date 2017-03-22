use Test;
use Tabula::Fixture-Class;

my $class = Fixture-Class.new(class-name => 'AdviceWorkflow', :namespace<foo.bar>);

my $sig = 'This_is_a_step()';
$class.add-method($sig);
my $method = $class.methods<thisisastep()>;
ok $method, "we stored the method at the expected key location";
is $method.definition, $sig,
    "stores a method with no arguments";

my $please = 'Please_dont__the__(String verb, string noun)';
$class.add-method($please);
is $class.methods<pleasedontthe()>.definition, $please,
    "stores a method with string arguments";

my $found = $class.find-step-method-from-text('This is a Step');
ok $found, "finds a method with no arguments";
is $found.definition, $method.definition, "finds the _right_ method";

my $unfound = $class.find-step-method-from-text('there is no such thing');
nok $unfound, 'properly returns no method for no such method';

done-testing;
