use Test;
use Tabula::Tabula-Grammar;
use Tabula::Target-Echo;
use Tabula::Target-Testopia;
use Tabula::Execution-Context;

module Grammar-Testing {

    multi sub skip( $desc, &code ) is export { skip $desc }

    sub curry-parser-emitting-Tabula( $rule ) is export {
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

    sub curry-parser-emitting-Testopia( $rule ) is export {
        use Test;
        my $grammar = Tabula-Grammar.new;
        my $actions = Target-Testopia.new;

        return (
            sub ( $label, $input ) {
                my $out = $grammar.parse( $input, :rule($rule), :actions($actions) );
                if $out !~~ Match {
                    ok $out, "[$label] $rule expected to parse";
                    return;
                }
                return $out;
            },
            $actions
        );
    }

    sub curry-parser-expecting-parse-fail( $rule ) is export {
        use Test;
        my $grammar = Tabula-Grammar.new;
        my $actions = Target-Testopia.new;

        return (
            sub ( $command ) {
                try {
                    my $out = $grammar.parse( $command, :rule($rule), :actions($actions) );
                    ok False, 'The text [' ~ $command ~ '] parsed as $rule, and it should not have.';
                    CATCH {
                        default { return $out; }
                    }
                }
            },
            $actions
        );
    }


    sub curry-expect-parse-error(&parser) is export {
        return
            sub ($command, $message, $description) {
                try {
                    my $parse = parser( "expect parse failure of [$command]", $command );
                    ok False, 'The text [' ~ $command ~ '] parsed, when it should not have.';
                    CATCH {
                        default { is .Str, $message, $description; }
                    }
                }
            };
    }
}
