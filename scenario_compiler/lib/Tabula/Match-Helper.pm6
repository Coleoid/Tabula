use v6;

role Match-Helper {

    method line-of-match-start($match) {
        1 + ($match.prematch.comb: /\n/)
    }

    method lines-in-match($match) {
        1 + ($match.comb: /\n/)
    }

    method source-location($match) {
        .file-name ~ ':' ~ .line-of-match-start($match);
    }

    method name-section($section-shape, $/) {
        my $start-line = .line-of-match-start($/);
        my $end-line = $start-line + .lines-in-match($/) - 2;

        sprintf("%s_from_%03d_to_%03d", $section-shape, $start-line, $end-line);
    }

}

role CSharp-Writer {
    method class-prefix() {
        q:to/END/;
        using System;
        using System.Collections.Generic;
        using System.Linq;

        namespace Tabula
        {
        END
    }

    method class-name() {
        .file-name.subst('.scn', '') ~ '_generated';
    }

    method class-declaration() {
'    public class ' ~ .class-name ~ '
        : GeneratedScenarioBase, IGeneratedScenario
    {
';
    }

    method class-scenario-label() {
'        public string ScenarioLabel = "' ~ .file-name ~ ':  ' ~ .scenario-title ~ '";
';
    }

    method class-constructor() {
'        public ' ~ .class-name ~ '(TabulaStepRunner runner)
            : base(runner)
        { }
';
    }

    method get-class-header() {
        .class-prefix ~
        .class-declaration ~
        .class-scenario-label ~ "\n"
        .class-constructor ~ "\n";
    }

    method get-class-footer() {
'    }
}
';
    }

}
