use v6;
use Tabula::Scope;
use Tabula::Match-Helper;

class Execution-Context
    does Match-Helper {
    has str $.file-name is rw;

    submethod BUILD {
        self.open-scope("Outer scenario scope");
    }

    has Scope $.current-scope;
    method open-scope($name = 'unnamed block') {
        $!current-scope = Scope.new(
            name => $name,
            parent => $!current-scope
        );
    }
    method close-scope() {
        $!current-scope // fail "trying to end a scope where there is none.";
        $!current-scope = $!current-scope.parent;
    }

    #  Delegate more directly to current-scope?
    method add-fixture($fixture) {
        $!current-scope.add-fixture($fixture);
    }
    method add-alias($alias, $value) {
        $!current-scope.add-alias($alias, $value);
    }
    method add-label($label, $value) {
        $!current-scope.add-label($label, $value);
    }

    #  Given an action match, finds step method it fits, and
    # resolve any labels in its arguments.
    #  Begins in current scope, stops when it finds a step method.
    method resolve-step($match) {
        $!current-scope.resolve-step($match);
    }
    #  ?:  Should we scan each fixture only once during each resolution?

    #  Determine how an argument should be shown in source.
    #  Type preparation, nested label replacements, ...
    method resolve-arg($arg) { ... }

    #  Fetch the value of a label
    method resolve-label($label) { ... }
    #  Label values which include other labels resolve backward only,
    # with the intent (and hope) to prevent any loops.

    ##################################################################
    #

}

#   ===================================
class Build-Context-old {
    has Scope @.scopes;
    has Scope $.current-scope handles <AddLibraryToScope>;

    has Str $.file-name is rw;
    has %.registry;
    has %.yet-initialized;
    has @.problems;
    has $.lib-declarations;

    # submethod BUILD {
    #     self.BeginScope("Outer scenario scope");
    # }
    #
    # method BeginScope($name = 'unnamed block') {
    #     $!current-scope = Scope.new(
    #         name => $name,
    #         parent => $!current-scope,
    #         problems => @!problems,
    #     );
    #     @!scopes.push($!current-scope);
    # }
    #
    # method EndScope() {
    #     $!current-scope // fail "trying to end a scope where there is none.";
    #
    #     @!scopes.pop();
    #     $!current-scope = $!current-scope.parent;
    # }

    method RegisterLibrary($lib) {
        if not %!registry{$lib.flat-name} {
            %!registry{$lib.flat-name} = $lib;
            $!lib-declarations ~= "        public " ~ $lib.class-name ~ " "
                ~ $lib.instance-name ~ " = new " ~ $lib.class-name ~ "();\n";
        }
    }

    method GetFixtureCall($match, $location) {
        my $result = self!get-method-name($match);

        if $result {
            my $argsText = getArgsText($match);
            return $result ~ "($argsText)";
        }
        else {
            #TODO:  Text::Levenshtein::Damerau for approximate matching
            my $message = $location ~ ':  ' ~ $result.exception.message
                ~ " '" ~ ~$match ~ "' in libraries in scope.";
            @!problems.push( $message );
            fail $message;
        }
    }

    method !get-method-name($match) {
        my $arg-count = 0;
        my @words = gather {
            for $match<Phrase><Symbol> {
                when .<Word> {take .<Word>.lc.subst("'", '', :g)}
                when .<Term> {$arg-count++}
            }
        }
        my $flatName = join "", @words;
        return self!find-step-method($flatName, $arg-count);
    }

    method !find-step-method($flatName, $arg-count) {
        $!current-scope.find-step-method($flatName, $arg-count);
    }

    #TODO:  Evaluate terms as number and date types
    sub get-Term-string($term) {
        given $term {
            when .<String> {return .made};
            when .<Number> {return '"' ~ .made ~ '"'};
            when .<Date>   {return '"' ~ .made ~ '"'};
            when .<ID>     {return 'alias["' ~ .<ID><Word> ~ '"]'};
            default        {fail "Unknown Term type"};
        }
    }

    sub getArgsText($match) {
        my @args = $match<Phrase><Symbol>
            .grep({.<Term>})
            .map({get-Term-string(.<Term>)});

        return join ', ', @args;
    }
}
