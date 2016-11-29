use v6;
use Tabula::CSharp-Writer;
use Tabula::Match-Helper;

#  Composes a Tabula scenario into a C# class
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

    has %.declared-fixtures;
    has Str $.fixture-declaration-text;
    method declare-fixture($fixture) {
        if not %!declared-fixtures{$fixture.key} {
            %!declared-fixtures{$fixture.key} = $fixture;
            $!fixture-declaration-text ~= "        public " ~ $fixture.class-name ~ " "
                ~ $fixture.instance-name ~ ";\n";
        }
    }

    #  The fixture instantiations go into the generated constructor.
    has %.instantiated-fixtures;
    has Str $.fixture-instantiation-text;
    method instantiate-fixture($fixture) {
        if not %!instantiated-fixtures{$fixture.key} {
            %!instantiated-fixtures{$fixture.key} = $fixture;
            $!fixture-instantiation-text ~= "            " ~
                $fixture.instance-name ~ " = new " ~ $fixture.class-name ~ "();\n";
        }
    }


    ##################################################################
    #  Our ExecuteScenario text, built by add-next-section()
    has Str $.execute-body-text;

    #  Tabula scenario flow:
    #  A paragraph may run directly, or per-row of tables below it,
    # so we stage each paragraph until we know what comes after it.
    has Str $.staged-paragraph;
    has Bool $.paragraph-pending;

    method add-paragraph-call($paragraph-name) {
        $!execute-body-text ~= '            ' ~ $paragraph-name ~ "();\n";
    }

    method add-table-call($table-name) {
        $!execute-body-text ~= '            Run_para_over_table( ' ~ $!staged-paragraph ~ ', ' ~ $table-name ~ " );\n";
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

    method add-next-section($name) {
        if $name ~~ /^paragraph/ { self.stage-paragraph($name); }
        elsif $name ~~ /^table/  { self.add-table-call($name); }
        elsif $name eq ''        { self.flush-pending-paragraph; }
        else { die "I do not know how to add a section named [$name]." }
    }


    ##################################################################
    #  Composing code for paragraphs and tables of the scenario class,
    # and adding them to the ExecuteScenario() plan.

    method compose-paragraph($/) {
        my $name = self.name-section('paragraph', $/);
        my $statements = [~] $<Statement>.map({ .made ?? ("            " ~ .made) !! "" });

        my $para = "        public void " ~ $name ~ "()\n        \{\n"
            ~ $statements
            ~ "        \}\n";

        self.declare-section($para);
        self.add-next-section($name);
        return $para;
    }

    #TODO: Table Labels
    method compose-table($/) {
        my $name = self.name-section('table', $/);

        #TODO: Row hashes
        my $table = '        public Table ' ~ $name ~ '()
        {
            return new Table {
                Header = new List<string>     ' ~ $<Table-Header>.ast.chomp ~ ','
            ~ '
                Data = new List<List<string>> {';

        for $<Table-Row> -> $row {
            $table ~= '
                    new List<string>          ' ~ $row.ast.chomp ~ ',';
        }

        $table ~= '
                }
            };
        }
';

        self.declare-section($table);
        self.add-next-section($name);
        return $table;
    }


    ##################################################################
    #  Composing portions of the scenario class file

    method compose-constructor() {
'        public ' ~ self.class-name ~ '(TabulaStepRunner runner)
            : base(runner)
        {
'       ~ $!fixture-instantiation-text ~ '        }
';
    }

    #  ExecuteScenario() runs the scenario as it was sequenced, at the
    # paragraph granularity.
    method compose-method-ExecuteScenario() {
        self.flush-pending-paragraph();
'        public void ExecuteScenario()
        {
' ~ $!execute-body-text ~ '        }
';
    }

    # Paragraph and Table method declarations
    method compose-section-declarations() {
        $!section-declaration-text = @!section-declarations.elems == 0
            ?? ""
            !! "\n" ~ (join "\n", @!section-declarations);
    }

    #  Returns the full source file--includes, namespace, class.
    method compose-file() {
        my $full-source-file =
            self.get-class-header() ~
            $!fixture-declaration-text ~ "\n" ~
            self.compose-constructor() ~ "\n" ~
            self.compose-method-ExecuteScenario() ~
            self.compose-section-declarations() ~
            self.get-class-footer();

        return $full-source-file;
    }


    ##################################################################
    #  Initialization

    method clear-work() {
        $!staged-paragraph = '';
        $!paragraph-pending = False;

        @!section-declarations = Empty;
        $!section-declaration-text = '';
        $!fixture-declaration-text = '';
        $!fixture-instantiation-text = '';
        $!execute-body-text = '';
    }

    submethod BUILD {
        self.clear-work();
    }
}
