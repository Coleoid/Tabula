use v6;

#   ===================================
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


#   ===================================
#   A set of fantasy (*-f) classes to clarify the API now that I've
#   grappled with making the code work for a while.

#  Fixture:  A class containing testing methods to be called by the
# class which implements a Tabula scenario.
class Fixture-f {
    #  Keys are flat names suffixed with an encoding of its arg types,
    # perhaps ":ssis" or "(sdis)" or similar, something easy to parse
    # and good for approximate matching.
    has %.methods;

    method add-method { ... }
}

#  Holding fixtures, labels, and aliases for a portion of the scenario.
class Scope-f {
    has $.name;
    has $.parent;

    #  Methods defined in multiple fixtures will be found in the most recently
    # added fixture.
    has Fixture-f @.fixtures;
    method add-fixture($fixture) { ... }

    #  An alias value may be defined in terms of other aliases, but only
    # those which have already been defined.  Resolution is most-recent-first.
    has %.aliases;
    method add-alias($alias, $value) { ... }

    #  Arguments, table values, any other symbolic reference
    has %.labels;
    method add-label($label, $value) { ... }
}

class Execution-Context-f {
    has Scope-f $!current-scope;
    method open-scope() { ... }
    method close-scope() { !!! }

    #  Delegates to Scope-f
    method add-fixture($fixture) { ... }
    method add-alias($alias, $value) { ... }
    method add-label($label, $value) { ... }

    #  Given an action match, finds step method it fits, and
    # resolve any labels in its arguments.
    #  Begins in current scope, stops when it finds a step method.
    method resolve-action($action) { ... }
    #  ?:  Should we scan each fixture only once during each resolution?

    #  Determine how an argument should be shown in source.
    #  Type preparation, nested label replacements, ...
    method resolve-arg($arg) { ... }

    #  Fetch the value of a label
    method resolve-label($label) { ... }
    #  Label values which include other labels resolve backward only,
    # with the intention (and hope) that this prevents any loops.

}

#  Loads fixtures from disk, parses methods and arguments, holds results
class Fixture-Binder-f {
    has %.fixtures;

    #  Main responsibilities
    method load-fixtures($folder) { ... }
    method parse-source($source) { ... }
    method get-fixture($name) { ... }

    method canonicalize-fixture-name($name) { ... }
    method canonicalize-method-name($name) { ... }
}


#  Writes the code which implements the Tabula scenario.
class Code-Scribe-f {
    method declare-section( $section ) { ... }
    method add-section-to-scenario( $name ) { ... }
    method Assemble() { ... }

}

#   ===================================
class Scope {
    has $.name;
    has $.parent;
    has @!problems;

    has StepLibrary @.libraries;

    submethod BUILD(:$!name, :$!parent, :@!problems) {}

    method add-problem($problem) {
        @!problems.push( $problem );
    }

    method AddLibrary(StepLibrary $library) {
        for @!libraries {
            if $_ ~~ $library {
                add-problem "Tried to add duplicate library <<$($library.flat-name)>>.";
            }
        }
        @!libraries.push($library);
    }


    method !find-step-method($flatName, $arg-count) {

        #TODO:  alias resolution (has failing test 25 oct 16)
        for self.aliases {
        }

        for self.libraries {
            #TODO:  Distinguish overloads by argument count
            if .steps{$flatName}:exists && .steps{$flatName}[1] == $arg-count {
                my $foundMethod = .steps{$flatName}[0];
                return .instance-name ~ "." ~ $foundMethod;
            }
        }

        if not $!parent { add-problem "No step matching <<$flatName>> with [$arg-count] arguments."; }
        $!parent.find-step-method($flatName, $arg-count);
    }

    multi method AddLibraryToScope(Str $phrase) {
        my $flatName = $phrase.lc.subst(' ', '', :g).subst('workflow', '');
        my $library = %!registry{$flatName} // 0;
        if $library {
            self.AddLibraryToScope($library);
        }
        else {
            add-problem "Did not find library matching <<$phrase>> to add to scope.";
        }
    }

    multi method AddLibraryToScope(StepLibrary $library) {
        my $result = self.AddLibrary($library);
        if not $result {
            add-problem $result.exception.message;
        }
    }
}


#   ===================================
class Build-Context {
    has Scope @.scopes;
    has Scope $.current-scope handles <AddLibraryToScope>;

    has Str $.file-name is rw;
    has %.registry;
    has %.yet-initialized;
    has @.problems;
    has $.lib-declarations;

    submethod BUILD {
        self.BeginScope("Outer scenario scope");
    }

    method BeginScope($name = 'unnamed block') {
        $!current-scope = Scope.new(
            name => $name,
            parent => $!current-scope,
            problems => @!problems,
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

    method GetFixtureCall($match, $location) {
        my $result = self!get-method-name($match);

        if $result {
            my $argsText = getArgsText($match);
            return $result ~ "($argsText)";
        }
        else {
            #TODO:  Text::Levenshtein::Damerau to find close misses
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
