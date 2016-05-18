
class StepLibrary {
    has Str $.class-name;
    has Str $.instance-name;
    has Str $.flat-name;

    has %.steps;

    submethod BUILD(:$!class-name is required, :$!instance-name = "") {

        $!flat-name = $!class-name.lc.subst('workflow', '');
    }
}

class Scope {
    has $.name;
    has StepLibrary @.libraries;
    has $.parent;

    submethod BUILD(:$!name, :$!parent) {
    }

    method AddLibrary($library) {
        @!libraries.push($library) unless (@!libraries.first: * == $library);
    }
}

class Build-Context {
    has $.file-name;
    has Scope @.scopes;
    has Scope $.current-scope;
    has %.registered-libraries;
    has @.Problems;

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
        %!registered-libraries{$lib.flat-name} = $lib;
    }

    multi method AddLibraryToScope(Str $phrase) {
        my $flatName = $phrase.lc.subst(' ', '').subst('workflow', '');
        my $library = %!registered-libraries{$flatName} // 0;
        if  $library {
            self.AddLibraryToScope($library);
        }
        else {
            @!Problems.push( "Did not find library <<$flatName>> to add to scope." );
        }
    }

    multi method AddLibraryToScope(StepLibrary $library) {
        $!current-scope.AddLibrary($library);
    }

    method GetFixtureCall($match) {
        my ($found, $methodName) = self!getMethodName($match);

        if $found {
            my $argsText = getArgsText($match);
            return True, $methodName ~ "($argsText)";
        }
        else {
            return False, ~$match;
            #TODO: Text::Levenshtein::Damerau
        }
    }

    method !getMethodName($match) {
        my @words = gather {
            for $match<Symbol> {
                when .<Word> {take .<Word>.lc}
            }
        }
        my $flatName = join "", @words;
        return self!findStepMethod($flatName);
    }

    method !findStepMethod($flatName) {

        for $!current-scope.libraries {
            if .steps{$flatName}:exists {
                my $foundMethod = .steps{$flatName};
                return True, .class-name ~ "." ~ $foundMethod;
            }
        }

        return False, "No such method in scope";
    }

    sub getArgsText($match) {
        my @args = gather {
            for $match<Symbol> {
                when .<Term> {
                    #TODO: handle target param types other than string
                    when .<Term><String> {take .<Term>.made};
                    when .<Term><Number> {take '"' ~ .<Term>.made ~ '"'};
                    when .<Term><Date>   {take '"' ~ .<Term>.made ~ '"'};
                    when .<Term><ID>     {take 'val["' ~ .<Term><ID><Word> ~ '"]'};
                    default {fail "Unknown Term type"};
                }
            }
        }
        return join ', ', @args;
    }


}
