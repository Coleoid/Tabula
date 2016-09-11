class CSharp-Scribe {
    has $.scenario-title is rw;
    has $.file-name is rw;
    has $!class-name;

    my $execute-body;
    my @section-declarations;
    my @table-declarations;

    my $prior-paragraph;
    my $paragraph-used;

    method start-class() {
        $execute-body = "\n        ";
        $prior-paragraph = "";
        $paragraph-used = True;
        $!generated-SectionDeclarations = "";
        @section-declarations = ();
    }

    submethod BUILD {
        self.start-class();
    }

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
'    public class ' ~ $!class-name ~ '
        : GeneratedScenarioBase, IGeneratedScenario
    {
';
    }

    method get-class-scenario-label() {
'        public string ScenarioLabel = "' ~ $!file-name ~ ':  ' ~ $!scenario-title ~ '";
';
    }

    method get-class-constructor() {
'        public ' ~ $!class-name ~ '(TabulaStepRunner runner)
            : base(runner)
        { }
';
    }

    has $.generated-ExecuteScenario;
    method get-class-execute-scenario() {
        $!generated-ExecuteScenario =
'        public void ExecuteScenario()
        {' ~ $execute-body ~ '}
';
        $!generated-ExecuteScenario
    }

    has $.generated-SectionDeclarations;

    method get-section-declarations() {
        $!generated-SectionDeclarations = @section-declarations.elems == 0
            ?? ""
            !! "\n" ~ (join "\n", @section-declarations);

        return $!generated-SectionDeclarations;
    }

    method get-class-suffix() {
'    }
}
';
    }

    method Assemble() {
        $!class-name = self.get-class-name();
        self.add-section-to-scenario("");

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
