
class StepLibrary {
    has Str $.class-name;
    has Str $.instance-name;
    has Str $.flat-name;

    has %.steps;
    has %.args;

    submethod BUILD(:$!class-name is required, :$!instance-name = "") {
        my $short-name = $!class-name.subst('Workflow', '');
        $!flat-name = $short-name.lc;
        if $!instance-name eq "" {
            $!instance-name = $short-name;
        }
    }
}

class Scope {
    has $.name;
    has StepLibrary @.libraries;
    has $.parent;

    submethod BUILD(:$!name, :$!parent) {
    }

    method AddLibrary(StepLibrary $library) {
        for @!libraries {
            if $_ ~~ $library {
                fail "Tried to add duplicate library <<$($library.flat-name)>>.";
            }
        }
        @!libraries.push($library);
        return True;
    }
}

#   ===================================
class Build-Context {
    has $.file-name;
    has Scope @.scopes;
    has Scope $.current-scope;
    has %.registry;
    has %.yet-initialized;
    has @.problems;
    has $.lib-declarations;

    submethod BUILD {
        self.BeginScope("Outer scenario scope");
    }

    method BeginScope($name) {
        $!current-scope = Scope.new(
            name => ($name // 'unnamed block'),
            parent => $!current-scope
        );
        @!scopes.push($!current-scope);
    }

    method EndScope() {
        $!current-scope // fail "trying to end a scope where there is none.";

        @!scopes.pop();
        $!current-scope = $!current-scope.parent;
    }

    method RegisterLibrary($lib) {
        if not %!registry{$lib.flat-name} {
            %!registry{$lib.flat-name} = $lib;
            $!lib-declarations ~= "        public " ~ $lib.class-name ~ " "
                ~ $lib.instance-name ~ " = new " ~ $lib.class-name ~ "();\n";
        }
    }

    multi method AddLibraryToScope(Str $phrase) {
        my $flatName = $phrase.lc.subst(' ', '', :g).subst('workflow', '');
        my $library = %!registry{$flatName} // 0;
        if  $library {
            self.AddLibraryToScope($library);
        }
        else {
            @!problems.push( "Did not find library <<$flatName>> to add to scope." );
        }
    }

    multi method AddLibraryToScope(StepLibrary $library) {
        my $result = $!current-scope.AddLibrary($library);
        if not $result {
            @!problems.push( $result.exception.message );
        }
    }

    method GetFixtureCall($match, $location) {
        my $result = self!getMethodName($match);

        if $result {
            my $argsText = getArgsText($match);
            return $result ~ "($argsText)";
        }
        else {
            #TODO:  Text::Levenshtein::Damerau to find close misses
            my $message = $location ~ ':  ' ~ $result.exception.message ~ " '" ~ ~$match ~ "' in libraries in scope.";
            @!problems.push( $message );
            fail $message;
        }
    }

    method !getMethodName($match) {
        my $arg-count = 0;
        my @words = gather {
            for $match<Symbol> {
                when .<Word> {take .<Word>.lc.subst("'", '', :g)}
                when .<Term> {$arg-count++}
            }
        }
        my $flatName = join "", @words;
        return self!findStepMethod($flatName, $arg-count);
    }

    method !findStepMethod($flatName, $arg-count) {
        #TODO:  Loop upward through parent scopes seeking step in libraries
        for $!current-scope.libraries {
            if .steps{$flatName}:exists && .steps{$flatName}[1] == $arg-count {
                my $foundMethod = .steps{$flatName}[0];
                return .instance-name ~ "." ~ $foundMethod;
            }
        }

        fail "Did not find step to match";
    }

    sub get-Term-string($term) {
        given $term {
            when .<String> {return .made};
            when .<Number> {return '"' ~ .made ~ '"'};
            when .<Date>   {return '"' ~ .made ~ '"'};
            when .<ID>     {return 'alias["' ~ .<ID><Word> ~ '"]'};
            default {fail "Unknown Term type"};
        }
    }

    sub getArgsText($match) {
        my @args = gather {

            $match<Symbol>
                .grep({.<Term>})
                .map({take get-Term-string(.<Term>)});

#            for $match<Symbol> {
#                when .<Term> {
#                    take get-Term-string( .<Term> )
#                }
#            }
        }
        return join ', ', @args;
    }


}
