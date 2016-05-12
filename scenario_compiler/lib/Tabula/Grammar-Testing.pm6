use Test;
use Tabula::Tabula-Grammar;
use Tabula::Generate-Tabula;
use Tabula::Generate-CSharp;
use Tabula::Build-Context;

module Grammar-Testing {

    our $CSharp-Context is export;

    multi sub skip( $desc, &code ) is export { skip $desc }

    sub curry-parser-for( $rule ) is export {
        use Test;
        state $grammar = Tabula-Grammar.new;
        state $actions = Generate-Tabula.new;

        return sub ( $label, $input ) {
            my $out = $grammar.parse( $input, :rule($rule), :actions($actions) );
            ok $out, "[$label] $rule parses";
            return unless $out ~~ Match;
            is $out.made.trim, $input.trim, "Actions rebuild input Tabula from parse of [$label] $rule";

            return $out;
        }
    }

    sub curry-csharp-parser-for( $rule ) is export {
        use Test;
        state $grammar = Tabula-Grammar.new;
        state $actions = Generate-CSharp.new;
        once { $CSharp-Context = $actions.Context; }

        return sub ( $label, $input ) {
            my $out = $grammar.parse( $input, :rule($rule), :actions($actions) );
            ok $out, "[$label] $rule parses";
            return unless $out ~~ Match;

            return $out;
        }
    }
}
