use v6;

#  Composition of a Tabula scenario into a C# class
class Code-Scribe {
    has $.scenario-title is rw;
    has $.file-name is rw;

    ##################################################################
    #  Sections (tables and paragraphs) are added by declare-section()
    has str @.section-declarations;
    has str $.section-declaration-text;

    method declare-section( $section ) {
        @!section-declarations.push( $section );
    }


    ##################################################################
    #  Our execution plan is built by use-section-in-scenario()
    has str $.execute-body-text;

    #  A paragraph may run directly or per-row of tables below it,
    # so we stage each paragraph until we know what comes after it.
    has str $.staged-paragraph;
    has Bool $.paragraph-pending;

    method add-paragraph-call($paragraph-name) {
        $!execute-body-text ~= '    ' ~ $paragraph-name ~ "();\n        ";
    }

    method add-table-call($table-name) {
        $!execute-body-text ~= '    Run_para_over_table( ' ~ $!staged-paragraph ~ ', ' ~ $table-name ~ " );\n        ";
        $!paragraph-pending = False;
    }

    method flush-pending-paragraph() {
        self.add-paragraph-call($!staged-paragraph) if $!paragraph-pending;
        $!paragraph-pending = False;
    }

    method stage-paragraph($name) {
        self.flush-pending-paragraph();
        $!staged-paragraph = $name;
        $!paragraph-pending = True;
    }

    method use-section-in-scenario( $name ) {
        if $name ~~ /^paragraph/ { self.stage-paragraph($name); }
        elsif $name ~~ /^table/  { self.add-table-call($name); }
        elsif $name eq ''        { self.flush-pending-paragraph(); }
        else { die "I do not know how to use a section named [$name]." }
    }


    ##################################################################
    #  Initialization

    method clear-work() {
        $!staged-paragraph = "";
        $!paragraph-pending = False;

        @!section-declarations = Empty;
        $!section-declaration-text = '';
        $!execute-body-text = "\n        ";
    }

    submethod BUILD {
        self.clear-work();
    }


    ##################################################################
    #  Composing small parts of the scenario class

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
'    public class ' ~ self.get-class-name() ~ '
        : GeneratedScenarioBase, IGeneratedScenario
    {
';
    }

    method get-class-scenario-label() {
'        public string ScenarioLabel = "' ~ $!file-name ~ ':  ' ~ $!scenario-title ~ '";
';
    }

    method get-class-constructor() {
'        public ' ~ self.get-class-name() ~ '(TabulaStepRunner runner)
            : base(runner)
        { }
';
    }

    method get-class-suffix() {
'    }
}
';
    }

    ##################################################################
    #  Composing the major sections of the scenario class

    method compose-class-prefix() {
        .get-class-prefix() ~
        .get-class-declaration() ~
        .get-class-scenario-label() ~ "\n"
        .get-class-constructor() ~ "\n";
    }

    method compose-method-ExecuteScenario() {
'        public void ExecuteScenario()
        {' ~ $!execute-body-text ~ '}
';
    }

    method compose-section-declarations() {
        $!section-declaration-text = @!section-declarations.elems == 0
            ?? ""
            !! "\n" ~ (join "\n", @!section-declarations);
    }

    #  Returns the full source file--includes, namespace, class.
    has str $.full-source-file;
    method compose-file() {
        $!full-source-file =
            self.compose-class-prefix() ~
            self.compose-method-ExecuteScenario() ~
            self.compose-section-declarations() ~
            self.get-class-suffix();

        self.clear-work();
        return $!full-source-file;
    }
}
