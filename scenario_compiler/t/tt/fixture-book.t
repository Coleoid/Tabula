use Test;
use Tabula::Fixture-Book;

my $book = Fixture-Book.new(class-name => 'AdviceWorkflow');

my $this = 'This_is_a_step()';
$book.add-method($this);
is $book.methods<thisisastep()>.definition, $this;

my $please = 'Please_dont__the__(String verb, string noun)';
$book.add-method($please);
is $book.methods<pleasedontthe(ss)>.definition, $please;

done-testing;
