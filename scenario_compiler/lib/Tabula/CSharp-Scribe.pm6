class CSharp-Scribe {
    has $.scenario-title is rw;
    has $.file-name is rw;
    has $!class-name;

    my $execute-body = "\n        ";
    my @section-declarations;
    my @table-declarations;

    my $prior-paragraph = "";
    my $paragraph-used = True;

    method add-section-to-scenario( $name ) {

        if $name ~~ /^paragraph/ {
            if not $paragraph-used {
                $execute-body ~= '    ' ~ $prior-paragraph ~ "();\n        ";
            }
            $prior-paragraph = $name;
            $paragraph-used = False;
        }
        elsif $name ~~ /^table/ {
            $execute-body ~= '    Run_para_over_table( ' ~ $prior-paragraph ~ ', ' ~ $name ~ " );\n        ";
            $paragraph-used = True;
        }
        elsif $name eq '' {
            if not $paragraph-used {
                $execute-body ~= '    ' ~ $prior-paragraph ~ "();\n        ";
            }
        }
        else { die "what is this name [$name]?" }
    }

    method declare-section( $section ) {
        @section-declarations.push( $section );
    }


    method get-class-prefix() {
        q:to/END/;
        using System;
        using System.Collections.Generic;
        using System.Linq;

        namespace Tabula
        {
        END
    }

    method get-class-name() {
        $!file-name.subst('.scn', '') ~ '_generated';
    }

    method get-class-declaration() {
        qq:to/END/;
            public class $!class-name
                : GeneratedScenarioBase, IGeneratedScenario
            \{
        END
    }

    method get-class-scenario-label() {
        qq:to/END/;
                public string ScenarioLabel = "$!file-name:  $!scenario-title";
        END
    }

    method get-class-constructor() {
        qq:to/END/;
                public $!class-name\(TabulaStepRunner runner)
                    : base(runner)
                \{ \}
        END
    }

    has $.class-ExecuteScenario;

    method get-class-execute-scenario() {
        # no further sections on the way, so add any unused paragraph to body.
        self.add-section-to-scenario('');

        $!class-ExecuteScenario = qq:to/END/;
                public void ExecuteScenario()
                \{$execute-body\}
        END

        return $!class-ExecuteScenario;
    }

    method get-section-declarations() {
        return "" if @section-declarations.elems == 0;
        return "\n" ~ (join "\n", @section-declarations);
    }

    method get-class-suffix() {
'    }
}
';
    }

    method clear-scenario() {
        $prior-paragraph = "";
        $paragraph-used = True;
        $execute-body = "\n        ";
    }

    method Assemble() {
        $!class-name = self.get-class-name();

        self.get-class-prefix() ~
        self.get-class-declaration() ~
        self.get-class-scenario-label() ~
        "\n" ~
        self.get-class-constructor() ~
        "\n" ~
        self.get-class-execute-scenario() ~
        self.get-section-declarations() ~
        self.get-class-suffix();
    }

}
