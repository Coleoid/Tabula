class CSharp-Scribe {
    has $.scenario-title is rw;
    has $.file-name is rw;
    has $!class-name;

    my $execute-body = "\n        ";
    my @paragraph-declarations;

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

    method get-class-execute-scenario() {
'        public void ExecuteScenario()
        {' ~ $execute-body ~ '}
';
    }

    method get-class-suffix() {
'    }
}
';
    }

    #TODO: calling the paragraph only via tables
    method add-para-to-scenario( $name ) {
        $execute-body ~= '    ' ~ $name ~ "();\n        ";
    }

    method declare-paragraph( $para ) {
        @paragraph-declarations.push( $para );
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
        self.get-class-suffix();
    }

}
