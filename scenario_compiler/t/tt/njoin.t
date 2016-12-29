use Test;
use Tabula::Code-Scribe :njoin;

plan 10;

my $out = njoin( () );
is $out, '', "empty input, empty output";

$out = njoin( (), delim => '===' );
is $out, '', "empty input with delim arg, empty output";

$out = njoin( 3, () );
is $out, '', "multiplied delim arg with empty input, empty output";

$out = njoin( '', '', '' );
is $out, '', "list of empty entries, empty output";

$out = njoin( 'cheese' );
is $out, 'cheese', "the cheese stands alone";

$out = njoin( <one two> );
is $out, "one\ntwo", "two entries joined with default one newline";

$out = njoin( 2, <one two>, delim => '*-*' );
is $out, "one*-**-*two", "two entries joined with doubled custom delim";

$out = njoin( <one two>, delim => '~' );
is $out, "one~two", "'delim' arg works";

$out = njoin( ["foo  ", "bar\n  \n\n", "  baz qux"] );
is $out, "foo\nbar\n  baz qux", "trailing whitespace trimmed";

$out = njoin( ['', "foo", ' ', '', "\n\n ", "bar", "baz", ' '] );
is $out, "foo\nbar\nbaz", "emptyish entries removed";
