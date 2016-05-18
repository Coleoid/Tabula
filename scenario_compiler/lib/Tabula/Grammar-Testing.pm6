use Test;
use Tabula::Tabula-Grammar;
use Tabula::Target-Tabula;
use Tabula::Target-Testopia;
use Tabula::Build-Context;

module Grammar-Testing {

    multi sub skip( $desc, &code ) is export { skip $desc }

    sub curry-parser-emitting-Tabula( $rule ) is export {
        use Test;
        state $grammar = Tabula-Grammar.new;
        state $actions = Target-Tabula.new;

        return sub ( $label, $input ) {
            my $out = $grammar.parse( $input, :rule($rule), :actions($actions) );
            ok $out, "[$label] $rule parses";
            return unless $out ~~ Match;
            is $out.made.trim, $input.trim, "Actions rebuild input Tabula from parse of [$label] $rule";

            return $out;
        }
    }

    sub curry-parser-emitting-Testopia( $rule ) is export {
        use Test;
        state $grammar = Tabula-Grammar.new;
        state $actions = Target-Testopia.new;

        return (
            sub ( $label, $input ) {
                my $out = $grammar.parse( $input, :rule($rule), :actions($actions) );
                if $out !~~ Match {
                    ok $out, "[$label] $rule parses";
                    return;
                }
                return $out;
            },
            $actions.Context
        );
    }
}
