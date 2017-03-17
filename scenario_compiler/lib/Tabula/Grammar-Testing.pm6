use Test;
use Tabula::Tabula-Grammar;
use Tabula::Target-Echo;
use Tabula::Target-Testopia;
use Tabula::Execution-Context;

module Grammar-Testing {

    multi sub skip( $desc, &code ) is export { skip $desc }

    sub get-parser-emitting-Tabula( $rule ) is export {
        use Test;
        state $grammar = Tabula-Grammar.new;
        state $actions = Target-Echo.new;

        return sub ( $label, $input ) {
            my $out = $grammar.parse( $input, :rule($rule), :actions($actions) );
            ok $out, "[$label] $rule parses";
            return unless $out ~~ Match;

            if $out.made.defined {
                is norm($out.made), norm($input), "Actions rebuild input Tabula from parse of [$label] $rule";
            }

            return $out;
        }
    }

    sub norm( $input ) is export {
        $input.trim
            .subst( /^^\h+/, '', :g )        # leading ws on lines -> gone
            .subst( /\h\h+ || \t/, ' ', :g ) # runs of ws -> single space
    }

    sub get-parser-emitting-Testopia( $rule ) is export {
        use Test;
        my $grammar = Tabula-Grammar.new;
        my $actions = Target-Testopia.new;

        return (
            sub ( $input ) {
                my $out = $grammar.parse( $input, :rule($rule), :actions($actions) );
                if $out !~~ Match {
                    ok False, "The text [$input] did not parse as a $rule.";
                    return;
                }
                return $out;
            },
            $actions
        );
    }

    sub get-parser-expecting-parse-fail( $rule ) is export {
        use Test;
        my $grammar = Tabula-Grammar.new;
        my $actions = Target-Testopia.new;

        return (
            sub ( $input ) {
                try {
                    my $out = $grammar.parse( $input, :rule($rule), :actions($actions) );
                    ok False, 'The text [' ~ $input ~ "] parsed as a $rule, and it should not have.";
                    CATCH {
                        default {
                            ok True, 'The text [' ~ $input ~ "] is not a $rule.";
                            return .message;
                        }
                    }
                }
            },
            $actions
        );
    }

}
