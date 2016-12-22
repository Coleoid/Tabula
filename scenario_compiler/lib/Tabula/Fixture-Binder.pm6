use v6;
use Tabula::Fixture-Class;

#  Loads fixtures from disk, binds them into books, shelves results
class Fixture-Binder {
    has %.shelf;

    #  Main responsibilities
    method load-fixtures($folder) {

        my @folders;
        if $folder.IO.d {
            @folders.push($folder);
        }
        else {
            die "[$folder] is not a folder within [$*CWD].";
        }

        while (my $target = @folders.pop) {
            my @folder-contents = dir $target;

            for @folder-contents {
                when .IO.d {
                    @folders.push($_) unless / '\.' \w /;  # skip dot-folders
                }
                when / .+ '.' cs $ / {
                    self.parse-source($_);
                }
            }
        }
    }

    method parse-source($file) {
        my Fixture-Class $book;
        my Bool $found-workflow = False;

        for $file.IO.lines {
            if $found-workflow {
                if / public \s+ void \s+ (\w+ '(' <-[)]>* ')') / {
                    #say "  $0";
                    $book.add-method(~$0);
                }
            }

            if / public \s+ class \s+ (\w+) / {
                my $class-name = ~$0;
                $found-workflow = so ($class-name ~~ / Workflow $ /);
                if $found-workflow {
                    $book = Fixture-Class.new(class-name => $class-name);
                    self.add-workflow($book);
                    #say "$class-name:";
                }
                else {
                    $book = Nil;
                }
            }
        }
    }

    method add-workflow($book) {
        %!shelf{$book.key} = $book;
    }

    method get-workflow($name) {
        %!shelf{$name} || %!shelf{key-from-command-text($name)};
    }

    sub key-from-command-text($text) {
        $text.lc
            .subst('workflow', '')
            .subst('_', '', :g)
            .subst(' ', '', :g);
    }
}
