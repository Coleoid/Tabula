
class StepLibrary {
    has Str $.name;
    has $.flatName;

    has %.steps;

    submethod BUILD(:$!name) {
        $!flatName = $!name.lc.subst(' ', '');
    }
}

class Scope {
    has $.name;
    has StepLibrary @.libraries;
    has $.parent;

    submethod BUILD(:$!name, :$!parent) {
    }

    method AddLibrary($name) {
        @!libraries.push($name);
    }
}

class Build-Context {
    has $.fileName;
    has Scope @.scopes;
    has %.allLibraries;
    has Scope $!currentScope;

    submethod BUILD {
        self.BeginScope("Outer scenario scope");
    }

    method BeginScope($name) {
        $!currentScope = Scope.new(
            name => ($name // 'unnamed block'),
            parent => $!currentScope );
        @!scopes.push($!currentScope);
    }

    method EndScope() {
        $!currentScope // fail "trying to end a scope where there is none.";

        @!scopes.pop();
        $!currentScope = $!currentScope.parent;
    }

    method RegisterLibrary($lib) {
        %!allLibraries{$lib.flatName} = $lib;
    }

    method AddLibraryToScope(StepLibrary $library) {
        my $flatName = $library.name.lc.subst(' ', '');
        if %!allLibraries{$flatName} :exists {
            $!currentScope.AddLibrary($library);
        }
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

        for $!currentScope.libraries {
            if .steps{$flatName}:exists {
                my $foundMethod = .steps{$flatName};
                return True, .name ~ "." ~ $foundMethod;
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
