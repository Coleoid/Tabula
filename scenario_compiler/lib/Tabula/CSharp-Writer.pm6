use v6;

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
        self.file-name.subst('.scn', '') ~ '_generated';
    }

    method class-declaration() {
'    public class ' ~ self.class-name ~ '
        : GeneratedScenarioBase, IGeneratedScenario
    {
';
    }

    method class-scenario-label() {
'        public string ScenarioLabel = @"' ~ self.file-name ~ ':  "' ~ self.scenario-title ~ '"";
';
    }

    method get-class-header() {
        self.class-prefix ~
        self.class-declaration ~
        self.class-scenario-label ~ "\n";
    }

    method get-class-footer() {
'    }
}
';
    }

}
