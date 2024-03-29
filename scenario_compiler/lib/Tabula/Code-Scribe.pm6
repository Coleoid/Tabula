use v6;

#  Normalizing join, to improve output sanity, because generated code
#   doesn't have to look like crap.
multi sub njoin( Int $count, *@elements, :$delim = "\n" )
    is export(:njoin) {
    return njoin( @elements, delim => $delim x $count );
}

multi sub njoin( *@elements, :$delim = "\n" )
    is export(:njoin) {
    my $separator = '';

    my $result = '';
    for @elements -> $element {
        if $element ~~ / ^^ $<content> = ([\s* \S+]+) \s* $$ / {
            $result ~= $separator ~ $<content>;
            $separator = $delim;
        }
    }

    return $result;
}

#  Composes a Tabula scenario into a C# class
class Code-Scribe {
    has $.scenario-title is rw;
    has $.file-name is rw;

    method class-file-prefix() {
        q:to/END/;
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using Acadis.Constants.Accounting;
        using Acadis.Constants.Admin;
        using Acadis.SystemUtilities;

        namespace Tabula
        {
        END
    }

    #TODO: a coherent technique for scenario suffixes
    method class-name() {
        self.file-name
            .subst('.scn', '')
            .subst('.tab', '')
            ~ '_generated'
    }

    method class-declaration() {
'    public class ' ~ self.class-name ~ '  //  ' ~ $!scenario-title ~ '
        : GeneratedScenarioBase, IGeneratedScenario
    {';
}

    method class-scenario-label() {
        '            ScenarioLabel = @"'
            ~ self.file-name ~ ':  "'
            ~ self.scenario-title ~ '"";';
    }

    method get-class-header() {
        njoin( self.class-file-prefix, self.class-declaration )
    }

    method get-class-footer() {
        "    }\n}";
    }



    ##################################################################
    #  Sections (tables and paragraphs) are added by declare-section()
    my @section-declarations;
    method declare-section( $section ) {
        push @section-declarations, $section;
    }

    my %seen-fixtures;
    my @fixture-declarations;
    my @fixture-instantiations;
    method initialize-fixture($fixture) {
        return if %seen-fixtures{$fixture.key};
        %seen-fixtures{$fixture.key} = True;

        my $full-class-name = $fixture.namespace ~ '.' ~ $fixture.class-name;
        push @fixture-declarations,
            "        public $full-class-name $($fixture.instance-name);";
        push @fixture-instantiations,
            "            $($fixture.instance-name) = new " ~ $($full-class-name) ~ '();';
    }


    ##################################################################
    #  Our ExecuteScenario text, built by add-next-section()
    my @body-actions;

    #  Tabula scenario flow:
    #  A paragraph may run directly, or per-row of tables below it,
    # so we stage each paragraph until we know what comes after it.
    has Str $.staged-paragraph;
    has Str $!staged-comment = '';
    has Bool $.paragraph-pending;

    method add-next-section($name, $comment = '') {
        if $name ~~ /^paragraph/ { self.stage-paragraph($name, $comment); }
        elsif $name ~~ /^table/  { self.add-table-call($name); }
        elsif $name eq ''        { self.flush-pending-paragraph; }
        else { die "I do not know how to add a section named [$name]." }
    }

    method stage-paragraph($name, $comment = '') {
        self.flush-pending-paragraph();
        $!staged-paragraph = $name;
        $!staged-comment = $comment;
        $!paragraph-pending = True;
    }

    method flush-pending-paragraph() {
        self.add-paragraph-call($!staged-paragraph, $!staged-comment) if $!paragraph-pending;
        $!paragraph-pending = False;
    }

    method add-paragraph-call($paragraph-name, $comment) {
        push @body-actions, '            ' ~ $paragraph-name ~ '();' ~ $comment;
    }

    method add-table-call($table-name, ) {
        push @body-actions, '            Run_para_over_table( '
            ~ $!staged-paragraph ~ ', ' ~ $table-name ~ " );" ~ $!staged-comment;
        $!paragraph-pending = False;
    }


    ##################################################################
    #  Composing code for paragraphs and tables of the scenario class,
    # and adding them to the ExecuteScenario() plan.

    sub get-Do-statement($code, $source-location) {
        my $quoted-code = '@"' ~ $code.subst('"', '""', :g) ~ '"';
        'Do(() =>     ' ~ $code
            ~ ',     "' ~ $source-location ~ '", ' ~ $quoted-code ~ ' );';
    }

    method compose-step-statement($call, $location) {
        return get-Do-statement($call, $location);
    }

    method compose-unfound($step-text, $location) {
        my $text = $step-text.subst('"', '\"', :g);

        #TODO: suggest near-matches
        return 'Unfound(     "' ~ $text ~ '",     "' ~ $location ~ '" );';
    }

    #TODO
    method compose-not-implemented($messsage, $location) {
        return '';
    }

    method compose-set-statement($lhs, $rhs, $location) {
        my $command = 'var[' ~ $lhs ~ '] = ' ~ $rhs;
        return get-Do-statement($command, $location);
    }


    method compose-paragraph($name, $label, $statements) {
        my $label-line = '';
        my $label-comment = '';
        if $label ne '' {
            $label-line = '            Label(  "' ~ $label ~ '" );';
            $label-comment = "   //  $label";
        }

        my $para = njoin(
            '        public void ' ~ $name ~ "()\n        \{",
            $label-line,
            $statements,
            '        }'
        ) ~ "\n";

        self.declare-section($para);
        self.add-next-section($name, $label-comment);
        return $para;
    }

    method compose-table($name, $header, @rows) {
        #TODO: Label

        my $table = njoin(
'        public Table ' ~ $name ~ '()
        {
            return new Table {
                Header = new List<string>     ' ~ $header ~ ',',
'                Data = new List<List<string>> {',
            @rows.map({
'                    new List<string>          ' ~ $_ ~ ',' }),
'                }
            };
        }
');

        self.declare-section($table);
        self.add-next-section($name);
        return $table;
    }


    ##################################################################
    #  Composing portions of the scenario class file

    method compose-constructor() {
        njoin(
'        public ' ~ self.class-name ~ '()
            : base()
        {',
            self.class-scenario-label,
            @fixture-instantiations,
'        }'
        );
    }

    #  ExecuteScenario() runs the scenario as it was sequenced, at the
    # paragraph granularity.
    method compose-method-ExecuteScenario() {
        self.flush-pending-paragraph();
        njoin(
'        public void ExecuteScenario()
        {',
            @body-actions,
'        }'
        );
    }

    # Paragraph and Table method declarations
    method compose-fixture-declarations() {
        njoin(@fixture-declarations)
    }

    #  Returns the full source file--includes, namespace, class.
    method compose-file() {
        njoin(
            self.get-class-header(),
            njoin( 2,
                self.compose-method-ExecuteScenario(),
                @section-declarations,
                self.compose-fixture-declarations(),
                self.compose-constructor(),
            ),
            self.get-class-footer()
        ) ~ "\n";
    }


    ##################################################################
    #  Unit testing helpers for readability

    method trimmed-body-actions() is export(:test-content) {
        njoin( @body-actions.map({.trim}) ) ~ "\n"
    }

    method show-sections() is export(:test-content) {
        njoin( 2, @section-declarations ) ~ "\n"
    }


    ##################################################################
    #  Initialization

    method clear-work() {
        $!staged-paragraph = '';
        $!paragraph-pending = False;

        %seen-fixtures = Empty;
        @fixture-declarations = Empty;
        @fixture-instantiations = Empty;

        @body-actions = Empty;
        @section-declarations = Empty;
    }

    submethod BUILD {
        self.clear-work();
    }
}
