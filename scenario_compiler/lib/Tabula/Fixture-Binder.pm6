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
        my Fixture-Class $class;
        my Bool $found-class = False;

        for $file.IO.lines {
            if $found-class {
                if / public \s+ void \s+ (\w+ '(' <-[)]>* ')') / {
                    #say "  $0";
                    $class.add-method(~$0);
                }
            }

            if / public \s+ class \s+ (\w+) / {
                my $class-name = ~$0;
                $found-class = so ($class-name ~~ / Workflow $ /);
                if $found-class {
                    $class = Fixture-Class.new(class-name => $class-name);
                    self.add-class($class);
                    #say "$class-name:";
                }
                else {
                    $class = Nil;
                }
            }
        }
    }

    method add-class($class) {
        %!shelf{$class.key} = $class;
    }

    method get-class($name) {
        %!shelf{$name} || %!shelf{key-from-command-text($name)};
    }

    sub key-from-command-text($text) {
        $text.lc
            .subst('workflow', '')
            .subst('_', '', :g)
            .subst(' ', '', :g);
    }
}
