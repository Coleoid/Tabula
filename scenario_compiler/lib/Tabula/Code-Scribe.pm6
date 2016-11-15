use v6;
use Tabula::Match-Helper;

#  Composition of a Tabula scenario into a C# class
class Code-Scribe
    does Match-Helper
    does CSharp-Writer {
    has $.scenario-title is rw;
    has $.file-name is rw;

    ##################################################################
    #  Sections (tables and paragraphs) are added by declare-section()
    has Str @.section-declarations;
    has Str $.section-declaration-text;
    method declare-section( $section ) {
        @!section-declarations.push( $section );
    }

    #  Create one instance of each fixture class used in a scenario.
    #  Seems fine for now.  Later complexities may prove me wrong.
    has %.declared-fixtures;
    has Str $.fixture-declaration-text;
    method declare-fixture($fixture) {
        if not %!declared-fixtures{$fixture.flat-name} {
            %!declared-fixtures{$fixture.flat-name} = $fixture;
            $!fixture-declaration-text ~= "        public " ~ $fixture.class-name ~ " "
                ~ $fixture.instance-name ~ " = new " ~ $fixture.class-name ~ "();\n";
        }
    }


    ##################################################################
    #  Our ExecuteScenario text, built by use-section-in-scenario()
    has Str $.execute-body-text;

    #  A paragraph may run directly, or per-row of tables below it,
    # so we stage each paragraph until we know what comes after it.
    has Str $.staged-paragraph;
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

    method use-section-in-scenario($name) {
        if $name ~~ /^paragraph/ { self.stage-paragraph($name); }
        elsif $name ~~ /^table/  { self.add-table-call($name); }
        elsif $name eq ''        { self.flush-pending-paragraph; }
        else { die "I do not know how to use a section named [$name]." }
    }


    ##################################################################
    #  Composing scenario sections of the scenario class

    method compose-paragraph($/) {
        my $name = self.name-section('paragraph', $/);

        my $para = "        public void " ~ $name ~ "()\n        \{\n "
            ~ [~] $<Statement>.map({ "           " ~ .made})
            ~ "        \}\n";

        self.declare-section($para);
        self.use-section-in-scenario($name);
        return $para;
    }

    method compose-table($/) {
        my $name = self.name-section('table', $/);

        my $table = '        ' ~ $name ~ " = new Table \{
            Header = new List<string>     " ~ $<Table-Header>.ast.chomp ~ ',';
            ~ '
            Data = new List<List<string>> {';

        for $<Table-Row> {
            $table ~= '
                new List<string>          ' ~ $_.ast.chomp ~ ',';
        }

        $table ~= '
            }
        };
';

        self.declare-section($table);
        self.use-section-in-scenario($name);
        return $table;
    }

    # The scenario at the table/paragraph granularity
    method compose-method-ExecuteScenario() {
        self.flush-pending-paragraph();
'        public void ExecuteScenario()
        {' ~ $!execute-body-text ~ '}
';
    }

    # Paragraph methods and table List declarations
    method compose-section-declarations() {
        $!section-declaration-text = @!section-declarations.elems == 0
            ?? ""
            !! "\n" ~ (join "\n", @!section-declarations);
    }

    #  Returns the full source file--includes, namespace, class.
    has Str $.full-source-file;
    method compose-file() {
        $!full-source-file =
            self.get-class-header() ~
            self.compose-method-ExecuteScenario() ~
            self.compose-section-declarations() ~
            self.get-class-footer();

        self.clear-work();
        return $!full-source-file;
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
}
