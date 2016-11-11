use Test;
use Tabula::Fixture;

my $fixture = Fixture.new(class-name => 'AdviceWorkflow');
$fixture.add-method('This_is_a_step()');
$fixture.add-method('Please_dont__the__(Str verb, Str noun)');

done-testing;
