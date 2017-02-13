use v6;
use Tabula::Execution-Context;
use Tabula::Code-Scribe;
use Tabula::Fixture-Binder;
use Tabula::Match-Helper;

class Target-Testopia does Match-Helper {
    has Execution-Context $.Context;
    has Code-Scribe       $.Scribe;
    has Fixture-Binder    $.Binder;

    submethod BUILD {
        $!Context = Execution-Context.new;
        $!Scribe  = Code-Scribe.new;
        $!Binder  = Fixture-Binder.new;
    }

    #REFAC:  Get this out, into Target-Testopia
    sub get-Do-statement( $code, $source-location ) {
        my $quoted-code = '@"' ~ $code.subst('"', '""', :g) ~ '"';

        'Do(() =>     '
            ~ $code ~ ',     "'
            ~ $source-location ~ '", '
            ~ $quoted-code ~ ' );';
    }

    method normalized-name-CSharp() {
        $!Context.file-name.subst('.scn', '').subst('.tab', '')
    }


    ##################################################################
    ### Grammar methods, alphabetically (putting TOP at the bottom :)

    #nn?
    method Action($/) {
        make ($<Step> || $<Block> || $<Command>).made
    }

    # complete crap
    method Block($/) {
        $!Context.open-scope($<String>);

        make
            '{  //  ' ~ ($<String> // "unnamed block") ~ "\n"
            ~ ($<Section> ?? [~] $<Section>.map({.made}) !! "")
            ~ $<Block-End> ~ "\}\n";

        $!Context.close-scope();
    }

    #nn?
    method Command($/) {
        # Set emits code back into the streams
        make ($<Command-Use> || $<Command-Tag> || $<Command-Set>).made
    }

    # mature
    method Command-Set($/) {
        my $source-location = $!Context.source-location($/);

        my $lhs = $<Word>
            ?? '"' ~ $<Word>.lc ~ '"'
            !! $<Variable>.made;
        my $rhs = $<Term>.made;

        make $!Scribe.compose-set-statement($lhs, $rhs, $source-location);
    }

    #--
    method Command-Alias($/) {
        $!Context.add-alias($/);
    }

    #NI
    method Command-Tag($/) {
        my $source-location = $!Context.source-location($/);
        my $cmd = $<PhraseList>.elems == 1 ?? 'tag' !! 'tags';
        make $!Scribe.compose-not-implemented(">$cmd: $<Phrases>", $source-location);
    }

    # mature
    method Command-Use($/) {
        for $<Phrases><Phrase> -> $fixture-label {
            my $fixture = $!Binder.get-class(~$fixture-label);
            if so $fixture {
                $!Context.add-fixture($fixture);
                $!Scribe.initialize-fixture($fixture);
            }
            else {
                warn "unable to find fixture matching [$fixture-label].";
            }
        }

        make ""
    }

    #scribe
    method Paragraph($/) {
        my $name = self.name-section('paragraph', $/);
        my $label = $<Paragraph-Label> ?? $<Paragraph-Label><String><Body> !! '';
        my $statements = [~] $<Statement>.map({ .made ?? ("            " ~ .made) !! "" });

        make $!Scribe.compose-paragraph( $name, $label, $statements );
    }

    method Para-Open($) {
        $!Context.open-scope('');
    }

    method Para-Close($) {
        $!Context.close-scope();
    }

    # mature
    method Phrase($/) {
        my $for-Cell = ($*Phrase-Context.defined and $*Phrase-Context eq 'Cell');
        my $single-Term = $<Symbol>.elems == 1 && (! $<Symbol>[0]<Word>.defined);

        my $symbols = $<Symbol>.map({.made}).join(' ');

        if $single-Term or not $for-Cell {
            make $symbols;
            return;
        }

        make '"' ~ $symbols.subst('"', '\"', :g) ~ '"';
    }

    #text
    method Phrases($/) {
        make $<Phrase>.map({.made}).join(', ');
    }

    # mature
    method Scenario($/) {
        $!Scribe.scenario-title = $<String>;
        $!Scribe.file-name = $!Context.file-name;

        make $!Scribe.compose-file();
    }

    #nn?
    method Section($/) {
        make ($<Paragraph> || $<Table> || $<Document> || $<Break-Line>).made
    }

    #echo
    method Statement($/) {
        my $statement = $<Action>.made;
        my $comment-suffix = ($<Comment> ?? "  " ~ $<Comment> !! "");
        if $statement or $comment-suffix {
            make $statement ~ $comment-suffix ~ "\n";
        }
        else {
            make "";
        }
    }

    #inline C#
    method Step($match) {
        my $source-location = $!Context.source-location($match);

        my $result = $!Context.resolve-step($match);
        if $result {
            $match.make( get-Do-statement( $result, $source-location ) );
        }
        else {
            #TODO: this is where suggestions of near-matches will go
            my $stepText = $match.subst('"', '\"', :g);
            $match.make( 'Unfound(     "' ~ $stepText ~ '",     "' ~ $source-location ~ '" );' );
        }
    }

    #echo
    method Symbol($/) {
        make $<Word> || $<Term>.made
    }

    # mature
    method Table($/) {
        my $name = self.name-section('table', $/);
        my $header = $<Table-Header>.made;
        my @rows = $<Table-Row>.map({.made});

        make $!Scribe.compose-table($name, $header, @rows);
    }

    #nn?
    method Table-Cell($/) {
        if $<Phrases> { make $<Phrases>.made }
        else { make $<Empty-Cell> }
    }

    #echo
    method Table-Cells($/) {
        make $<Table-Cell>.map({.made}).join(', ')
    }

    #inline C#
    method Table-Header($/) {
        make $<Indentation> ~ '{ ' ~ $<Table-Cells>.made ~ ' }'
    }

    #echo
    method Table-Label($/) {
        make "$<Indentation>=== $<Step> ===\n"
    }

    #inline C#
    method Table-Row($/) {
        make $<Indentation> ~ '{ ' ~ $<Table-Cells>.made ~ ' }'
    }

    method Term($/) {
        if    $<Date>   { make '"' ~ $<Date>   ~ '"' }
        elsif $<Number> { make '"' ~ $<Number> ~ '"' }
        elsif $<String> { make $<String> }
        else            { make $<Variable>.made }
    }

    #C#
    method Variable($/) {
        make 'var["' ~ $<Word>.lc ~ '"]';
    }

}
