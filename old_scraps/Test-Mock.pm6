use Test;
use Test::Mock;

plan 2;

class Foo {
    my $frustration = 0;
    method lol() { 'rofl' }
    method wtf() { $frustration++; 'oh ffs' }
    method fml() { throw 'foad' if $frustration > 10_000 }
}

my $x = mocked(Foo);

set-mock($x,
    *.when('lol')
        .then({ 'rofl' }, times => 2)
        .then({ 'roflcopter' }),
);


$x.lol();
$x.lol();

check-mock($x,
    *.called('lol', times => 2),
    *.never-called('wtf'),
);
