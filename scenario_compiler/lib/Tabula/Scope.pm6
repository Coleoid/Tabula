use v6;
use Tabula::Fixture;

#  Holding fixtures, labels, and aliases for a portion of the scenario.
class Scope {
    has $.name;
    has Scope $.parent;
    submethod BUILD(:$!name, :$!parent) {
        @!fixtures = Empty;
    }

    #  Methods defined in multiple fixtures will be found in the most recently
    # added fixture.
    has @.fixtures;
    method add-fixture(Fixture $fixture) {
        @!fixtures.push($fixture);
    }

    #  An alias value may be defined in terms of other aliases, but only
    # those which have already been defined.  Resolution is most-recent-first.
    has %.aliases;
    method add-alias($alias, $value) { ... }

    #  Arguments, table values, any other symbolic reference
    has %.labels;
    method add-label($label, $value) { ... }

    method find-step-method($flatName, $arg-count) {
        for .fixtures {
            #WHARGARBRGL
        }
    }

    method resolve-step($step) {
        my $method-call = .find-step-method($step);

        return $method-call || .parent
            ?? .parent.resolve-step($step)
            !! fail "No method matching ($step) found in scope.";
    }

}

#   ===================================
# class Scope-old {
#     has $.name;
#     has $.parent;
#     has @!problems;
#
#     has StepLibrary @.libraries;
#
#     submethod BUILD(:$!name, :$!parent, :@!problems) {}
#
#     method add-problem($problem) {
#         @!problems.push( $problem );
#     }
#
#     method AddLibrary(StepLibrary $library) {
#         for @!libraries {
#             if $_ ~~ $library {
#                 add-problem "Tried to add duplicate library <<$($library.flat-name)>>.";
#             }
#         }
#         @!libraries.push($library);
#     }
#
#
#     method !find-step-method($flatName, $arg-count) {
#
#         #TODO:  alias resolution (has failing test 25 oct 16)
#         for self.aliases {
#         }
#
#         for self.libraries {
#             #TODO:  Distinguish overloads by argument count
#             if .steps{$flatName}:exists && .steps{$flatName}[1] == $arg-count {
#                 my $foundMethod = .steps{$flatName}[0];
#                 return .instance-name ~ "." ~ $foundMethod;
#             }
#         }
#
#         if not $!parent { add-problem "No step matching <<$flatName>> with [$arg-count] arguments."; }
#         $!parent.find-step-method($flatName, $arg-count);
#     }
#
#     multi method AddLibraryToScope(Str $phrase) {
#         my $flatName = $phrase.lc.subst(' ', '', :g).subst('workflow', '');
#         my $library = %!registry{$flatName} // 0;
#         if $library {
#             self.AddLibraryToScope($library);
#         }
#         else {
#             add-problem "Did not find library matching <<$phrase>> to add to scope.";
#         }
#     }
#
#     multi method AddLibraryToScope(StepLibrary $library) {
#         my $result = self.AddLibrary($library);
#         if not $result {
#             add-problem $result.exception.message;
#         }
#     }
# }
